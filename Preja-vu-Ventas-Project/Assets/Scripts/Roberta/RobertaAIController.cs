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

    // --- START: ToolBox (chat libre con grabación/audio) ---
    public IEnumerator StartTankToolBox()
    {
        Debug.Log("Roberta: Iniciando TankToolBox (asistente)");

        currentState = NarrativeState.Introduction;
        GameManager.Instance.chatController.retryCount = 0;

        // Preparar UI
        GameManager.Instance.chatController.chatAIBoxUI.gameObject.SetActive(true);
        GameManager.Instance.chatController.chatAIBoxUI._playerCommandPromptPanelObject.SetActive(true);
        GameManager.Instance.chatController.chatAIBoxUI._answerButtonsHolerObject.SetActive(false);
        GameManager.Instance.chatController.chatAIBoxUI._answerButtonsObject.SetActive(false);
        GameManager.Instance.chatController.chatAIBoxUI._answerButtonsRecordingObject.SetActive(false);
        GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(true);
        Debug.Log(0);

        // Lanza la introducción narrativa
        CallTrigger(narrativeTankToolBox);
        yield return new WaitUntil(() => GameManager.Instance.chatController.isNarrativeDesingActive);

        // Permitir que el NPC responda a triggers
        ConvaiNPCManager.Instance.isEnabledToSendText = true;
        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = true;
        ConvaiNPCManager.Instance.isEnabledToShowText = true;

        // Espera a que Roberta empiece o timeout
        yield return StartCoroutine(GameManager.Instance.chatController.WaitForNPCTalkingOrTimeout());
        Debug.Log(1);

        // Si hubo timeout, permitir retry/cancel
        if (GameManager.Instance.chatController.npcTimeoutOccurred)
        {
            Debug.Log("TimeOut");

            // Manejar decisión del usuario (retry / cancel)
            yield return StartCoroutine(HandleTimeoutForNarrative(() =>
            {
                // retryAction: relanzar trigger
                CallTrigger(narrativeTankToolBox);
            }));

            if (GameManager.Instance.chatController.npcTimeoutOccurred) yield break; // usuario canceló o fallo repetido
        }

        // espera que termine de hablar
        GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(false);
        yield return new WaitWhile(() => isTalking);
        Debug.Log(2);

        // Ahora el chat queda abierto hasta que userFinished sea true
        while (!GameManager.Instance.chatController.userFinished)
        {
            Debug.Log(3);
            currentState = NarrativeState.WaitingForConfirmation;

            // Activar UI de grabación y botones cuando sea necesario
            GameManager.Instance.chatController.chatAIBoxUI._answerButtonsHolerObject.SetActive(true);
            GameManager.Instance.chatController.chatAIBoxUI._answerButtonsRecordingObject.SetActive(true);
            GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(false);

            // Espera que el usuario grabe o envíe texto (WaitForUserAudio maneja la UI)
            yield return StartCoroutine(GameManager.Instance.chatController.WaitForUserAudio());

            // Cuando el usuario envía, los flags en chatController se actualizan
            // Esperamos que Roberta comience a hablar o timeout
            yield return StartCoroutine(GameManager.Instance.chatController.WaitForNPCTalkingOrTimeout());

            if (GameManager.Instance.chatController.npcTimeoutOccurred)
            {
                // Mostrar panel y gestionar retry/cancel
                yield return StartCoroutine(HandleTimeoutForNarrative(() =>
                {
                    // retryAction: no sabemos si fue texto o voz aquí, por seguridad
                    // volver a activar holder para que el usuario reenvíe
                    GameManager.Instance.chatController.chatAIBoxUI._answerButtonsHolerObject.SetActive(true);
                    GameManager.Instance.chatController.chatAIBoxUI._answerButtonsRecordingObject.SetActive(true);
                }));

                if (GameManager.Instance.chatController.npcTimeoutOccurred)
                {
                    // Usuario canceló o reintento falló
                    yield break;
                }
            }

            // Esperar que termine de hablar antes de iterar
            yield return new WaitWhile(() => isTalking);

            // limpiar flags de UI para la próxima iteración
            GameManager.Instance.chatController.chatAIBoxUI._answerButtonsHolerObject.SetActive(false);
            GameManager.Instance.chatController.chatAIBoxUI._answerButtonsRecordingObject.SetActive(false);
            GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(true);
        }

        // Cierre del chat
        yield return StartCoroutine(EndChat());
    }

    //public IEnumerator StartFinalTestFeedBack()
    //{
    //    currentState = NarrativeState.Introduction;
    //    GameManager.Instance.chatController.retryCount = 0;

    //    // --- UI Setup ---
    //    GameManager.Instance.chatController.chatAIBoxUI.gameObject.SetActive(true);
    //    GameManager.Instance.chatController.chatAIBoxUI._playerCommandPromptPanelObject.SetActive(true);
    //    GameManager.Instance.chatController.chatAIBoxUI._answerButtonsHolerObject.SetActive(false);
    //    GameManager.Instance.chatController.chatAIBoxUI._answerButtonsObject.SetActive(false);
    //    GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(true);

    //    // --- 1. INTRODUCCIÓN ---
    //    CallTrigger(narrativeFinalTestFeedBack);
    //    yield return new WaitUntil(() => GameManager.Instance.chatController.isNarrativeDesingActive);

    //    ConvaiNPCManager.Instance.isEnabledToSendText = true;
    //    ConvaiNPCManager.Instance.isEnabledToGetNewNPC = true;
    //    ConvaiNPCManager.Instance.isEnabledToShowText = true;

    //    yield return new WaitUntil(() => isTalking);
    //    GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(false);
    //    yield return new WaitUntil(() => !isTalking);

    //    // Esperar OK del usuario para continuar
    //    currentState = NarrativeState.WaitingForConfirmation;
    //    yield return GameManager.Instance.chatController.WaitForUserConfirmation();
    //    if (currentState == NarrativeState.Error) yield break;

    //    // --- 2. ENVÍO DE RESPUESTA DEL USUARIO ---
    //    currentState = NarrativeState.AwaitingUserResponse;
    //    string userAnswer = GameManager.Instance.playerStats.sessions[
    //        GameManager.Instance.playerStats.lastSessionIndex
    //    ].finalAnswer;

    //    if (string.IsNullOrEmpty(userAnswer))
    //        userAnswer = "El usuario no responde.";

    //    SendPlayerMessage(userAnswer);

    //    // Esperar que IA hable (análisis o ejemplo)
    //    yield return new WaitUntil(() => isTalking);
    //    GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(false);
    //    yield return new WaitUntil(() => !isTalking);
    //    yield return new WaitForSeconds(0.5f);

    //    // --- 3. DETECTAR TIPO DE RESPUESTA ---

    //    yield return new WaitUntil(() => isNarrativeSectionConfirmed);
    //    SectionStartConfirmation(false);

    //    if (!isExampleSection)
    //    {
    //        // === RUTA A: Feedback en dos partes ===
    //        // 🟣 Parte 1 completada → esperar OK
    //        currentState = NarrativeState.AwaitingUserResponse;
    //        yield return GameManager.Instance.chatController.WaitForUserConfirmation();

    //        if (currentState == NarrativeState.Error) yield break;

    //        // 🟣 Enviar OK → Parte 2
    //        SendPlayerMessage("ok");
    //        ExampleSectionState(false);

    //        yield return new WaitUntil(() => isTalking);
    //        yield return new WaitUntil(() => !isTalking);
    //        yield return new WaitForSeconds(0.5f);
    //    }

    //    // --- 4. ESPERAR OK FINAL PARA CIERRE ---
    //    currentState = NarrativeState.AwaitingFinalOk;
    //    yield return GameManager.Instance.chatController.WaitForUserConfirmation();
    //    if (currentState == NarrativeState.Error) yield break;

    //    SendPlayerMessage("ok");

    //    // --- 5. MENSAJE DE CIERRE ---
    //    currentState = NarrativeState.Closing;
    //    yield return new WaitUntil(() => isTalking);
    //    GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(false);
    //    yield return new WaitUntil(() => !isTalking);

    //    // --- 6. LIMPIEZA FINAL ---
    //    yield return StartCoroutine(GameManager.Instance.chatController.EndChat());

    //    Debug.Log("✅ Feedback final completado correctamente.");
    //}

    public IEnumerator StartFinalTestFeedBack()
    {
        Debug.Log("Roberta: Iniciando StartFinalTestFeedBack");
        currentState = NarrativeState.Introduction;
        GameManager.Instance.chatController.retryCount = 0;

        // Preparar UI
        GameManager.Instance.chatController.chatAIBoxUI.gameObject.SetActive(true);
        GameManager.Instance.chatController.chatAIBoxUI._playerCommandPromptPanelObject.SetActive(true);
        GameManager.Instance.chatController.chatAIBoxUI._answerButtonsHolerObject.SetActive(false);
        GameManager.Instance.chatController.chatAIBoxUI._answerButtonsObject.SetActive(false);
        GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(true);

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

        GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(false);
        yield return new WaitWhile(() => isTalking);
        Debug.Log(2);


        // 2) ESPERAR CONFIRMACIÓN INICIAL (OK) POR PARTE DEL USUARIO
        currentState = NarrativeState.WaitingForConfirmation;
        yield return StartCoroutine(GameManager.Instance.chatController.WaitForUserConfirmation());
        if (currentState == NarrativeState.Error) yield break;

        // 3) ENVIAR RESPUESTA DEL USUARIO (obtenida de sesiones)
        currentState = NarrativeState.AwaitingUserResponse;

        string newAnswer = (!string.IsNullOrEmpty(GameManager.Instance.playerStats.sessions[GameManager.Instance.playerStats.lastSessionIndex].finalAnswer)
            ? GameManager.Instance.playerStats.sessions[GameManager.Instance.playerStats.lastSessionIndex].finalAnswer
            : "El usuario no responde");

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

    // COROUTINE para limpiar y cerrar el chat de forma ordenada
    IEnumerator EndChat()
    {
        GameManager.Instance.chatController.chatAIBoxUI.ClearUI();
        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = false;
        ConvaiNPCManager.Instance.isEnabledToShowText = false;
        ConvaiNPCManager.Instance.isEnabledToSendText = false;
        isApproved = false;
        GameManager.Instance.chatController.userFinished = false;

        // small delay antes de desactivar NPC activo (por seguridad de UI)
        yield return new WaitForSeconds(0.5f);
        ConvaiNPCManager.Instance.SetActiveConvaiNPC(null);
        GameManager.Instance.chatController.chatAIBoxUI.gameObject.SetActive(false);
    }

    

    #region Métodos abstractos implementados / overrides
    public override void AnalyzeAIResponse()
    {
        // Mantengo la lógica original de análisis que tenías,
        // aquí solo delegamos al método padre si lo necesitas.
        if (GameManager.Instance.chatController.iaResponseLines == null || GameManager.Instance.chatController.iaResponseLines.Count == 0)
            return;

        isApproved = false; // Valor por defecto

        List<string> lineasExpandida = new List<string>();

        foreach (string linea in GameManager.Instance.chatController.iaResponseLines)
        {
            string[] subLineas = linea.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.None);
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

        isEndingAnalyzeAIResponse = false;
    }
    #endregion

    //public IEnumerator StartTankToolBox()
    //{
    //    Debug.Log("🔹 Iniciando flujo de asistencia de Roberta (Toolbox).");
    //    currentState = NarrativeState.Introduction;

    //    // --- UI Setup ---
    //    GameManager.Instance.chatController.chatAIBoxUI.gameObject.SetActive(true);
    //    GameManager.Instance.chatController.chatAIBoxUI._playerCommandPromptPanelObject.SetActive(true);
    //    GameManager.Instance.chatController.chatAIBoxUI._answerButtonsHolerObject.SetActive(false);
    //    GameManager.Instance.chatController.chatAIBoxUI._answerButtonsObject.SetActive(false);
    //    GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(true);

    //    CallTrigger(narrativeTankToolBox);
    //    yield return new WaitUntil(() => GameManager.Instance.chatController.isNarrativeDesingActive);
    //    ConvaiNPCManager.Instance.isEnabledToSendText = true;
    //    ConvaiNPCManager.Instance.isEnabledToGetNewNPC = true;
    //    ConvaiNPCManager.Instance.isEnabledToShowText = true;


    //    yield return new WaitUntil(() => isTalking);
    //    GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(false);
    //    yield return new WaitUntil(() => !isTalking);

    //    while (!GameManager.Instance.chatController.userFinished)
    //    {
    //        currentState = NarrativeState.WaitingForConfirmation;
    //        yield return GameManager.Instance.chatController.WaitForUserAudio();

    //        GameManager.Instance.chatController.isSendingMessage = false;
    //        GameManager.Instance.chatController.isRecordingMessage = false;

    //        yield return new WaitUntil(() => isTalking);
    //        GameManager.Instance.chatController.chatAIBoxUI._loadingObject.SetActive(false);
    //        yield return new WaitUntil(() => !isTalking);

    //    }

    //    yield return StartCoroutine(GameManager.Instance.chatController.EndChat());
    //}

    


    //public override void AnalyzeAIResponse()
    //{
    //    if (GameManager.Instance.chatController.iaResponseLines == null || GameManager.Instance.chatController.iaResponseLines.Count == 0)
    //        return;

    //    isApproved = false; // Valor por defecto

    //    List<string> lineasExpandida = new List<string>();

    //    foreach (string linea in GameManager.Instance.chatController.iaResponseLines)
    //    {
    //        string[] subLineas = linea.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
    //        lineasExpandida.AddRange(subLineas);
    //    }

    //    bool resultadoDetectado = false;

    //    for (int i = 0; i < lineasExpandida.Count; i++)
    //    {
    //        string linea = lineasExpandida[i].Trim().ToLower();

    //        if (linea.StartsWith("resultado final"))
    //        {
    //            Debug.Log($"[Paso 1] Línea detectada: {linea}");

    //            if (linea.Contains("desaprobado"))
    //            {
    //                isApproved = false;
    //                resultadoDetectado = true;
    //                Debug.Log("[Paso 2] Desaprobado detectado en misma línea.");
    //                break;
    //            }
    //            else if (linea.Contains("aprobado"))
    //            {
    //                isApproved = true;
    //                resultadoDetectado = true;
    //                Debug.Log("[Paso 2] Aprobado detectado en misma línea.");
    //                break;
    //            }

    //            if (i + 1 < lineasExpandida.Count)
    //            {
    //                string siguienteLinea = lineasExpandida[i + 1].Trim().ToLower();
    //                Debug.Log("[Paso 3] Siguiente línea: " + siguienteLinea);

    //                if (siguienteLinea.Contains("desaprobado"))
    //                {
    //                    isApproved = false;
    //                    resultadoDetectado = true;
    //                    Debug.Log("[Paso 4] Desaprobado detectado en siguiente línea.");
    //                    break;
    //                }
    //                else if (siguienteLinea.Contains("aprobado"))
    //                {
    //                    isApproved = true;
    //                    resultadoDetectado = true;
    //                    Debug.Log("[Paso 4] Aprobado detectado en siguiente línea.");
    //                    break;
    //                }
    //            }
    //        }
    //    }

    //    // ✅ Si no se detectó a través de "resultado final", buscar en todo el texto
    //    if (!resultadoDetectado)
    //    {
    //        foreach (string linea in lineasExpandida)
    //        {
    //            string check = linea.Trim().ToLower();

    //            if (check == "aprobado")
    //            {
    //                isApproved = true;
    //                Debug.Log("[Paso Extra] Aprobado detectado sin cabecera.");
    //                break;
    //            }
    //            else if (check == "desaprobado")
    //            {
    //                isApproved = false;
    //                Debug.Log("[Paso Extra] Desaprobado detectado sin cabecera.");
    //                break;
    //            }
    //        }
    //    }

    //    Debug.Log("[Paso Final] Resultado final (bool): " + isApproved);

    //    RobertaController.Instance.robertaAI.isEndingAnalyzeAIResponse = false;
    //}
}
