using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spectrum : MonoBehaviour
{
    public bool isShowing;

    [Range(0.0000001f, 0.1f)]
    public float refValue = 0.00005f;

    [Range(0.1f, 5f)]
    public float scaleMultiplier = 1f;

    [Range(0.01f, 1f)]
    public float releaseTime = 0.2f;

    public AudioSource audioSource;
    public AudioClip microphoneClip;
    public SpectrumElement spectrumElementPrefab;
    public GameObject spectrumHolder;

    private SpectrumElement[] spectrumElements = new SpectrumElement[92];
    private float[] spectrum = new float[2048];

    private void Start()
    {
        StartMicrophone();
    }
    
    void StartMicrophone()
    {
        string microphoneName = Microphone.devices[0];
        Debug.Log(microphoneName);
        microphoneClip = Microphone.Start(microphoneName, true, 1, AudioSettings.outputSampleRate);
        audioSource.clip = microphoneClip;
    }

    private void Update()
    {
        if (!isShowing)
            return;

        if (spectrumElements[0] == null)
        {
            CreateElements();
        }


        if (microphoneClip == null)
        {
            audioSource.clip = microphoneClip;
        }

        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        for (int i = 0; i < spectrumElements.Length; i++)
        {
            var value = 20f * Mathf.Log10(spectrum[i + 2] / refValue);
            spectrumElements[i].SetScale(value);
        }

        Keyboard keyboard = Keyboard.current;

        // Verifica si la tecla "Enter" está siendo presionada
        if (keyboard.cKey.wasPressedThisFrame)
        {
            AlignElementsCircular();
        }

        if (keyboard.hKey.wasPressedThisFrame)
        {
            AlignElementsHorizontal();
        }
    }

    private void CreateElements()
    {
        spectrumHolder = GameManager.Instance.finalTestController.audioRecordingController.spectrumElementsHolder;
        for (int i = 0; i < spectrumElements.Length; i++)
        {
            spectrumElements[i] = Instantiate(spectrumElementPrefab, spectrumHolder.transform, false);
        }

        AlignElementsHorizontal();
        //AlignElementsCircular();
    }

    private void AlignElementsHorizontal()
    {
        var shiftPerElement = 1.5f;
        var leftShift = shiftPerElement * spectrumElements.Length / 2f;

        for (int i = 0; i < spectrumElements.Length; i++)
        {
            var element = spectrumElements[i];
            element.transform.localRotation = Quaternion.identity;
            element.transform.localPosition = Vector3.right * (i * shiftPerElement - leftShift);
        }
    }

    private void AlignElementsCircular()
    {
        var radius = 20f;
        var angleStep = 360f / spectrumElements.Length;

        int angleMultiplier = 0;

        for (int i = 0; i < spectrumElements.Length; i += 2)
        {
            spectrumElements[i].transform.localRotation = Quaternion.identity;
            spectrumElements[i + 1].transform.localRotation = Quaternion.identity;

            //Left
            spectrumElements[i].transform.Rotate(Vector3.forward, angleMultiplier * -angleStep);
            spectrumElements[i].transform.localPosition = spectrumElements[i].transform.up * radius;

            //Right
            spectrumElements[i + 1].transform.Rotate(Vector3.forward, (angleMultiplier + 1) * angleStep);
            spectrumElements[i + 1].transform.localPosition = spectrumElements[i + 1].transform.up * radius;

            angleMultiplier++;
        }
    }
}
