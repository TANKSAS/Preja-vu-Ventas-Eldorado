using Convai.Scripts.Runtime.Core;
using Convai.Scripts.Runtime.Features;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobertaAIController : ConvaiParametersEvaluator
{
    public NarrativeDesignTrigger narrativeFinalTestFeedBack;
    public NarrativeDesignTrigger narrativeTankToolBox;
    public NarrativeDesignTrigger narrativeTankToolboxJiujitsuQuiz;

    public IEnumerator StartTankToolBox()
    {
        Debug.Log("🔹 Iniciando flujo de asistencia de Roberta (Toolbox).");
        currentState = NarrativeState.Introduction;

        // --- UI Setup ---
        GameManager.Instance.chatController.chatAIBoxUI.gameObject.SetActive(true);
        GameManager.Instance.chatController.chatAIBoxUI._playerCommandPromptPanelObject.SetActive(true);
        GameManager.Instance.chatController.chatAIBoxUI._answerButtonsHolerObject.SetActive(false);
        GameManager.Instance.chatController.chatAIBoxUI._answerButtonsObject.SetActive(false);
        GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(true);

        CallTrigger(narrativeTankToolBox);
        yield return new WaitUntil(() => GameManager.Instance.chatController.isNarrativeDesingActive);
        ConvaiNPCManager.Instance.isEnabledToSendText = true;
        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = true;
        ConvaiNPCManager.Instance.isEnabledToShowText = true;


        yield return new WaitUntil(() => isTalking);
        GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(false);
        yield return new WaitUntil(() => !isTalking);

        while (!GameManager.Instance.chatController.userFinished)
        {
            currentState = NarrativeState.WaitingForConfirmation;
            yield return GameManager.Instance.chatController.WaitForUserAudio();

            GameManager.Instance.chatController.isSendingMessage = false;
            GameManager.Instance.chatController.isRecordingMessage = false;

            yield return new WaitUntil(() => isTalking);
            GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(false);
            yield return new WaitUntil(() => !isTalking);

        }

        yield return StartCoroutine(EndChat());
    }

    public IEnumerator StartFinalTestFeedBack()
    {
        currentState = NarrativeState.Introduction;
        GameManager.Instance.chatController.retryCount = 0;

        // --- UI Setup ---
        GameManager.Instance.chatController.chatAIBoxUI.gameObject.SetActive(true);
        GameManager.Instance.chatController.chatAIBoxUI._playerCommandPromptPanelObject.SetActive(true);
        GameManager.Instance.chatController.chatAIBoxUI._answerButtonsHolerObject.SetActive(false);
        GameManager.Instance.chatController.chatAIBoxUI._answerButtonsObject.SetActive(false);
        GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(true);

        // --- 1. INTRODUCCIÓN ---
        CallTrigger(narrativeFinalTestFeedBack);
        yield return new WaitUntil(() => GameManager.Instance.chatController.isNarrativeDesingActive);

        ConvaiNPCManager.Instance.isEnabledToSendText = true;
        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = true;
        ConvaiNPCManager.Instance.isEnabledToShowText = true;

        yield return new WaitUntil(() => isTalking);
        GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(false);
        yield return new WaitUntil(() => !isTalking);

        // Esperar OK del usuario para continuar
        currentState = NarrativeState.WaitingForConfirmation;
        yield return GameManager.Instance.chatController.WaitForUserConfirmation();
        if (currentState == NarrativeState.Error) yield break;

        // --- 2. ENVÍO DE RESPUESTA DEL USUARIO ---
        currentState = NarrativeState.AwaitingUserResponse;
        string userAnswer = GameManager.Instance.playerStats.sessions[
            GameManager.Instance.playerStats.lastSessionIndex
        ].finalAnswer;

        if (string.IsNullOrEmpty(userAnswer))
            userAnswer = "El usuario no responde.";

        SendPlayerMessage(userAnswer);

        // Esperar que IA hable (análisis o ejemplo)
        yield return new WaitUntil(() => isTalking);
        GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(false);
        yield return new WaitUntil(() => !isTalking);
        yield return new WaitForSeconds(0.5f);

        // --- 3. DETECTAR TIPO DE RESPUESTA ---

        yield return new WaitUntil(() => isNarrativeSectionConfirmed);
        SectionStartConfirmation(false);

        if (!isExampleSection)
        {
            // === RUTA A: Feedback en dos partes ===
            // 🟣 Parte 1 completada → esperar OK
            currentState = NarrativeState.AwaitingUserResponse;
            yield return GameManager.Instance.chatController.WaitForUserConfirmation();

            if (currentState == NarrativeState.Error) yield break;

            // 🟣 Enviar OK → Parte 2
            SendPlayerMessage("ok");
            ExampleSectionState(false);

            yield return new WaitUntil(() => isTalking);
            yield return new WaitUntil(() => !isTalking);
            yield return new WaitForSeconds(0.5f);
        }

        // --- 4. ESPERAR OK FINAL PARA CIERRE ---
        currentState = NarrativeState.AwaitingFinalOk;
        yield return GameManager.Instance.chatController.WaitForUserConfirmation();
        if (currentState == NarrativeState.Error) yield break;

        SendPlayerMessage("ok");

        // --- 5. MENSAJE DE CIERRE ---
        currentState = NarrativeState.Closing;
        yield return new WaitUntil(() => isTalking);
        yield return new WaitUntil(() => !isTalking);

        // --- 6. LIMPIEZA FINAL ---
        yield return StartCoroutine(EndChat());

        Debug.Log("✅ Feedback final completado correctamente.");
    }

    IEnumerator EndChat()
    {
        GameManager.Instance.chatController.chatAIBoxUI.ClearUI();
        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = false;
        ConvaiNPCManager.Instance.isEnabledToShowText = false;
        ConvaiNPCManager.Instance.isEnabledToSendText = false;
        GameManager.Instance.chatController.userFinished = false;

        yield return new WaitForSeconds(10f);
        ConvaiNPCManager.Instance.SetActiveConvaiNPC(null);
        GameManager.Instance.chatController.chatAIBoxUI.gameObject.SetActive(false);
    }

    public override void AnalyzeAIResponse()
    {
        if (GameManager.Instance.chatController.iaResponseLines == null || GameManager.Instance.chatController.iaResponseLines.Count == 0)
            return;

        isApproved = false; // Valor por defecto

        List<string> lineasExpandida = new List<string>();

        foreach (string linea in GameManager.Instance.chatController.iaResponseLines)
        {
            string[] subLineas = linea.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            lineasExpandida.AddRange(subLineas);
        }

        bool resultadoDetectado = false;

        for (int i = 0; i < lineasExpandida.Count; i++)
        {
            string linea = lineasExpandida[i].Trim().ToLower();

            if (linea.StartsWith("resultado final"))
            {
                Debug.Log($"[Paso 1] Línea detectada: {linea}");

                if (linea.Contains("desaprobado"))
                {
                    isApproved = false;
                    resultadoDetectado = true;
                    Debug.Log("[Paso 2] Desaprobado detectado en misma línea.");
                    break;
                }
                else if (linea.Contains("aprobado"))
                {
                    isApproved = true;
                    resultadoDetectado = true;
                    Debug.Log("[Paso 2] Aprobado detectado en misma línea.");
                    break;
                }

                if (i + 1 < lineasExpandida.Count)
                {
                    string siguienteLinea = lineasExpandida[i + 1].Trim().ToLower();
                    Debug.Log("[Paso 3] Siguiente línea: " + siguienteLinea);

                    if (siguienteLinea.Contains("desaprobado"))
                    {
                        isApproved = false;
                        resultadoDetectado = true;
                        Debug.Log("[Paso 4] Desaprobado detectado en siguiente línea.");
                        break;
                    }
                    else if (siguienteLinea.Contains("aprobado"))
                    {
                        isApproved = true;
                        resultadoDetectado = true;
                        Debug.Log("[Paso 4] Aprobado detectado en siguiente línea.");
                        break;
                    }
                }
            }
        }

        // ✅ Si no se detectó a través de "resultado final", buscar en todo el texto
        if (!resultadoDetectado)
        {
            foreach (string linea in lineasExpandida)
            {
                string check = linea.Trim().ToLower();

                if (check == "aprobado")
                {
                    isApproved = true;
                    Debug.Log("[Paso Extra] Aprobado detectado sin cabecera.");
                    break;
                }
                else if (check == "desaprobado")
                {
                    isApproved = false;
                    Debug.Log("[Paso Extra] Desaprobado detectado sin cabecera.");
                    break;
                }
            }
        }

        Debug.Log("[Paso Final] Resultado final (bool): " + isApproved);

        RobertaController.Instance.robertaAI.isEndingAnalyzeAIResponse = false;
    }
}
