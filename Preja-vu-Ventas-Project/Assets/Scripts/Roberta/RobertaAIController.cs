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
    

    IEnumerator StartTankToolBox()
    {
        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = true;
        yield return new WaitForSeconds(5f);
        ConvaiNPCManager.Instance.isEnabledToShowText = false;
        CallTrigger(narrativeTankToolBox);

        yield return new WaitUntil(() => isTalking);
        Debug.Log("Inicio aqui la Introduccion de caja de herramientas");
        yield return new WaitUntil(() => !isTalking);
        yield return new WaitForSeconds(0.5f);

        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = false;
        ConvaiNPCManager.Instance.isEnabledToShowText = false;
    }

    public IEnumerator StartFinalTestFeedBack(AssessmentModule assessmentModule)
    {
        currentState = NarrativeState.Introduction;
        retryCount = 0;

        GameManager.Instance.chatAIBoxUI.gameObject.SetActive(true);
        GameManager.Instance.chatAIBoxUI._playerCommandPromptPanelObject.SetActive(true);
        GameManager.Instance.chatAIBoxUI._answerButtonsObject.SetActive(false);
        GameManager.Instance.chatAIBoxUI._loadingObject.SetActive(true);

        CallTrigger(narrativeFinalTestFeedBack);
        yield return new WaitUntil(() => isNarrativeDesingActive);

        ConvaiNPCManager.Instance.isEnabledToSendText = true;
        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = true;
        ConvaiNPCManager.Instance.isEnabledToShowText = true;

        // 🟡 [1] INTRODUCCIÓN DE LA IA
        yield return new WaitUntil(() => isTalking);
        GameManager.Instance.chatAIBoxUI._loadingObject.SetActive(false);
        yield return new WaitUntil(() => !isTalking);
        Debug.Log("Finalizó introducción de la IA.");

        // 🟠 [2] CONFIRMACIÓN DEL USUARIO (OK)
        currentState = NarrativeState.WaitingForConfirmation;
        yield return WaitForUserConfirmation();
        if (currentState == NarrativeState.Error) yield break;

        // 🟣 [3] USUARIO ENVÍA SU RESPUESTA (pitch)
        currentState = NarrativeState.AwaitingUserResponse;
        Debug.Log("Inicio de respuesta del usuario");
        yield return new WaitUntil(() => !WebRequestController.Instance.InProgress);

        SendPlayerMessage(!string.IsNullOrEmpty(assessmentModule.finalAnswer)
            ? assessmentModule.finalAnswer
            : "El usuario no responde");
        //"¿Sabía que los bancos tradicionales rechazan el 40 % de solicitudes de crédito de pequeñas empresas que en realidad SÍ pueden pagar ? Soy Patricia Morales de SmartCredit Solutions.Hemos desarrollado una plataforma de inteligencia artificial que analiza más de 200 variables financieras alternativas desde flujo de caja digital hasta patrones de ventas - para evaluar el verdadero riesgo crediticio de PyMEs en menos de 24 horas.Mientras otros sistemas bancarios solo revisan historial crediticio tradicional, nosotros vemos el panorama completo: transacciones digitales, ventas estacionales, proveedores, incluso reviews online.Los bancos que usan nuestra tecnología han aumentado sus aprobaciones crediticias un 60 % manteniendo la misma tasa de morosidad. ¿Le gustaría ver cómo podríamos analizar 100 solicitudes reales de su cartera la próxima semana ? ");

        yield return new WaitUntil(() => isTalking);
        GameManager.Instance.chatAIBoxUI._loadingObject.SetActive(false);
        yield return new WaitUntil(() => !isTalking);
        yield return new WaitForSeconds(0.5f);

        // 🔵 [4] FEEDBACK O EJEMPLO DE LA IA
        currentState = NarrativeState.ProcessingResponse;

        // IA responde (ya ocurrió arriba con la transcripción)
        // Aquí esperamos que el usuario confirme si entendió el feedback o ejemplo

        // 🟤 [5] CONFIRMACIÓN FINAL DEL USUARIO (OK)
        currentState = NarrativeState.AwaitingFinalOk;
        yield return WaitForUserConfirmation();
        if (currentState == NarrativeState.Error) yield break;

        // 🟣 Enviamos OK para que IA dé su mensaje de cierre
        SendPlayerMessage("ok");
        currentState = NarrativeState.Closing;

        yield return new WaitUntil(() => isTalking);
        GameManager.Instance.chatAIBoxUI._loadingObject.SetActive(false);
        yield return new WaitUntil(() => !isTalking);
        yield return new WaitForSeconds(0.5f);

        // ⚫ [6] CIERRE DE LA IA

        // 🧹 LIMPIEZA FINAL
        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = false;
        ConvaiNPCManager.Instance.isEnabledToShowText = false;
        ConvaiNPCManager.Instance.isEnabledToSendText= false;
        isApproved = false;
        GameManager.Instance.chatAIBoxUI.gameObject.SetActive(false);
        GameManager.Instance.chatAIBoxUI.ClearUI();
        ConvaiNPCManager.Instance.SetActiveConvaiNPC(null); // ✅ Limpia el NPC
        Debug.Log("Fin del feedback final.");
    }

    public IEnumerator StartQuestion(int methodologyIndex)
    {
        GameManager.Instance.chatAIBoxUI.gameObject.SetActive(true);
        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = true;
        ConvaiNPCManager.Instance.isEnabledToShowText = true;
        
        switch (methodologyIndex)
        {
            case 0:
                CallTrigger(narrativeTankToolboxJiujitsuQuiz);
                break;

            case 1:
                CallTrigger(narrativeTankToolboxJiujitsuQuiz);
                break;

            case 2:
                CallTrigger(narrativeTankToolboxJiujitsuQuiz);
                break;
                
            case 3:
                CallTrigger(narrativeTankToolboxJiujitsuQuiz);
                break;

            case 4:
                CallTrigger(narrativeTankToolboxJiujitsuQuiz);
                break;
        }

        yield return new WaitUntil(() => isNarrativeDesingActive);
        yield return new WaitUntil(() => isTalking);
        Debug.Log("Inicio aqui la Introduccion");
        yield return new WaitUntil(() => !isTalking);

        yield return new WaitForSeconds(0.5f);

        int questionIndex = 0; // problemas
        Debug.Log(questionIndex);

        SendPlayerMessage(questionIndex.ToString());
        yield return new WaitUntil(() => isTalking);
        Debug.Log("Inicio aqui la Pregunta");
        yield return new WaitUntil(() => !isTalking);
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Inicio Respuesta del usuario");
       // ConvaiNPCManager.Instance.isEnabledToShowText = true;

        yield return new WaitUntil(() => !isNarrativeDesingActive);
        yield return new WaitUntil(() => isTalking);
        Debug.Log("Inicio aqui el Feedback");
       // ConvaiNPCManager.Instance.isEnabledToShowText = false;
        yield return new WaitUntil(() => !isTalking);
        yield return new WaitForSeconds(0.5f);

        SendPlayerMessage("ok");
        yield return new WaitUntil(() => isTalking);
        Debug.Log("Inicio aqui la Calificacion");
        yield return new WaitUntil(() => !isTalking);
        yield return new WaitForSeconds(0.5f);

        ConvaiNPCManager.Instance.isEnabledToGetNewNPC = false;
        ConvaiNPCManager.Instance.isEnabledToShowText = false;
        GameManager.Instance.chatAIBoxUI.ClearUI();
    }

    public override void AnalyzeAIResponse()
    {
        if (iaResponseLines == null || iaResponseLines.Count == 0)
            return;

        isApproved = false; // Valor por defecto

        List<string> lineasExpandida = new List<string>();

        foreach (string linea in iaResponseLines)
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
