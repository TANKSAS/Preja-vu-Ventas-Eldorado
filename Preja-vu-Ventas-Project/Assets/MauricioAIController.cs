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
        //SendPlayerMessage("¿Cómo estás? Bienvenida. Bienvenida, buenas tardes a Colchones El Dorado. Mi nombre es Ángela, ¿en qué te podemos asesorar? Ok. Bueno, nosotros tenemos variedad en todo lo que es el portafolio de colchones El Dorado, pero para entender mejor, eh, digamos, ¿hace cuánto tiempo tienes tu colchón? ¿Hace cuánto tiempo tienes tu colchón, más o menos? ¿Cuánto hace que lo usas? Bueno, mira, este, eeeh, este tiene un sistema en el cual, digamos que te permite relajarte mejor en las noches. Este es un colchón, eh... eh, confortable, ¿sí? De igual manera, pues acuérdate que el 30 % del descanso también equivale a una buena almohada. Esta es una almohada que es, eh, cervical, te va a ayudar a mejor postura en el tema del sueño. La ideal es que te acuestes totalmente para identificar, digamos, que estés durmiendo bien en las posiciones donde generalmente-- como descansas. ¿Sí? De igual manera, también tenemos otros tipos de colchón. Lo ideal es que tú puedas, eh... acostarse en todos los demás. Sí, claro. Tenemos, por ejemplo, acá esta. Si quieres, acuéstate en este. Este es un poco más confortable. ¿Sí? Lo ideal es tomar la posición de descanso en... como generalmente duermes, ¿vale? Cómo lo sientes. En este caso. Ay, se pausó. Adicional a ello, este, por ejemplo, ya va a tener una tela un poco más, eh, cómoda debido al gramaje que tiene, digamos, que la suavidad que tiene el colchón como tal. ¿Sí? Cuéntame. ¿Qué preguntas tienes? Bueno, básicamente la mayor diferencial es que nosotros manejamos el 20 % más de amortiguadores, eh, que en el mercado convencional. Acá vas a encontrar mayor número de amortiguadores, poniende más materia prima, ¿sí? Eh, adicional a ello, no solamente tiene más confort el colchón en general, sino que también se va a adaptar más a las curvaturas de tu cuerpo. Adicional, tiene más, eh, digamos, tecnología las telas de tercera generación. Por ejemplo, el Magneros que estábamos mirando tiene una tecnología que la tela hace que te ayude a reducir la hormona del cortisol y mejora, digamos, la calidad de sueño. Claro, totalmente. Igual acá está en la página, lo podemos buscar. Te muestro acá directamente en dónde están todos los componentes que tiene el colchón. Mira, acá está todo. ¿Mmm? Todo es respaldado directamente, es una tecnología alemana. Sí, señora. El envío se demora cuarenta y ocho horas. ¿Sí? O sea que se ajusta dentro de la semana correspondiente. Igual, si quieres, regálame acá la dirección de envío y te hacemos llegar inmediatamente... Bueno, en cuarenta y ocho horas, pues, el, el paquete como tal. ¿Qué método de pago vas a utilizar? Listo, entonces, eh... vamos a tomar acá los datos.");
        SendPlayerMessage(dialogue);
        Debug.Log("Enviando respuesta : " + dialogue);
        
        yield return new WaitUntil(() => isTalking);
        Debug.Log("Resultados Obtenidos");
        AnalyzeAIResponse();
        GameManager.Instance.chatController.chatAIBoxUI.ClearUI();
        //GameManager.Instance.chatAIBoxUI.gameObject.SetActive(true);
    }

    public override void AnalyzeAIResponse()
    {
        if (GameManager.Instance.chatController.iaResponseLines.Count != 0) 
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
        foreach (string input in GameManager.Instance.chatController.iaResponseLines)
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
