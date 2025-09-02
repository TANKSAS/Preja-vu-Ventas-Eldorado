using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomEventListener : MonoBehaviour
{
    [Header("Configuración del Evento")]
    public string eventToListen = "MyCustomEvent";
    
    [Header("Unity Event")]
    public UnityEvent OnEventTriggered;
    
    [Header("Controles")]
    [Space]
    [Tooltip("Ejecutar el evento manualmente para testing")]
    public bool executeEvent = false;

     void OnEnable()
    {
        if (!string.IsNullOrEmpty(eventToListen))
        {
            CustomEventManager.Subscribe(eventToListen, OnCustomEventTriggered);
            Debug.Log($"Suscrito al evento: '{eventToListen}'");
        }
    }

    void OnDisable()
    {
        if (!string.IsNullOrEmpty(eventToListen))
        {
            CustomEventManager.Unsubscribe(eventToListen, OnCustomEventTriggered);
        }
    }

    void OnCustomEventTriggered()
    {
        Debug.Log($"¡Evento '{eventToListen}' recibido!");
        TriggerEvent();
    }
    
    
    public void TriggerEvent()
    {
        Debug.Log($"¡Evento '{eventToListen}' ejecutado!");
        OnEventTriggered?.Invoke();
    }
    
    public void TriggerEvent(string eventToListen)
    {
        Debug.Log($"¡Evento '{eventToListen}' ejecutado!");
        OnEventTriggered?.Invoke();
    }
}