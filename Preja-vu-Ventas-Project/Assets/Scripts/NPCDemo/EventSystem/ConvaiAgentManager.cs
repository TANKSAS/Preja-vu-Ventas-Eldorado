using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Convai.Scripts.Runtime.Core;

[System.Serializable]
public class AgentData
{
    [Header("Información del Agente")]
    public string agentName;
    public string characterID;
    [TextArea(3, 5)]
    public string description;
    
    [Header("Configuración")]
    public bool resetHistoryOnSwitch = true;
    public float switchDelay = 0.5f;
}

public class ConvaiAgentManager : MonoBehaviour
{
    [Header("Referencias")]
    public ConvaiNPC convaiNPC;
    
    [Header("Configuración de Agentes")]
    public List<AgentData> agents = new List<AgentData>();
    
    [Header("UI Feedback")]
    public GameObject loadingPanel;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    [Header("UI Botones de Mensajes")]
    public Button button1;
    public Button button2;
    public Button button3;
    public List<string> messageList;
    
    private int currentAgentIndex = 0;
    private bool isSwitching = false;
    private string originalCharacterID;
    private Dictionary<string, string> agentSessions = new Dictionary<string, string>();
    
    void Start()
    {
        InitializeSystem();
        // Asignar listeners a los botones
        if (button1 != null && messageList.Count > 0)
            button1.onClick.AddListener(() => SendMessageToActiveAgent(messageList[0], 0));
        if (button2 != null && messageList.Count > 1)
            button2.onClick.AddListener(() => SendMessageToActiveAgent(messageList[1], 1));
        if (button3 != null && messageList.Count > 2)
            button3.onClick.AddListener(() => SendMessageToActiveAgent(messageList[2], 2));
    }
    
    void InitializeSystem()
    {
        // Guardar el ID original del personaje
        if (convaiNPC != null)
        {
            originalCharacterID = convaiNPC.characterID;
            LogDebug($"ID original guardado: {originalCharacterID}");
        }
        
        // Inicializar sesiones
        InitializeAgentSessions();
    }
    
   
    
    void InitializeAgentSessions()
    {
        foreach (var agent in agents)
        {
            if (!agentSessions.ContainsKey(agent.characterID))
            {
                agentSessions[agent.characterID] = System.Guid.NewGuid().ToString();
                LogDebug($"Sesión creada para {agent.agentName}: {agentSessions[agent.characterID]}");
            }
        }
    }
    
    public void SwitchToAgent(int agentIndex)
    {
        if (isSwitching)
        {
            LogDebug("Cambio en progreso, ignorando solicitud");
            return;
        }
        
        if (agentIndex < 0 || agentIndex >= agents.Count)
        {
            LogDebug($"Índice de agente inválido: {agentIndex}");
            return;
        }
        
        if (agentIndex == currentAgentIndex)
        {
            LogDebug($"Ya estamos usando el agente {agents[agentIndex].agentName}");
            return;
        }
        
        StartCoroutine(SwitchToAgentCoroutine(agentIndex));
    }
    
    IEnumerator SwitchToAgentCoroutine(int agentIndex)
    {
        isSwitching = true;
        var targetAgent = agents[agentIndex];
        
        LogDebug($"Iniciando cambio a agente: {targetAgent.agentName}");
        
        // Mostrar panel de carga
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
        
        // Detener cualquier conversación en curso
        if (convaiNPC != null)
        {
            StopCurrentConversation();
        }
        
        // Esperar un momento para que se procese la detención
        yield return new WaitForSeconds(targetAgent.switchDelay);
        
        // Realizar el cambio
        PerformAgentSwitch(agentIndex);
        
        // Esperar un momento adicional para estabilización
        yield return new WaitForSeconds(0.2f);
        
        // Ocultar panel de carga
        if (loadingPanel != null)
            loadingPanel.SetActive(false);
        
        
        // Resetear historial si es necesario
        if (targetAgent.resetHistoryOnSwitch)
        {
            ResetConversationHistory();
        }
        
        isSwitching = false;
        LogDebug($"Cambio completado a: {targetAgent.agentName}");
        
        // Disparar evento personalizado
        CustomEventManager.TriggerEvent($"AgentSwitched_{targetAgent.agentName}");
    }
    
    void PerformAgentSwitch(int agentIndex)
    {
        currentAgentIndex = agentIndex;
        var targetAgent = agents[agentIndex];
        
        if (convaiNPC != null)
        {
            // Cambiar el ID del personaje
            convaiNPC.characterID = targetAgent.characterID;
            
            // Generar nueva sesión si es necesario
            if (!agentSessions.ContainsKey(targetAgent.characterID))
            {
                agentSessions[targetAgent.characterID] = System.Guid.NewGuid().ToString();
            }
            
            LogDebug($"Agente cambiado a: {targetAgent.agentName} (ID: {targetAgent.characterID})");
        }
    }
    
    void StopCurrentConversation()
    {
        if (convaiNPC != null)
        {
            // Detener cualquier proceso de conversación activo
            convaiNPC.StopAllCoroutines();
            
            // Si hay métodos específicos para detener la conversación, úsalos aquí
            // Por ejemplo: convaiNPC.StopConversation();
            
            LogDebug("Conversación actual detenida");
        }
    }
    
    void ResetConversationHistory()
    {
        if (convaiNPC != null)
        {
            try
            {
                // Generar nueva sesión para forzar el reinicio del historial
                var currentAgent = agents[currentAgentIndex];
                agentSessions[currentAgent.characterID] = System.Guid.NewGuid().ToString();
                
                LogDebug($"Historial de conversación reiniciado para {currentAgent.agentName}");
            }
            catch (System.Exception e)
            {
                LogDebug($"Error al reiniciar historial: {e.Message}");
            }
        }
    }
   
    
    // Método público para obtener el agente actual
    public AgentData GetCurrentAgent()
    {
        if (currentAgentIndex < agents.Count)
            return agents[currentAgentIndex];
        return null;
    }
    
    // Método público para obtener la sesión actual
    public string GetCurrentSession()
    {
        var currentAgent = GetCurrentAgent();
        if (currentAgent != null && agentSessions.ContainsKey(currentAgent.characterID))
        {
            return agentSessions[currentAgent.characterID];
        }
        return null;
    }
    
    // Método para enviar mensaje al agente activo
    public void SendMessageToActiveAgent(string message, int index)
    {
        if (messageList != null && index >= 0 && index < messageList.Count)
        {
            message = messageList[index];
        }
        if (convaiNPC != null && !string.IsNullOrEmpty(message))
        {
            // Suponiendo que convaiNPC tiene un método SendPlayerMessage
            convaiNPC.SendTextDataAsync(message);
            LogDebug($"Mensaje enviado al agente activo: {message}");
        }
        else
        {
            LogDebug("No se pudo enviar el mensaje: convaiNPC o mensaje inválido");
        }
    }
    
    void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[ConvaiAgentManager] {message}");
        }
    }
    
}