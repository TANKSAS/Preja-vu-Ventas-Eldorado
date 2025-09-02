using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VRTemplate;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    [SerializeField]
    VideoTimeScrubControl scrubControl;
    [SerializeField]
    float minTime;
    [SerializeField]
    TMP_Text toolTitleText;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (scrubControl.m_VideoIsPlaying)
        {
            if (scrubControl.m_VideoPlayer.time < minTime && LevelProgressManager.Instance.currentUnit != GameManager.Instance.playerStats.methodologyModule)
            {
                toolTitleText.gameObject.SetActive(true);
                ShowText();
            }
            else
            {
                toolTitleText.gameObject.SetActive(false);
                toolTitleText.text = "";
            }
        }
    }

    private void ShowText()
    {
        toolTitleText.text = scrubControl.m_VideoNameText.text;
        // Aplica el degradado al texto según la duración del tiempo mínimo
        float alpha = Mathf.Clamp01((float)scrubControl.m_VideoPlayer.time / minTime);
        Color startColor = Color.white; // Color inicial del texto
        Color endColor = new Color(1.0f, 1.0f, 1.0f, 0.0f); // Color final del texto (transparente)
        toolTitleText.color = Color.Lerp(endColor, startColor, alpha);
    }
}
