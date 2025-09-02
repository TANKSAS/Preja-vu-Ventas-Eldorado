using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class convaiEventsTrigger : ConvaiParametersEvaluator
{
    [Header("UI Mensajes")]
    public Button button1;
    public Button button2;
    public Button button3;
    public List<string> messageList;
    public NPCData data;

    void Start()
    {
        GameManager.Instance.chatAIBoxUI.gameObject.SetActive(true);
        if (button1 != null && button2 != null && button3 != null && messageList != null && messageList.Count >= 3)
        {
            button1.onClick.AddListener(() => SendMessageButton(messageList[0], 0));
            button2.onClick.AddListener(() => SendMessageButton(messageList[1], 1));
            button3.onClick.AddListener(() => SendMessageButton(messageList[2], 2));
        }
    }
    void SendMessageButton(string message, int index)
    {
        if (messageList != null && index >= 0 && index < messageList.Count)
        {
            message = messageList[index];
            SendPlayerMessage(message);
        }
    }
    
    public override void AnalyzeAIResponse()
    {
        // Implementa aquí la lógica para analizar la respuesta de la IA si es necesario
    }
}
