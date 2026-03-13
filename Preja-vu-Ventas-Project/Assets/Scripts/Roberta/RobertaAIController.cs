using Convai.Scripts.Runtime.Core;
using Convai.Scripts.Runtime.Features;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobertaAIController : ConvaiParametersEvaluator
{
    public NarrativeDesignTrigger narrativeWelcome;
    public NarrativeDesignTrigger narrativeFinalTestFeedBack;
    public NarrativeDesignTrigger narrativeDiagnosisFeedBack;
    public NarrativeDesignTrigger narrativeTankToolBox;
    public NarrativeDesignTrigger narrativeTankToolboxJiujitsuQuiz;

    IEnumerator Welcome()
    {
        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = true;
        yield return new WaitForSeconds(5f);
        ConvaiNPCManager.Instance.isEnabledToShowText = false;
        CallTrigger(narrativeWelcome);

        yield return new WaitUntil(() => isTalking);
        Debug.Log("Inicio aqui la Pregunta de idioma");
        yield return new WaitUntil(() => !isTalking);
        Debug.Log("Inicio Respuesta del usuario");
        ConvaiNPCManager.Instance.isEnabledToShowText = true;

        yield return new WaitUntil(() => isTalking);
        Debug.Log("Saludo");
        yield return new WaitUntil(() => !isTalking);
        yield return new WaitForSeconds(0.5f);

        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = false;
        ConvaiNPCManager.Instance.isEnabledToShowText = false;
    }

    public IEnumerator StartTankToolBox()
    {
        Debug.Log("🔹 Iniciando flujo de asistencia de Roberta (Toolbox).");
        currentState = NarrativeState.Introduction;

        // --- UI Setup ---
        GameManager.Instance.chatController.chatAIBoxUI.gameObject.SetActive(true);
        GameManager.Instance.chatController.chatAIBoxUI._playerCommandPromptPanelObject.SetActive(true);
        GameManager.Instance.chatController.chatAIBoxUI._answerButtonsPanelObject.SetActive(false);
        GameManager.Instance.chatController.chatAIBoxUI._answerButtonsObject.SetActive(false);
        GameManager.Instance.chatController.chatAIBoxUI._loadingPanelObject.SetActive(true);

        CallTrigger(narrativeTankToolBox);
        yield return new WaitUntil(() => GameManager.Instance.chatController.isNarrativeDesingActive);
        ConvaiNPCManager.Instance.isEnabledToSendText = true;
        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = true;
        ConvaiNPCManager.Instance.isEnabledToShowText = true;


        yield return new WaitUntil(() => isTalking);
        GameManager.Instance.chatController.chatAIBoxUI._loadingPanelObject.SetActive(false);
        yield return new WaitUntil(() => !isTalking);

        while (!GameManager.Instance.chatController.userFinished)
        {
            currentState = NarrativeState.WaitingForConfirmation;
            yield return GameManager.Instance.chatController.WaitForUserAudio();

            GameManager.Instance.chatController.isSendingMessage = false;
            GameManager.Instance.chatController.isRecordingMessage = false;

            yield return new WaitUntil(() => isTalking);
            GameManager.Instance.chatController.chatAIBoxUI._loadingPanelObject.SetActive(false);
            yield return new WaitUntil(() => !isTalking);

        }

        yield return StartCoroutine(EndChat());
    }

    public IEnumerator StartDiagnosisFeedBack()
    {
        currentState = NarrativeState.Introduction;
        GameManager.Instance.chatController.retryCount = 0;

        // Preparar UI
        GameManager.Instance.chatController.chatAIBoxUI.gameObject.SetActive(true);
        GameManager.Instance.chatController.chatAIBoxUI._playerCommandPromptPanelObject.SetActive(true);
        GameManager.Instance.chatController.chatAIBoxUI._answerButtonsPanelObject.SetActive(false);
        GameManager.Instance.chatController.chatAIBoxUI._answerButtonsObject.SetActive(false);
        GameManager.Instance.chatController.chatAIBoxUI._loadingPanelObject.SetActive(true);

        // --- 1) INTRODUCCIÓN (trigger)
        CallTrigger(narrativeDiagnosisFeedBack);
        yield return new WaitUntil(() => GameManager.Instance.chatController.isNarrativeDesingActive);

        ConvaiNPCManager.Instance.isEnabledToSendText = true;
        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = true;
        ConvaiNPCManager.Instance.isEnabledToShowText = true;


        // Esperar que Roberta hable o timeout
        yield return StartCoroutine(GameManager.Instance.chatController.WaitForNPCTalkingOrTimeout());
        Debug.Log(0);

        if (GameManager.Instance.chatController.npcTimeoutOccurred)
        {
            Debug.Log(1);
            // mostrar panel retry/cancel y reintentar introducción si el usuario lo pide
            yield return StartCoroutine(HandleTimeoutForNarrative(() =>
            {
                CallTrigger(narrativeFinalTestFeedBack);
            }));

            if (GameManager.Instance.chatController.npcTimeoutOccurred) yield break;
        }

        GameManager.Instance.chatController.chatAIBoxUI._loadingPanelObject.SetActive(false);
        yield return new WaitWhile(() => isTalking);
        Debug.Log(2);


        // 2) ESPERAR CONFIRMACIÓN INICIAL (OK) POR PARTE DEL USUARIO
        currentState = NarrativeState.WaitingForConfirmation;
        yield return StartCoroutine(GameManager.Instance.chatController.WaitForUserConfirmation());
        if (currentState == NarrativeState.Error) yield break;

        // 3) ENVIAR RESPUESTA DEL USUARIO (obtenida de sesiones)
        currentState = NarrativeState.AwaitingUserResponse;

        string newAnswer = "El usuario no responde";
        int currentSessionIndex = 0;
        currentSessionIndex = GameManager.Instance.playerStats.GetLastSessionIndex(KindOfAssessment.Diagnosis);

        if (GameManager.Instance.playerStats.sessions.Count > 0)
        {
            Debug.Log("Si hay datos de sesiones anteriores");
            newAnswer = (!string.IsNullOrEmpty(GameManager.Instance.playerStats.sessions[currentSessionIndex].finalAnswer)
            ? GameManager.Instance.playerStats.sessions[currentSessionIndex].finalAnswer
            : "El usuario no responde");
        }
        else
        {
            Debug.Log("No hay datos de sesiones anteriores");
        }

        cachedUserAnswer = newAnswer;
        SendPlayerMessage(newAnswer);

        // Esperar que IA hable (análisis o ejemplo)
        yield return new WaitUntil(() => isTalking);
        GameManager.Instance.chatController.chatAIBoxUI._loadingPanelObject.SetActive(false);
        yield return new WaitUntil(() => !isTalking);
        yield return new WaitForSeconds(0.5f);

        // --- 3. DETECTAR TIPO DE RESPUESTA ---

        yield return new WaitUntil(() => isNarrativeSectionConfirmed);
        SectionStartConfirmation(false);

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

    public IEnumerator StartFinalTestFeedBack()
    {
        Debug.Log("Roberta: Iniciando StartFinalTestFeedBack");
        currentState = NarrativeState.Introduction;
        GameManager.Instance.chatController.retryCount = 0;

        // Preparar UI
        GameManager.Instance.chatController.chatAIBoxUI.gameObject.SetActive(true);
        GameManager.Instance.chatController.chatAIBoxUI._playerCommandPromptPanelObject.SetActive(true);
        GameManager.Instance.chatController.chatAIBoxUI._answerButtonsPanelObject.SetActive(false);
        GameManager.Instance.chatController.chatAIBoxUI._answerButtonsObject.SetActive(false);
        GameManager.Instance.chatController.chatAIBoxUI._loadingPanelObject.SetActive(true);

        // 1) INTRODUCCIÓN (trigger)
        CallTrigger(narrativeFinalTestFeedBack);
        yield return new WaitUntil(() => GameManager.Instance.chatController.isNarrativeDesingActive);

        ConvaiNPCManager.Instance.isEnabledToSendText = true;
        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = true;
        ConvaiNPCManager.Instance.isEnabledToShowText = true;

        // Esperar que Roberta hable o timeout
        yield return StartCoroutine(GameManager.Instance.chatController.WaitForNPCTalkingOrTimeout());
        Debug.Log(0);

        if (GameManager.Instance.chatController.npcTimeoutOccurred)
        {
            Debug.Log(1);
            // mostrar panel retry/cancel y reintentar introducción si el usuario lo pide
            yield return StartCoroutine(HandleTimeoutForNarrative(() =>
            {
                CallTrigger(narrativeFinalTestFeedBack);
            }));

            if (GameManager.Instance.chatController.npcTimeoutOccurred) yield break;
        }

        GameManager.Instance.chatController.chatAIBoxUI._loadingPanelObject.SetActive(false);
        yield return new WaitWhile(() => isTalking);
        Debug.Log(2);


        // 2) ESPERAR CONFIRMACIÓN INICIAL (OK) POR PARTE DEL USUARIO
        currentState = NarrativeState.WaitingForConfirmation;
        yield return StartCoroutine(GameManager.Instance.chatController.WaitForUserConfirmation());
        if (currentState == NarrativeState.Error) yield break;

        // 3) ENVIAR RESPUESTA DEL USUARIO (obtenida de sesiones)
        currentState = NarrativeState.AwaitingUserResponse;

        string newAnswer = "El usuario no responde";
        int currentSessionIndex = 0;
        currentSessionIndex = GameManager.Instance.playerStats.GetLastSessionIndex(KindOfAssessment.Interview);

        if (GameManager.Instance.playerStats.sessions.Count > 0)
        {
            Debug.Log("Si hay datos de sesiones anteriores");
            newAnswer = (!string.IsNullOrEmpty(GameManager.Instance.playerStats.sessions[currentSessionIndex].finalAnswer)
            ? GameManager.Instance.playerStats.sessions[currentSessionIndex].finalAnswer
            : "El usuario no responde");
        }
        else
        {
            Debug.Log("No hay datos de sesiones anteriores");
        }


        cachedUserAnswer = newAnswer;
        SendPlayerMessage(newAnswer);

        // Esperar que Roberta hable (análisis o ejemplo) o TIMEOUT
        yield return StartCoroutine(GameManager.Instance.chatController.WaitForNPCTalkingOrTimeout());

        if (GameManager.Instance.chatController.npcTimeoutOccurred)
        {
            // Mostrar panel, permitir reintento del envío del nuevoAnswer
            yield return StartCoroutine(HandleTimeoutForNarrative(() =>
            {
                SendPlayerMessage(cachedUserAnswer);
            }));

            if (GameManager.Instance.chatController.npcTimeoutOccurred) yield break;
        }

        // esperar que termine de hablar
        yield return new WaitWhile(() => isTalking);
        yield return new WaitForSeconds(0.25f);

        // 4) Detección de rama (ejemplo o feedback) usando flags de Convai/Narrative
        // Esperamos a que la sección narrativa confirme su rama
        yield return new WaitUntil(() => isNarrativeSectionConfirmed);
        SectionStartConfirmation(false); // reset de la confirmación para próximos pasos

        // Si no es ejemplo -> ruta FEEDBACK en 2 partes
        if (!isExampleSection)
        {
            // Espera OK del usuario para obtener la segunda parte del feedback
            currentState = NarrativeState.AwaitingUserResponse;
            yield return StartCoroutine(GameManager.Instance.chatController.WaitForUserConfirmation());

            if (currentState == NarrativeState.Error) yield break;

            // Enviar OK para que la IA continúe con la parte 2
            SendPlayerMessage("ok");
            ExampleSectionState(false); // reset flag

            // Esperar parte 2 o timeout
            yield return StartCoroutine(GameManager.Instance.chatController.WaitForNPCTalkingOrTimeout());

            if (GameManager.Instance.chatController.npcTimeoutOccurred)
            {
                // permitir reintento de la parte 2
                yield return StartCoroutine(HandleTimeoutForNarrative(() =>
                {
                    SendPlayerMessage("ok");
                }));

                if (GameManager.Instance.chatController.npcTimeoutOccurred) yield break;
            }

            yield return new WaitWhile(() => isTalking);
            yield return new WaitForSeconds(0.25f);
        }

        // 5) ESPERAR OK FINAL PARA CIERRE (aplica para ruta ejemplo y feedback)
        currentState = NarrativeState.AwaitingFinalOk;
        yield return StartCoroutine(GameManager.Instance.chatController.WaitForUserConfirmation());
        if (currentState == NarrativeState.Error) yield break;

        SendPlayerMessage("ok");

        // Esperar cierre o timeout
        yield return StartCoroutine(GameManager.Instance.chatController.WaitForNPCTalkingOrTimeout());
        if (GameManager.Instance.chatController.npcTimeoutOccurred) yield break;

        yield return new WaitWhile(() => isTalking);
        yield return new WaitForSeconds(0.25f);

        // 6) LIMPIEZA FINAL
        yield return StartCoroutine(EndChat());

        Debug.Log("Roberta: Feedback final completado.");
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

            if (linea.StartsWith("resultado final") || linea.StartsWith("final result"))
            {
                Debug.Log($"[Paso 1] Línea detectada: {linea}");

                if (linea.Contains("desaprobado") || linea.Contains("not approved"))
                {
                    isApproved = false;
                    resultadoDetectado = true;
                    Debug.Log("[Paso 2] Desaprobado detectado en misma línea.");
                    break;
                }
                else if (linea.Contains("aprobado") || linea.Contains("approved"))
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

                    if (siguienteLinea.Contains("desaprobado") || linea.Contains("not approved"))
                    {
                        isApproved = false;
                        resultadoDetectado = true;
                        Debug.Log("[Paso 4] Desaprobado detectado en siguiente línea.");
                        break;
                    }
                    else if (siguienteLinea.Contains("aprobado") || linea.Contains("approved"))
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

                if (check == "aprobado" || check == "approved")
                {
                    isApproved = true;
                    Debug.Log("[Paso Extra] Aprobado detectado sin cabecera.");
                    break;
                }
                else if (check == "desaprobado" || check == "not approved")
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
