using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ProductReference : MonoBehaviour
{
    public Image uiProductSprite;
    public TMP_Text uiProductNameText;
    public TMP_Text uiProductDescritionText;
    public GameObject productPanel;
    public GameObject gazeActivator;


    public void ShowUI()
    {
        if (!gameObject.activeInHierarchy) return;

        gazeActivator.SetActive(false);
        productPanel.SetActive(true);   
    }

    public void HideUI()
    {
        gazeActivator.SetActive(true);
        productPanel.SetActive(false);
    }
}

