using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Convai.Scripts.Runtime.Core;

public class MauricioAIController : ConvaiParametersEvaluator
{
    public int communication;
    public int trust;
    public int persuasion;
    public int objections;
    public int control;
    public int average;

    public IEnumerator StartGetPlayerResults(string dialogue)
    {
        //GameManager.Instance.chatAIBoxUI.gameObject.SetActive(true);
        ConvaiNPCManager.Instance.isEnabledToSendText = true;
        ConvaiNPCManager.Instance.isEnabledToShowText = true;

        yield return new WaitForSeconds(0.5f);
        //SendPlayerMessage("que quiere que fastidio, no sabe leer o que esta marca se llama silk es una marca muy mala, no se la rocomiendo le recomiendo mejor parmalat, la diferencia de esta leche es que es mas cara y ya de resto es leche, adios no vuelva");
        SendPlayerMessage(dialogue);
        Debug.Log("Enviando respuesta : " + dialogue);
        
        yield return new WaitUntil(() => isTalking);
        Debug.Log("Resultados Obtenidos");
        AnalyzeAIResponse();
        GameManager.Instance.chatAIBoxUI.ClearUI();
        //GameManager.Instance.chatAIBoxUI.gameObject.SetActive(true);
    }

    public override void AnalyzeAIResponse()
    {
        if (iaResponseLines.Count != 0) 
        {
            ProcessEvaluation();
        } 
        else
        {
            Debug.LogError("No hay palabras claves");
            return;
        }
        
    }

    void ProcessEvaluation()
    {
        // Recorremos cada entrada recibida
        foreach (string input in iaResponseLines)
        {
            // Dividimos por saltos de línea en caso de que un solo string tenga varias frases
            string[] lines = input.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string cleanLine = line.Trim(); // Eliminamos espacios innecesarios

                if (cleanLine.StartsWith("Effective Communication") || cleanLine.StartsWith("Communication Effectiveness"))
                //if (cleanLine.StartsWith(LanguageManager.Instance.GetStringValue("CommunicationEffectivenessEvaluationCriteria")))
                    communication = ExtractPercentage(cleanLine);
                else if (cleanLine.StartsWith("Confidence") || cleanLine.StartsWith("Trust"))
                //else if (cleanLine.StartsWith(LanguageManager.Instance.GetStringValue("TrustEvaluationCriteria")))
                    trust  = ExtractPercentage(cleanLine);
                else if (cleanLine.StartsWith("Persuasion"))
                //else if (cleanLine.StartsWith(LanguageManager.Instance.GetStringValue("PersuasionEvaluationCriteria")))
                    persuasion = ExtractPercentage(cleanLine);
                else if (cleanLine.StartsWith("Objection Handling"))
                //else if (cleanLine.StartsWith(LanguageManager.Instance.GetStringValue("ObjectionHandlingEvaluationCriteria")))
                    objections = ExtractPercentage(cleanLine);
                else if (cleanLine.StartsWith("Conversation Control"))
                //else if (cleanLine.StartsWith(LanguageManager.Instance.GetStringValue("ConversationControlEvaluationCriteria")))
                    control = ExtractPercentage(cleanLine);
                else
                    Debug.Log("No esta");
            }
        }

        average = (communication + trust + persuasion + objections + control) / 5;

        Debug.Log($"Evaluación IA:\n" +
                  $"Comunicación: {communication}%\n" +
                  $"Confianza: {trust }%\n" +
                  $"Persuasión: {persuasion}%\n" +
                  $"Objeciones: {objections}%\n" +
                  $"Control: {control}%\n" +
                  $"Promedio General: {average}%");

        //iaResponseLines.Clear(); // Limpias después de procesar
    }

    int ExtractPercentage(string line)
    {
        string[] parts = line.Split(':');
        
        if (parts.Length > 1)
        {
            string numberStr = parts[1].Trim().Replace("%", "");
            if (int.TryParse(numberStr, out int value))
                return value;
        }
        
        return 0;
    }
}
