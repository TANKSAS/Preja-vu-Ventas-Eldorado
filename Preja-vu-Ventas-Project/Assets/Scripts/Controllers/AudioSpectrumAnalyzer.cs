using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpectrumAnalyzer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip microphoneClip;
    public GameObject barPrefab;
    public float barWidth = 1f;
    public float maxHeight = 10f;

    private int sampleCount = 1024; // Número de muestras para analizar el espectro
    public float[] spectrumData;

    private void Awake()
    {
        StartMicrophone();
    }

    void StartMicrophone()
    {
        string microphoneName = Microphone.devices[0];
        microphoneClip = Microphone.Start(microphoneName, true, 1, AudioSettings.outputSampleRate);
        audioSource.clip = microphoneClip;
        audioSource.Play();// Obtener los datos del espectro de audio
    }

    private void Start()
    {
        spectrumData = new float[sampleCount / 2];
    }

    private void Update()
    {
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Rectangular);
        

        // Crear barras para visualizar el espectro
        for (int i = 0; i < spectrumData.Length; i++)
        {
            // Calcular la altura de la barra en función del espectro de audio
            float height = Mathf.Clamp(spectrumData[i] * maxHeight, 0f, maxHeight);

            // Calcular la posición de la barra en el eje x
            float xPos = i * barWidth;

            // Escalar la barra según la altura calculada
            barPrefab.transform.localScale = new Vector3(barWidth, height, 1f);
        }
    }
}
