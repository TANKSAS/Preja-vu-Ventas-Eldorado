using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OutputAudioRecorder : MonoBehaviour
{
    internal string FILENAME;
    private int outputRate;
    private int headerSize = 44; //default for uncompressed wav
    public String fileName;
    private bool recOutput = false;
    private FileStream fileStream;
    [SerializeField] float[] tempDataSource;
    public string currentFullPath;

    [SerializeField] List<float> segmentRecordingBuffer = new List<float>();
    [SerializeField] bool isRecordingSegment = false;
    
    void Start()
    {
        outputRate = AudioSettings.outputSampleRate;
    }

    public void StartRecording()
    {
        FILENAME = "record " + UnityEngine.Random.Range(1,1000);
        fileName = Path.GetFileNameWithoutExtension(FILENAME) + ".WAV";
        currentFullPath = Path.Combine(Application.persistentDataPath, fileName);

        if (!recOutput)
        {
            StartWriting(fileName);
            recOutput = true;
            Debug.Log("Start Recording");
        }
    }

    public void StopRecording()
    {
        recOutput = false;
        WriteHeader();
        Debug.Log("Stop Recording");
    }

    public void StartSegmentRecording()
    {
        segmentRecordingBuffer.Clear();
        isRecordingSegment = true;
        Debug.Log("Start Segment Recording");
    }

    public string StopSegmentRecordingAndSave()
    {
        isRecordingSegment = false;
        Debug.Log("Stop Segment Recording");

        // Ahora guardamos segmentRecordingBuffer a un archivo temporal
        string segmentFileName = "Segment_" + UnityEngine.Random.Range(1, 1000) + ".wav";
        string segmentFullPath = Path.Combine(Application.persistentDataPath, segmentFileName);

        SaveFloatArrayToWav(segmentRecordingBuffer.ToArray(), segmentFullPath);

        return segmentFullPath; // te devuelve el path para que lo uses (enviar a IA, etc)
    }


    public void OnAudioFilterRead(float[] data, int channels)
    {
        if (recOutput)
        {
            ConvertAndWrite(data); //audio data is interlaced
        }


        if (isRecordingSegment)
        {
            segmentRecordingBuffer.AddRange(data); // Guarda pedazo en memoria
        }
    }

    private void StartWriting(string name)
    {
        fileStream = new FileStream(Application.persistentDataPath + "/" + name, FileMode.Create);
        var emptyByte = new byte();
        for (int i = 0; i < headerSize; i++) //preparing the header
        {
            fileStream.WriteByte(emptyByte);
        }
    }

    private void ConvertAndWrite(float[] dataSource)
    {
        var intData = new Int16[dataSource.Length];
        //converting in 2 steps : float[] to Int16[], //then Int16[] to Byte[]
        var bytesData = new Byte[dataSource.Length * 2];
        //bytesData array is twice the size of
        //dataSource array because a float converted in Int16 is 2 bytes.
        var rescaleFactor = 32767; //to convert float to Int16
        for (var i = 0; i < dataSource.Length; i++)
        {
            intData[i] = (Int16)(dataSource[i] * rescaleFactor);
            var byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }
        fileStream.Write(bytesData, 0, bytesData.Length);
        tempDataSource = new float[dataSource.Length];
        tempDataSource = dataSource;
    }

    private void SaveFloatArrayToWav(float[] floatArray, string filePath)
    {
        int sampleRate = AudioSettings.outputSampleRate;
        int channels = 2; // o 1 si querés mono

        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            int headerSize = 44;
            int length = floatArray.Length;

            // Escribimos header vacío
            for (int i = 0; i < headerSize; i++)
                fileStream.WriteByte(0);

            // Convertimos y escribimos el audio
            var intData = new Int16[floatArray.Length];
            var bytesData = new Byte[floatArray.Length * 2];
            var rescaleFactor = 32767;

            for (int i = 0; i < floatArray.Length; i++)
            {
                intData[i] = (short)(floatArray[i] * rescaleFactor);
                var byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }

            fileStream.Write(bytesData, 0, bytesData.Length);

            // Ahora escribimos el header correctamente
            fileStream.Seek(0, SeekOrigin.Begin);
            var riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            fileStream.Write(riff, 0, 4);
            var chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
            fileStream.Write(chunkSize, 0, 4);
            var wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            fileStream.Write(wave, 0, 4);
            var fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            fileStream.Write(fmt, 0, 4);
            var subChunk1 = BitConverter.GetBytes(16);
            fileStream.Write(subChunk1, 0, 4);
            UInt16 one = 1;
            var audioFormat = BitConverter.GetBytes(one);
            fileStream.Write(audioFormat, 0, 2);
            var numChannels = BitConverter.GetBytes((ushort)channels);
            fileStream.Write(numChannels, 0, 2);
            var sampleRateBytes = BitConverter.GetBytes(sampleRate);
            fileStream.Write(sampleRateBytes, 0, 4);
            var byteRate = BitConverter.GetBytes(sampleRate * channels * 2);
            fileStream.Write(byteRate, 0, 4);
            UInt16 blockAlign = (ushort)(channels * 2);
            fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);
            UInt16 bitsPerSample = 16;
            fileStream.Write(BitConverter.GetBytes(bitsPerSample), 0, 2);
            var dataString = System.Text.Encoding.UTF8.GetBytes("data");
            fileStream.Write(dataString, 0, 4);
            var subChunk2 = BitConverter.GetBytes(length * 2);
            fileStream.Write(subChunk2, 0, 4);
        }
    }

    private void WriteHeader()
    {
        fileStream.Seek(0, SeekOrigin.Begin);
        var riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);
        var chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);
        var wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);
        var fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);
        var subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);
        UInt16 two = 2;
        UInt16 one = 1;
        var audioFormat = BitConverter.GetBytes(one);
        fileStream.Write(audioFormat, 0, 2);
        var numChannels = BitConverter.GetBytes(two);
        fileStream.Write(numChannels, 0, 2);
        var sampleRate = BitConverter.GetBytes(outputRate);
        fileStream.Write(sampleRate, 0, 4);
        var byteRate = BitConverter.GetBytes(outputRate * 4);
        fileStream.Write(byteRate, 0, 4);
        UInt16 four = 4;
        var blockAlign = BitConverter.GetBytes(four);
        fileStream.Write(blockAlign, 0, 2);
        UInt16 sixteen = 16;
        var bitsPerSample = BitConverter.GetBytes(sixteen);
        fileStream.Write(bitsPerSample, 0, 2);
        var dataString = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(dataString, 0, 4);
        var subChunk2 = BitConverter.GetBytes(fileStream.Length - headerSize);
        fileStream.Write(subChunk2, 0, 4);
        fileStream.Close();
    }
}