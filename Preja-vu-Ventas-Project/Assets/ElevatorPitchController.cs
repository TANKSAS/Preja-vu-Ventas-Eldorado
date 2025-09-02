using Convai.Scripts.Runtime.Core;
using System.Collections;
using TMPro;
using UnityEngine;

public class ElevatorPitchController : AssessmentModule
{
    public MauricioAIController mauricio;
    public bool answerReceived;
    public bool hasVideoEnded;

    public bool sendingAnswer;
    float average;

    void Awake()
    {
        GameManager.Instance.elevatorPitchController = this;
    }

    public void SetVideoStatus(bool state)
    {
        hasVideoEnded = state;
    }
    
    public void SetAnswerStatus(bool state)
    {
        answerReceived = state;
    }

    public void StartPitch()
    {
        currentCoroutine = DoElevetorPitch();
        StartCoroutine(currentCoroutine);
    }

    private IEnumerator DoElevetorPitch()
    {
        Debug.Log("Start ElevatorPitch");
        startAssessmentModule = true;
        SceneSettup();

        GameManager.Instance.timeLineController.SetPlayableDirector(11);
        GameManager.Instance.backGroundController.CallChangeVideo(3);
        yield return new WaitUntil(() => !GameManager.Instance.backGroundController.isLoading);

        yield return StartCoroutine(CountDown());
        
        audioRecordingController.audioSource = GameManager.Instance.spectrumVisualizer.audioSource;
        GameManager.Instance.spectrumVisualizer.audioSource.Play();
        
        helperController.IniciarTemporizador();
        GameManager.Instance.backGroundController.currentVideoPlayer.Play();
        GameManager.Instance.timeLineController.Play();
        GameManager.Instance.outputAudioRecorderController.StartRecording();
        GameManager.Instance.outputAudioRecorderController.StartSegmentRecording();

        mauricio.gameObject.SetActive(true);
        //establecer a mauricio como un
        Debug.Log("Esperando");

        yield return new WaitUntil(() => answerReceived);
        
        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = true;
        string segmentPath = GameManager.Instance.outputAudioRecorderController.StopSegmentRecordingAndSave();
        filePath = segmentPath;
        Debug.Log("Enviando Fragmento de respuesta");
        yield return StartCoroutine(SendSpeechToText(this, LanguageManager.Instance.currentLenguaje));
        
        if (GameManager.Instance.elevatorPitchController.finalAnswer != string.Empty)
        {
            yield return StartCoroutine(mauricio.StartGetPlayerResults(finalAnswer));
        }
        else
        {
            yield return StartCoroutine(mauricio.StartGetPlayerResults("El usuario no responde"));
        }


        yield return new WaitUntil(() => hasVideoEnded);
        //envio respuesta a IA
        Debug.Log("Video ya finalizo");

        average = mauricio.average;

        //determinar que video sigue segun el promedio
        DetermineVideoResponse(average);
        yield return new WaitUntil(() => !GameManager.Instance.backGroundController.isLoading);
        GameManager.Instance.backGroundController.currentVideoPlayer.Play();
        GameManager.Instance.timeLineController.Play();

        yield return new WaitUntil(() => !startAssessmentModule);
        End();

        mauricio.isTalking = false;
        mauricio.gameObject.SetActive(false);

        GameManager.Instance.backGroundController.CallChangeImagen(0);
        yield return new WaitUntil(() => !GameManager.Instance.backGroundController.isLoading);

        GameManager.Instance.backGroundController.RestartVideoPlayer(GameManager.Instance.backGroundController.currentVideoPlayer);
        SetAnswerStatus(false);
        hasVideoEnded = false;

        GameManager.Instance.outputAudioRecorderController.StopRecording();
        string newFilePath = GameManager.Instance.outputAudioRecorderController.currentFullPath;
        filePath = newFilePath;
        StartCoroutine(SendSpeechToText(this, LanguageManager.Instance.currentLenguaje));
        GameManager.Instance.CallFinalTestFeedBackQualifier(this);
    }

    public void DetermineVideoResponse(float mediaValue)
    {
        if (mediaValue > 50f)
        {
            Debug.Log("Entra a escenario positivo");
        GameManager.Instance.timeLineController.SetPlayableDirector(12);
            GameManager.Instance.backGroundController.CallChangeVideo(4);

        }
        if (mediaValue < 50f)
        {
            Debug.Log("Entra a escenario negativo");
            GameManager.Instance.timeLineController.SetPlayableDirector(13);
            GameManager.Instance.backGroundController.CallChangeVideo(5);
        }
        if (mediaValue == 50f)
        {
            float newMediaValue = mediaValue + Random.Range(-1f, 1f);
            DetermineVideoResponse(newMediaValue);
        }
    }

    protected override void SetupUI()
    {
        UIManager.Instance.mainMenu.SetActive(false);
        UIManager.Instance.SetCurrentUIMenu(UIManager.Instance.mainMenu);
        UIManager.Instance.ReplaceUIRotation();
        UIManager.Instance.modulePracticalMenu.SetActive(true);
    }

    public override void End()
    {
        GameManager.Instance.spectrumVisualizer.isShowing = false;
        robertaPrefab.SetActive(true);
        UIManager.Instance.modulePracticalMenu.SetActive(false);
        UIManager.Instance.helperPanel.SetActive(false);
        //helperController.ResetearTemporizador();
        UIManager.Instance.modulePracticalMenu.SetActive(false);
    }
}


