using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine;

public class FinalTestController : AssessmentModule
{
    public bool isDiagnosis;

    void Awake()
    {
        GameManager.Instance.finalTestController = this;
    }

    public void Update()
    {
        if (StartAssessmentModule)
        {
            Keyboard keyboard = Keyboard.current;

            // Verifica si la tecla "Enter" está siendo presionada
            if (keyboard.enterKey.wasPressedThisFrame)
            {
                Debug.Log("Key Saved " + GameManager.Instance.backGroundController.currentVideoPlayer.time);
                //Debug.Log("Key Saved. " + GameManager.Instance.timeLineController.playableDirectors[0].time);

                // Puedes realizar acciones adicionales cuando se presiona Enter.
            }
        }
    }

    public IEnumerator DoTestFirstTime()
    {
        Debug.Log("Start Interview Diagnosis");
        startAssessmentModule = true;
        SceneSettup();

        GameManager.Instance.backGroundController.CallChangeVideo(3);
        GameManager.Instance.timeLineController.SetPlayableDirector(1);
        yield return new WaitUntil(() => !GameManager.Instance.backGroundController.isLoading);

        yield return StartCoroutine(CountDown());

        float newTime = (float)GameManager.Instance.timeLineController.currentPlayableDirector.duration;
        audioRecordingController.StartRecordingData(newTime);
        audioRecordingController.audioSource = GameManager.Instance.spectrumVisualizer.audioSource;
        GameManager.Instance.spectrumVisualizer.audioSource.Play();
        GameManager.Instance.outputAudioRecorderController.StartRecording();
        GameManager.Instance.trackingController.StartTrainingSession(newTime);

        GameManager.Instance.backGroundController.currentVideoPlayer.Play();
        yield return new WaitUntil(() => GameManager.Instance.backGroundController.currentVideoPlayer.isPlaying);
        GameManager.Instance.timeLineController.Play();

        yield return new WaitUntil(() => !startAssessmentModule);
        End();

        Debug.Log("End Entrevista");
        GameManager.Instance.backGroundController.CallChangeImagen(0);
        yield return new WaitUntil(() => !GameManager.Instance.backGroundController.isLoading);

        string newFilePath = GameManager.Instance.outputAudioRecorderController.currentFullPath;
        filePath = newFilePath;

        StartCoroutine(SendSpeechToText());
        StartCoroutine(SaveSessionData());
    }

    public IEnumerator DoTest()
    {
        Debug.Log("Start Interview FinalTest");

        //  Marca que la prueba está en curso
        startAssessmentModule = true;

        //  Configura la escena inicial (UI, entorno, etc.)
        SceneSettup();

        //  Carga el video y timeline específicos para prueba final
        GameManager.Instance.backGroundController.CallChangeVideo(2);
        GameManager.Instance.timeLineController.SetPlayableDirector(0);

        // Espera a que el fondo termine de cargar
        yield return new WaitUntil(() => !GameManager.Instance.backGroundController.isLoading);

        //  Muestra cuenta regresiva visual antes de iniciar
        yield return StartCoroutine(CountDown());

        //  Inicia grabación de audio y tracking
        float newTime = (float)GameManager.Instance.timeLineController.currentPlayableDirector.duration;
        audioRecordingController.StartRecordingData(newTime);
        audioRecordingController.audioSource = GameManager.Instance.spectrumVisualizer.audioSource;
        GameManager.Instance.spectrumVisualizer.audioSource.Play();
        GameManager.Instance.trackingController.StartTrainingSession(newTime);
        GameManager.Instance.trackingController.EnableHandsDetecterMeshRenderers();
        GameManager.Instance.outputAudioRecorderController.StartRecording();

        //  Reproduce el video y el timeline
        GameManager.Instance.backGroundController.currentVideoPlayer.Play();
        yield return new WaitUntil(() => GameManager.Instance.backGroundController.currentVideoPlayer.isPlaying);
        GameManager.Instance.timeLineController.Play();

        //  Espera a que termine la entrevista
        yield return new WaitUntil(() => !startAssessmentModule);

        //  Finaliza la prueba y limpia estados
        End();
        Debug.Log("End Entrevista");

        GameManager.Instance.backGroundController.CallChangeImagen(0);
        yield return new WaitUntil(() => !GameManager.Instance.backGroundController.isLoading);

        string newFilePath = GameManager.Instance.outputAudioRecorderController.currentFullPath;
        filePath = newFilePath;

        StartCoroutine(SendSpeechToText());
        StartCoroutine(SaveSessionData());
    }

    protected override void SetupUI()
    {
        if (GameManager.Instance.isDiagnosis)
        {
            UIManager.Instance.practicalModuleCaseDetailPanel.SetActive(false);
            UIManager.Instance.SetCurrentUIMenu(UIManager.Instance.practicalModuleCaseDetailPanel);
            UIManager.Instance.ReplaceUIRotation();

        }
        else
        {
            UIManager.Instance.diagnosisInstructionsDetailPanel.SetActive(false);
            UIManager.Instance.SetCurrentUIMenu(UIManager.Instance.diagnosisInstructionsDetailPanel);
            UIManager.Instance.ReplaceUIRotation();
        }

        UIManager.Instance.modulePracticalMenu.SetActive(true);
    }

    public override void End()
    {
        GameManager.Instance.spectrumVisualizer.isShowing = false;
        GameManager.Instance.outputAudioRecorderController.StopRecording();
        UIManager.Instance.modulePracticalMenu.SetActive(false);
    }
}
