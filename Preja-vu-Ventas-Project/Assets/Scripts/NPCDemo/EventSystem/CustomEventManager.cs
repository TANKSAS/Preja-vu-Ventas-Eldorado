using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Manager estático para manejar eventos personalizados
public static class CustomEventManager
{
    private static Dictionary<string, System.Action> events = new Dictionary<string, System.Action>();
    
    // Suscribirse a un evento
    public static void Subscribe(string eventName, System.Action callback)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName] += callback;
        }
        else
        {
            events[eventName] = callback;
        }
    }
    
    // Desuscribirse de un evento
    public static void Unsubscribe(string eventName, System.Action callback)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName] -= callback;
            if (events[eventName] == null)
            {
                events.Remove(eventName);
            }
        }
    }
    
    // Disparar un evento
    public static void TriggerEvent(string eventName)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName]?.Invoke();
            Debug.Log($"Evento disparado: {eventName}");
        }
        else
        {
            Debug.LogWarning($"Evento '{eventName}' no tiene suscriptores");
        }
    }
}