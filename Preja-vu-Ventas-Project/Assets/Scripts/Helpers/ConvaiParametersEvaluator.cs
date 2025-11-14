using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Convai.Scripts.Runtime.Core;
using Convai.Scripts.Runtime.Features;
using Convai.Scripts.Runtime.UI;
using System.Linq;

public abstract class ConvaiParametersEvaluator : MonoBehaviour
{
    public ConvaiNPC currentConvaiNPC;
    public NarrativeState currentState = NarrativeState.Introduction; 
    public string currentIAAnswer;
    
    public bool isTalking;
    public bool isNPCListening;

    public bool isExampleSection;
    public bool isNarrativeSectionConfirmed;
    public bool isApproved;
    public bool isEndingAnalyzeAIResponse;

    [Header("Character IDs por idioma")]
    public string currentCharacterID;
    public string spanishCharacterID;
    public string englishCharacterID;
    public string portugueseCharacterID;

    public void GetNPCLanguage(Language currentLanguage)
    {
        switch (currentLanguage)
        {
            case Language.Español:
                Debug.Log($"{currentConvaiNPC.characterName} usa idioma Español.");
                currentCharacterID = spanishCharacterID;

                break;

            case Language.Ingles:
                Debug.Log($"{currentConvaiNPC.characterName} usa idioma Inglés.");
                currentCharacterID = englishCharacterID;
                break;

            case Language.Portugues:
                Debug.Log($"{currentConvaiNPC.characterName} usa idioma Portugués.");
                currentCharacterID = spanishCharacterID;
                break;

            default:
                Debug.LogWarning("Idioma no reconocido.");
                break;
        }

        if (currentConvaiNPC.characterID != currentCharacterID)
        {
            currentConvaiNPC.characterID = currentCharacterID;
        }
    }

    void OnEnable()
    {
        Debug.Log("Suscrito al evento");
        currentConvaiNPC.OnCharacterTalking += IsNPCTalking;
        ConvaiNPCManager.Instance.OnActiveNPCChanged += SelectedNPC;
    }

    void OnDisable()
    {
        //GameManager.Instance.chatController.iaResponseLines.Clear();
        currentConvaiNPC.OnCharacterTalking -= IsNPCTalking;
        ConvaiNPCManager.Instance.OnActiveNPCChanged -= SelectedNPC;
    }

    public void Listening(bool isListening)
    {
        isNPCListening = isListening;
    }

    public void SendPlayerMessage(string message)
    {
        if (currentConvaiNPC == null)
            return;

        currentConvaiNPC.playerInteractionManager.HandleInputSubmission(message);
    }

    public void CallTrigger(NarrativeDesignTrigger narrativeDesignTrigger)
    {
        if (currentConvaiNPC == null)
            return;

        narrativeDesignTrigger.InvokeSelectedTrigger();
    }

    private void SelectedNPC(ConvaiNPC newActiveNPC)
    {
        if (!newActiveNPC)
            return;

        GameManager.Instance.chatController.currentNPC = this;
    }

    public void NarrativeDesignStateChangeProcess(bool value)
    {
        Debug.Log("Change Narrative");
        GameManager.Instance.chatController.isNarrativeDesingActive = value;
    }

    public void ExampleSectionState(bool state)
    {
        isExampleSection = state;
        Debug.Log("Change Example Section State");
    }

    public void SectionStartConfirmation(bool state)
    {
        isNarrativeSectionConfirmed = state;
        Debug.Log("Start Section" + currentConvaiNPC.narrativeDesignManager._currentSectionID);
    }

    public void IsNPCTalking(bool isNPCTalking)
    {
        isTalking = isNPCTalking;
        Debug.Log("Talking : " + isTalking);
    }

    public void GetNPCResponse(string answer)
    {
        currentIAAnswer = answer;
        GameManager.Instance.chatController.iaResponseLines.Add(currentIAAnswer);
    }


    public abstract void AnalyzeAIResponse();
}
