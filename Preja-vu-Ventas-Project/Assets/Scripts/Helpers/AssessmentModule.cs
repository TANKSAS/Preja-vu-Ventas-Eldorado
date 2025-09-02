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
    protected IEnumerator currentCoroutine;

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
    
    public IEnumerator SendSpeechToText(AssessmentModule assessment,Language language)
    {
        switch (language)
        {
            case Language.Español:
                yield return StartCoroutine(WebRequestController.Instance.SendAudioToElevenLabs(assessment, filePath, apiUrl, apiKey, "es"));
                break;

            case Language.Ingles:
                yield return StartCoroutine(WebRequestController.Instance.SendAudioToElevenLabs(assessment, filePath, apiUrl, apiKey, "en"));
                break;
            
            case Language.Portugues:
                yield return StartCoroutine(WebRequestController.Instance.SendAudioToElevenLabs(assessment, filePath, apiUrl, apiKey, "por"));
                break;
        }
    }

    protected abstract void SetupUI();
    
    public abstract void End();
}
