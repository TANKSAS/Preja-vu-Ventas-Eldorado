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
    public List<string> iaResponseLines = new List<string>();
    
    public bool isTalking;
    public bool isNPCListening;
    public bool isNarrativeDesingActive;
    public bool isApproved;
    public bool isEndingAnalyzeAIResponse;

    public bool isSendingMessage = false;       // se activa desde el botón
    public bool isRetryingSendMessage = false;  // botón Retry
    public bool isPlayerConfirmed = false;      // botón OK
    public bool hasJustRetried = false;
    
    public int retryCount = 0;
    public int maxRetries = 3;

    void OnEnable()
    {
        Debug.Log("Suscrito al evento");
        currentConvaiNPC.OnCharacterTalking += IsNPCTalking;
        ConvaiNPCManager.Instance.OnActiveNPCChanged += SelectedNPC;
    }

    void OnDisable()
    {
        currentConvaiNPC.OnCharacterTalking -= IsNPCTalking;
        ConvaiNPCManager.Instance.OnActiveNPCChanged -= SelectedNPC;
        iaResponseLines.Clear();
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
        //Debug.Log("Cambio  por +  " + newActiveNPC.characterName);
        //// Accede al ChatUIHandler para buscar el personaje
        //ConvaiChatUIHandler chatHandler = GameManager.Instance._chatUIHandler;

        //if (chatHandler != null)
        //{
        //    // Busca el Character que tiene como GameObject al nuevo NPC
        //    var characterData = chatHandler.characters
        //        .FirstOrDefault(c => c.characterGameObject == newActiveNPC);

        //    if (characterData != null)
        //    {
        //        string npcName = characterData.characterName;
        //        Color npcColor = characterData.CharacterTextColor;

        //        // Envía mensaje vacío para separar bloques
        //        GameManager.Instance.chatAIBoxUI.SendCharacterText(npcName, "", npcColor);
        //    }
        //    else
        //    {
        //        Debug.LogWarning("No se encontró el Character correspondiente al nuevo NPC.");
        //    }
        //}
    }

    public void NarrativeDesignStateChangeProcess(bool value)
    {
        Debug.Log("Change Narrative");
        isNarrativeDesingActive = value;
    }

    public void IsNPCTalking(bool isNPCTalking)
    {
        isTalking = isNPCTalking;
        Debug.Log("Talking : "+ isTalking);
    }

    public void GetNPCResponse(string answer)
    {
        currentIAAnswer = answer;
        iaResponseLines.Add(currentIAAnswer);
    }

    public IEnumerator WaitForUserConfirmation()
{
    GameManager.Instance.chatAIBoxUI._loadingObject.SetActive(false);
    GameManager.Instance.chatAIBoxUI._answerButtonsObject.SetActive(true);

    // Inicializa flags
    isSendingMessage = false;
    isRetryingSendMessage = false;
    isPlayerConfirmed = false;
    hasJustRetried = false; // Se asume que ya lo tienes como campo en la clase

    // Espera a que el usuario presione Retry u OK
    yield return new WaitUntil(() => isSendingMessage);

    // Ciclo principal mientras la narrativa siga activa
    while (isNarrativeDesingActive)
    {
        // Si usuario presionó Retry
        if (isRetryingSendMessage && !hasJustRetried)
        {
            hasJustRetried = true;           // 🔒 Bloquea doble entrada
            yield return null;               // ⏱️ Espera un frame para evitar duplicado

            retryCount++;

            if (retryCount >= maxRetries)
            {
                Debug.LogWarning("Se alcanzó el límite máximo de reintentos.");
                currentState = NarrativeState.Error;
                ScenesManager.Instance.LoadErrorScene();
                yield break;
            }

            Debug.Log("Retry seleccionado. Reproduciendo el mensaje anterior.");

            GameManager.Instance.chatAIBoxUI._answerButtonsObject.SetActive(false);
            GameManager.Instance.chatAIBoxUI._loadingObject.SetActive(true);

            // Espera a que IA comience y termine de hablar nuevamente
            yield return new WaitUntil(() => isTalking);
            GameManager.Instance.chatAIBoxUI._loadingObject.SetActive(false);
            yield return new WaitUntil(() => !isTalking);

            // Reinicia flags antes de volver a esperar
            isSendingMessage = false;
            isRetryingSendMessage = false;
            isPlayerConfirmed = false;
            hasJustRetried = false;

            // Espera un momento antes de permitir otra entrada
            yield return null;

            // Reactiva botones
            GameManager.Instance.chatAIBoxUI._answerButtonsObject.SetActive(true);
            yield return new WaitUntil(() => isSendingMessage || !isNarrativeDesingActive);
        }

        // Si usuario presionó OK
        else if (isPlayerConfirmed)
        {
            Debug.Log("OK seleccionado. Continuando flujo.");

            // Solo reinicia contador si estamos en fase de feedback
            if (currentState == NarrativeState.ProcessingResponse || currentState == NarrativeState.AwaitingFinalOk)
                retryCount = 0;

            break;
        }

        yield return null;
    }

    // Limpieza al salir del bucle
    GameManager.Instance.chatAIBoxUI._answerButtonsObject.SetActive(false);
    GameManager.Instance.chatAIBoxUI._loadingObject.SetActive(true);
}

    public abstract void AnalyzeAIResponse();
}
