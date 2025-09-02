using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Convai.Scripts.Runtime.Core;


[System.Serializable]
public class CustomButtonEvent : MonoBehaviour
{
    [Header("Referencias")]
    public Button button;
    
    [Header("Configuración del Evento")]
    [Tooltip("Nombre único del evento que será disparado")]
    public string eventName = "MyCustomEvent";
    

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
            Debug.Log($"Botón configurado para disparar evento: '{eventName}'");
        }
        else
        {
            Debug.LogError("No se asignó ningún botón al script CustomButtonEvent");
        }
    
    }
    
    void OnButtonClick()
    {
        // Disparar evento personalizado
        if (!string.IsNullOrEmpty(eventName))
        {
            CustomEventManager.TriggerEvent(eventName);
        }
        
    }
    
    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}