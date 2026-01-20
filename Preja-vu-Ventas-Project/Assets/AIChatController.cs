using Convai.Scripts.Runtime.Core;
using Convai.Scripts.Runtime.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChatController : MonoBehaviour
{
    public ChatBoxUI chatAIBoxUI;
    public ConvaiParametersEvaluator currentNPC;
    public List<string> iaResponseLines = new List<string>();

    public bool isNarrativeDesingActive = false;
    public bool isSendingMessage = false;       // se activa desde el botón
    public bool isRetryingSendMessage = false;  // botón Retry
    public bool isPlayerConfirmed = false;      // botón OK
    public bool hasJustRetried = false;

    public bool userFinished = false;
    public bool npcTimeoutOccurred = false;
    public bool userChoseRetry = false;
    public bool userChoseCancel = false;

    public float npcResponseTimeout = 8f;  // segundos
    public int retryCount = 0;
    public int maxRetries = 3;
    public bool isRecordingMessage;


    public void InitializeChatUI()
    {
        chatAIBoxUI = GameObject.FindGameObjectWithTag("ChatAIBoxUI").GetComponent<ChatBoxUI>();
        chatAIBoxUI.gameObject.SetActive(false);
    }

    public void ForceCloseChatAfterCancel()
    {
        Debug.Log("🚪 Cerrando chat tras CANCEL");

        // Reset flags
        userChoseRetry = false;
        userChoseCancel = false;
        npcTimeoutOccurred = false;
        isSendingMessage = false;
        isRetryingSendMessage = false;
        isPlayerConfirmed = false;

        // Limpiar UI
        chatAIBoxUI.ClearUI();
        chatAIBoxUI.gameObject.SetActive(false);

        // Desactivar Convai
        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = false;
        ConvaiNPCManager.Instance.isEnabledToSendText = false;
        ConvaiNPCManager.Instance.isEnabledToShowText = false;
        ConvaiNPCManager.Instance.SetActiveConvaiNPC(null);
    }

    public IEnumerator EndChat()
    {
        isNarrativeDesingActive = false;
        isSendingMessage = false;
        isRetryingSendMessage = false;
        isPlayerConfirmed = false;
        hasJustRetried = false;
        userFinished = false; 

        GameManager.Instance.chatController.chatAIBoxUI.ClearUI();
        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = false;
        ConvaiNPCManager.Instance.isEnabledToShowText = false;
        ConvaiNPCManager.Instance.isEnabledToSendText = false;
        GameManager.Instance.chatController.userFinished = false;

        yield return new WaitForSeconds(5f);
        ConvaiNPCManager.Instance.SetActiveConvaiNPC(null);
        GameManager.Instance.chatController.chatAIBoxUI.gameObject.SetActive(false);
    }

    public IEnumerator WaitForNPCTalkingOrTimeout()
    {
        npcTimeoutOccurred = false;

        float timer = 0f;

        // Esperar a que empiece a hablar o se acabe el tiempo
        while (!currentNPC.isTalking && timer < npcResponseTimeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Si no habló, se generó timeout
        if (!currentNPC.isTalking)
        {
            npcTimeoutOccurred = true;
            Debug.Log("❌ NPC no respondió — Timeout");
        }
        else
        {
            Debug.Log("NPC respondió — Timeout" + timer);
        }
    }

    public IEnumerator WaitForUserErrorDecision()
    {
        Debug.Log("Entro a la confirmacion");

        chatAIBoxUI._playerCommandPromptPanelObject.SetActive(true);
        chatAIBoxUI._answerButtonsHolerObject.SetActive(true);
        chatAIBoxUI._errorPanel.SetActive(true);

        userChoseRetry = false;
        userChoseCancel = false;

        yield return new WaitUntil(() => userChoseRetry || userChoseCancel);

        Debug.Log("Envio la confirmacion");
        chatAIBoxUI._playerCommandPromptPanelObject.SetActive(false);
        chatAIBoxUI._answerButtonsHolerObject.SetActive(false);
        chatAIBoxUI._errorPanel.SetActive(false);
    }

    public IEnumerator WaitForUserAudio()
    {
        chatAIBoxUI._answerButtonsHolerObject.SetActive(true);
        chatAIBoxUI._answerButtonsRecordingObject.SetActive(true);
        
        isSendingMessage = false;
        isRetryingSendMessage = false;
        isPlayerConfirmed = false;
        hasJustRetried = false; // Se asume que ya lo tienes como campo en la clase
        
        // Espera a que el usuario presione Audio u End
        yield return new WaitUntil(() => isSendingMessage);
        chatAIBoxUI._answerButtonsHolerObject.SetActive(false);
        chatAIBoxUI._answerButtonsRecordingObject.SetActive(false);
        chatAIBoxUI._loadingObject.SetActive(true);
    }

    public IEnumerator WaitForUserConfirmation()
    {
        chatAIBoxUI._loadingObject.SetActive(false);
        chatAIBoxUI._answerButtonsHolerObject.SetActive(true);
        chatAIBoxUI._answerButtonsObject.SetActive(true);

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
                    currentNPC.currentState = NarrativeState.Error;
                    ScenesManager.Instance.LoadErrorScene();
                    yield break;
                }

                Debug.Log("Retry seleccionado. Reproduciendo el mensaje anterior.");

                chatAIBoxUI._answerButtonsHolerObject.SetActive(false);
                chatAIBoxUI._answerButtonsObject.SetActive(false);
                chatAIBoxUI._loadingObject.SetActive(true);

                // Espera a que IA comience y termine de hablar nuevamente
                yield return new WaitUntil(() => currentNPC.isTalking);
                chatAIBoxUI._loadingObject.SetActive(false);
                yield return new WaitUntil(() => !currentNPC.isTalking);

                // Reinicia flags antes de volver a esperar
                isSendingMessage = false;
                isRetryingSendMessage = false;
                isPlayerConfirmed = false;
                hasJustRetried = false;

                // Espera un momento antes de permitir otra entrada
                yield return null;

                // Reactiva botones
                chatAIBoxUI._answerButtonsHolerObject.SetActive(true);
                chatAIBoxUI._answerButtonsObject.SetActive(true);
                yield return new WaitUntil(() => isSendingMessage || !isNarrativeDesingActive);
            }

            // Si usuario presionó OK
            else if (isPlayerConfirmed)
            {
                Debug.Log("OK seleccionado. Continuando flujo.");

                // Solo reinicia contador si estamos en fase de feedback
                if (currentNPC.currentState == NarrativeState.ProcessingResponse || currentNPC.currentState == NarrativeState.AwaitingFinalOk)
                    retryCount = 0;

                break;
            }

            yield return null;
        }

        // Limpieza al salir del bucle
        chatAIBoxUI._answerButtonsHolerObject.SetActive(false);
        chatAIBoxUI._answerButtonsObject.SetActive(false);
        chatAIBoxUI._loadingObject.SetActive(true);
    }
}
