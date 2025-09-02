using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRecordingDataController : MonoBehaviour
{
    public float loudnessSensibility;
    float threshold = 0.1f;
    public AudioSource audioSource;
    public int sampleWindow;
    public float time;
    public bool isRecording;
    public float loudness;
    public float refreshFrequency;
    public List<float> dbData = new List<float>();
    public GameObject spectrumElementsHolder;

    private void Start()
    {
        //StartRecordingData(audioSource.clip.length);
    }

    public void StartRecordingData(float newTime)
    {
        isRecording = true;
        time = newTime;
        dbData.Clear();
        StartCoroutine(LoadData());
    }

    IEnumerator SaveData()
    {
        while (time > 0)
        {
            if (loudness > 12)
            {
                loudness = 12f;
            }
            
            dbData.Add(loudness);
            yield return new WaitForSeconds(refreshFrequency);

        }

        isRecording = false;
        Debug.Log("Termmino la Grabacion");
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (isRecording && time > 0)
    //    {
    //        loudness = GetLoudnessFromMicrophone() * loudnessSensibility;
    //        time -= Time.deltaTime;

    //        if (loudness < threshold)
    //            loudness = 0.2F;
    //    }
    //}

    IEnumerator LoadData()
    {
        while (time > 0)
        {
            if (loudness > 15)
            {
                loudness = 15f;
            }
            
            dbData.Add(loudness);
            yield return new WaitForSeconds(refreshFrequency);
        }

        audioSource.Stop();
        isRecording = false;
        Debug.Log("Termmino la Grabacion");
    }

    private void Update()
    {
        if (isRecording)
        {
            if (loudness < threshold)
                loudness = 0.2F;
            
            loudness = loudnessSensibility * GetLoudnessFromAudioClip(audioSource.timeSamples, audioSource.clip);
            time -= Time.deltaTime;
        }
    }

    public float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
    {
        int startPosition = clipPosition - sampleWindow;

        if (startPosition < 0)
            return 0;
        
        float[] wavesData = new float[sampleWindow];
        clip.GetData(wavesData, startPosition);

        float totalLoudness = 0;

        for (int i = 0; i < sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(wavesData[i]);
        }

        return totalLoudness / sampleWindow;
    }
}
