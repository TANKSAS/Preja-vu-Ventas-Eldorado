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

    public void StartFinalTest(bool isDiagnosisTest)
    {
        isDiagnosis = isDiagnosisTest;

        if (isDiagnosis)
        {
            currentCoroutine = DoTestFirstTime();
            Debug.Log("Diagnosis");
        }
        else
        {
            currentCoroutine = DoTest();
            Debug.Log("Final Test");
        }
    
        StartCoroutine(currentCoroutine);
    }

    IEnumerator DoTestFirstTime()
    {
        Debug.Log("Start Final Practice");
        startAssessmentModule = true;
        SceneSettup();

        GameManager.Instance.timeLineController.SetPlayableDirector(0);
        GameManager.Instance.backGroundController.CallChangeVideo(6);
        yield return new WaitUntil(() => !GameManager.Instance.backGroundController.isLoading);

        yield return StartCoroutine(CountDown());

        float newTime = (float)GameManager.Instance.timeLineController.currentPlayableDirector.duration;
        audioRecordingController.StartRecordingData(newTime);
        UIManager.Instance.helperPanel.SetActive(true);

        audioRecordingController.audioSource = GameManager.Instance.spectrumVisualizer.audioSource;
        GameManager.Instance.spectrumVisualizer.audioSource.Play();
        GameManager.Instance.backGroundController.currentVideoPlayer.Play();
        GameManager.Instance.timeLineController.Play();
        helperController.IniciarTemporizador();
        GameManager.Instance.outputAudioRecorderController.StartRecording();

        //GameManager.Instance.trackingController.StartTrainingSession(newTime);
        //GameManager.Instance.trackingController.EnableHandsDetecterMeshRenderers();

        yield return new WaitUntil(() => !startAssessmentModule);

        End();
        Debug.Log("End final Practice");

        string newFilePath = GameManager.Instance.outputAudioRecorderController.currentFullPath;
        filePath = newFilePath;

        yield return StartCoroutine(SendSpeechToText(this, LanguageManager.Instance.currentLenguaje));

        //yield return new WaitUntil(() => !GameManager.Instance.trackingController.isGettingData);
        //SaveSessionData();

        GameManager.Instance.backGroundController.CallChangeImagen(0);
        yield return new WaitUntil(() => !GameManager.Instance.backGroundController.isLoading);

        //UIManager.Instance.CallShowGraph();
        //yield return new WaitUntil(() => !UIManager.Instance.practicalResultsGraphicMenu.activeInHierarchy);

        GameManager.Instance.backGroundController.RestartVideoPlayer(GameManager.Instance.backGroundController.currentVideoPlayer);
        GameManager.Instance.CallFinalTestFeedBackQualifier(this);
    }

    IEnumerator DoTest()
    {
        Debug.Log("Start Final Practice");
        startAssessmentModule = true;
        SceneSettup();

        GameManager.Instance.timeLineController.SetPlayableDirector(0);
        GameManager.Instance.backGroundController.CallChangeVideo(6);
        yield return new WaitUntil(()=> !GameManager.Instance.backGroundController.isLoading);

        yield return StartCoroutine(CountDown());
        
        float newTime = (float)GameManager.Instance.timeLineController.currentPlayableDirector.duration;
        audioRecordingController.StartRecordingData(newTime);
        UIManager.Instance.helperPanel.SetActive(true);
        
        audioRecordingController.audioSource = GameManager.Instance.spectrumVisualizer.audioSource;
        GameManager.Instance.spectrumVisualizer.audioSource.Play();
        GameManager.Instance.backGroundController.currentVideoPlayer.Play();
        GameManager.Instance.timeLineController.Play();
        
        yield return new WaitUntil(() => GameManager.Instance.backGroundController.currentVideoPlayer.isPlaying);
        yield return new WaitUntil(() => !GameManager.Instance.backGroundController.currentVideoPlayer.isPlaying);

        helperController.IniciarTemporizador();
        GameManager.Instance.outputAudioRecorderController.StartRecording();

        //GameManager.Instance.trackingController.StartTrainingSession(newTime);
        //GameManager.Instance.trackingController.EnableHandsDetecterMeshRenderers();

        yield return new WaitUntil(() => !startAssessmentModule);
        
        End();
        Debug.Log("End final Practice");
        
        StartCoroutine(SendSpeechToText(this, LanguageManager.Instance.currentLenguaje));
        

        //yield return new WaitUntil(() => !GameManager.Instance.trackingController.isGettingData);
        //SaveSessionData();

        GameManager.Instance.backGroundController.CallChangeImagen(0);
        yield return new WaitUntil(() => !GameManager.Instance.backGroundController.isLoading);

        //UIManager.Instance.CallShowGraph();
        //yield return new WaitUntil(() => !UIManager.Instance.practicalResultsGraphicMenu.activeInHierarchy);
        
        GameManager.Instance.backGroundController.RestartVideoPlayer(GameManager.Instance.backGroundController.currentVideoPlayer);
        UIManager.Instance.SetCurrentUIMenu(UIManager.Instance.practicalResultsGraphicMenu);
        GameManager.Instance.CallFinalTestFeedBackQualifier(this);
    }

    void SaveSessionData()
    {
        GameManager.Instance.trackingController.UpdateSessionData();
        GameManager.Instance.playerStats.lastSessionIndex = GameManager.Instance.playerStats.sessions.Count - 1;
        GameManager.Instance.playerStats.sessions[GameManager.Instance.playerStats.lastSessionIndex].readingResultsvoices = new List<float>(audioRecordingController.dbData);
        GameManager.Instance.playerStats.sessions[GameManager.Instance.playerStats.lastSessionIndex].kindOfVoice = GraphManager.Instance.currentToneOfVoice;
        BaseDataManager.Instance.Save("/PlayerSalesData.json", GameManager.Instance.playerStats);
        Debug.Log("Sesion Guardada");
    }

    protected override void SetupUI()
    {
        if (isDiagnosis)
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
        robertaPrefab.SetActive(true);
        UIManager.Instance.modulePracticalMenu.SetActive(false);
        UIManager.Instance.helperPanel.SetActive(false);
        helperController.ResetearTemporizador();
    }
}
