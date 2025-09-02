using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.VRTemplate;
using System.Collections;

/// <summary>
/// Controls the steps in the in coaching card.
/// </summary>
public class StepManager : MonoBehaviour
{
    public bool isDiagnosisTest;

    [Serializable]
    class Step
    {
        [SerializeField]
        public GameObject stepObject;

        [SerializeField]
        public string buttonText;
    }
    

    [SerializeField]
    TextMeshProUGUI m_StepButtonTextField;

    [SerializeField]
    List<Step> m_StepList = new List<Step>();

    public int m_CurrentStepIndex = 0;
    public void Restart()
    {
        m_CurrentStepIndex = 0;
    }

    public void StartPreviousStep(GameObject stepPanel)
    {
        if (m_CurrentStepIndex == 0)
        {
            stepPanel.SetActive(false);
            UIManager.Instance.SetCurrentUIMenu(stepPanel);
            UIManager.Instance.ReplaceUIRotation();
            UIManager.Instance.mainMenu.SetActive(true);   
        }
        else
        {
            m_StepList[m_CurrentStepIndex].stepObject.SetActive(false);
            m_CurrentStepIndex = (m_CurrentStepIndex - 1) % m_StepList.Count;
            m_StepList[m_CurrentStepIndex].stepObject.SetActive(true);
        }
        
        m_StepButtonTextField.text = LanguageManager.Instance.GetStringValue(m_StepList[m_CurrentStepIndex].buttonText);
    }


    public void StartNextStep(GameObject stepPanel)
    {
        if (m_CurrentStepIndex == m_StepList.Count - 1)
        {
            stepPanel.SetActive(false);
            UIManager.Instance.SetCurrentUIMenu(stepPanel);
            UIManager.Instance.ReplaceUIRotation();
            GameManager.Instance.elevatorPitchController.StartPitch();
        }
        
        m_StepList[m_CurrentStepIndex].stepObject.SetActive(false);
        m_CurrentStepIndex = (m_CurrentStepIndex + 1) % m_StepList.Count;
        m_StepList[m_CurrentStepIndex].stepObject.SetActive(true);
        m_StepButtonTextField.text = LanguageManager.Instance.GetStringValue(m_StepList[m_CurrentStepIndex].buttonText);
    }
}
