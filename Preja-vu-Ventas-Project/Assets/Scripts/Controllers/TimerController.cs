using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    public bool isCounting;
    [SerializeField] bool is360 = false;
    float smoothTimeUpdate;
    float currentTime;
    float time;

    [SerializeField] Image eventTimerImage;
    [SerializeField] TMP_Text eventTimerText;
    [SerializeField] Slider eventTimerSlider;

    void Start()
    {
        isCounting = false;
        if (is360)
        {
            UIManager.Instance.modulePracticalMenu.GetComponent<Canvas>().worldCamera = Camera.main;
        }
    }

    public void EndTimer()
    {
        currentTime = 0;
        
        if (is360)
        {
            eventTimerSlider.maxValue = 1;
            eventTimerSlider.value = 1;
            UIManager.Instance.timer360Panel.SetActive(false);
        }
        else
        {
            eventTimerText.text = string.Empty;
        }

        isCounting = false;
    }

    public void StartTimer(float newTime)
    {
        time = newTime;
        currentTime = time;
        smoothTimeUpdate = currentTime;
        isCounting = true;

        if (is360)
        {
            eventTimerSlider.value = newTime;
            UIManager.Instance.timer360Panel.SetActive(true);
        }
        
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        SetupGUI();

        while (currentTime > 0 && isCounting)
        {
            currentTime--;

            if (eventTimerText != null)
            {
                eventTimerText.text = currentTime.ToString();
            }
            
            yield return new WaitForSeconds(1f);
        }

        EndTimer();
    }

    void Update()
    {
        if (isCounting)
        {
            UpdateTimer();
        }
    }

    void UpdateTimer()
    {
        smoothTimeUpdate -= Time.unscaledDeltaTime;

        if (is360)
        {
            eventTimerSlider.value = smoothTimeUpdate / time;
        }
        else
        {
            eventTimerImage.fillAmount = 1 - (smoothTimeUpdate / time);
        }
    }

    void SetupGUI()
    {
        if (is360)
        {
            eventTimerSlider.value = 1;
        }
        else
        {
            eventTimerImage.fillAmount = 1;
        }
    }
}


