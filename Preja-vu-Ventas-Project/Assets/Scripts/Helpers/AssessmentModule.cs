using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class AssessmentModule : MonoBehaviour
{
    public string finalAnswer;
    public UIHelperController helperController;
    public AudioRecordingDataController audioRecordingController;
    public GameObject robertaPrefab;
    public GameObject robertaObjects;
    public Sound startEffect;

    protected bool startAssessmentModule;
    protected string filePath;

    string apiUrl = "https://api.elevenlabs.io/v1/speech-to-text";
    string apiKey = "sk_79b548fd886efd0ba67a0320f0d9de678172a98f7bf7cc41";

    public bool StartAssessmentModule { get => startAssessmentModule; set => startAssessmentModule = value; }

    public void StopAssementModule()
    {
        startAssessmentModule = false;
        GameManager.Instance.backGroundController.currentVideoPlayer.Stop();
        GameManager.Instance.timeLineController.End();
    }

    protected virtual void SetupAudio()
    {
        GameManager.Instance.spectrumVisualizer.isShowing = true;
        UIManager.Instance.ChanceMusicBackGround(1);
    }

    protected virtual void SceneSettup()
    {
        SetupUI();
        SetupAudio();
        if (robertaPrefab != null) robertaPrefab.SetActive(false);
    }

    protected IEnumerator CountDown()
    {
        int count = 4;

        UIManager.Instance.countDownPanel.SetActive(true);
        SoundManager.Instance.PlayNewSound(startEffect.source);

        while (count > 0)
        {
            Debug.Log("Time" + count);
            UIManager.Instance.countDownPanel.GetComponentInChildren<TMP_Text>().text = count.ToString();
            yield return new WaitForSeconds(0.8f);
            count--;
        }

        UIManager.Instance.countDownPanel.SetActive(false);
        UIManager.Instance.countDownPanel.GetComponentInChildren<TMP_Text>().text = string.Empty;
    }

    public IEnumerator SendSpeechToText()
    {
        switch (LanguageManager.Instance.currentLenguaje)
        {
            case Language.Español:
                yield return StartCoroutine(WebRequestController.Instance.SendAudioToElevenLabs(this, filePath, apiUrl, apiKey, "es"));
                break;

            case Language.Ingles:
                yield return StartCoroutine(WebRequestController.Instance.SendAudioToElevenLabs(this, filePath, apiUrl, apiKey, "en"));
                break;

            case Language.Portugues:
                yield return StartCoroutine(WebRequestController.Instance.SendAudioToElevenLabs(this, filePath, apiUrl, apiKey, "por"));
                break;
        }
    }

    public IEnumerator SaveSessionData()
    {
        //GameManager.Instance.currentKinesthesiaRating = GraphManager.Instance.CalculateKiesthesiaRating(GameManager.Instance.trackingController.handsSafeZonaMovCounter, GameManager.Instance.trackingController.handsDangerMovCounter);
        //GameManager.Instance.currentToneOfVoiceRating = GraphManager.Instance.CalculateVoiceQualification(audioRecordingController.dbData);
        //GameManager.Instance.currentHeatMapRating = GraphManager.Instance.CalculateHeatMapRating(GameManager.Instance.trackingController.eyesSafeZoneCounter, GameManager.Instance.trackingController.eyesDangerZoneCounter);

        GameManager.Instance.trackingController.UpdateSessionData();
       
        SessionData newSession = new SessionData(GameManager.Instance.trackingController.moveHandsCounter, GameManager.Instance.trackingController.handsSafeZonaMovCounter,
            GameManager.Instance.trackingController.handsDangerMovCounter, GameManager.Instance.trackingController.eyesContactCounter, GameManager.Instance.trackingController.eyesSafeZoneCounter,
            GameManager.Instance.trackingController.eyesDangerZoneCounter, GameManager.Instance.backGroundController.currentVideoDuration,
            GameManager.Instance.screenshotController.filePath, GameManager.Instance.elevatorPitchController.finalAnswer, GameManager.Instance.currentToneOfVoiceRating, GameManager.Instance.currentKinesthesiaRating,
            GameManager.Instance.currentHeatMapRating, GameManager.Instance.currentAssessment, new List<float>(audioRecordingController.dbData));

        GameManager.Instance.playerStats.sessions.Add(newSession);
        yield return new WaitUntil(() => !WebRequestController.Instance.InProgress);

        int lastSessionIndex = GameManager.Instance.playerStats.sessions.Count - 1;
        GameManager.Instance.playerStats.sessions[lastSessionIndex].finalAnswer = GameManager.Instance.elevatorPitchController.finalAnswer;
        Debug.Log("Index de Sesion: " + lastSessionIndex);

        BaseDataManager.Instance.Save("/PlayerData.json", GameManager.Instance.playerStats);
        Debug.Log("Sesion Guardada");
    }

    protected abstract void SetupUI();
    
    public abstract void End();
}
