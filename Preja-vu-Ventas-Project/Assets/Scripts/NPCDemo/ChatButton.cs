using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // Namespace para TextMeshPro
using Convai.Scripts.Runtime.Core; // Namespace de Convai

public class ChatButton : MonoBehaviour
{
    [Header("UI References")]
    private Button button;
    public TextMeshProUGUI statusText; // Texto TMP para mostrar el estado actual
    public Image LoaderImage;
    
    [Header("Convai References")]
    public ConvaiNPC convaiNPC; // Referencia al componente ConvaiNPC
    
    [Header("Voice Settings")]
    public bool isPushToTalk = true; // Si es push-to-talk o toggle
    
    private bool isRecording = false;
    private bool isProcessing = false;

    void Start()
    {
        // Obtener el componente Button
        LoaderImage.gameObject.SetActive(false);
        button = GetComponent<Button>();
        
        if (button != null)
        {
            // Configurar eventos del botón según el modo
            if (isPushToTalk)
            {
                SetupPushToTalkMode();
            }
            else
            {
                button.onClick.AddListener(ToggleRecording);
            }
        }
        else
        {
            Debug.LogError("No se encontró el componente Button en este GameObject");
        }

        // Verificar que tenemos la referencia al ConvaiNPC
        if (convaiNPC == null)
        {
            convaiNPC = FindObjectOfType<ConvaiNPC>();
            if (convaiNPC == null)
            {
                Debug.LogError("No se encontró ningún ConvaiNPC en la escena. Asegúrate de tener un personaje Convai configurado.");
            }
        }

        UpdateStatusText("Listo para hablar");
    }

    void SetupPushToTalkMode()
    {
        // Para push-to-talk necesitamos detectar cuando se presiona y suelta
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }

        // Limpiar triggers existentes
        eventTrigger.triggers.Clear();

        // Configurar PointerDown (presionar)
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => { StartRecording(); });
        eventTrigger.triggers.Add(pointerDownEntry);

        // Configurar PointerUp (soltar)
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => { StopRecording(); });
        eventTrigger.triggers.Add(pointerUpEntry);

        // También manejar cuando el cursor sale del botón mientras está presionado
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
        pointerExitEntry.eventID = EventTriggerType.PointerExit;
        pointerExitEntry.callback.AddListener((data) => { 
            if (isRecording) StopRecording(); 
        });
        eventTrigger.triggers.Add(pointerExitEntry);
    }

    void StartRecording()
    {
        if (convaiNPC == null || isProcessing)
        {
            Debug.LogWarning("ConvaiNPC no disponible o ya procesando");
            return;
        }

        if (!isRecording)
        {
            Debug.Log("Iniciando grabación de voz...");
            
            // Comenzar la grabación usando Convai
            convaiNPC.StartListening();
            
            isRecording = true;
            UpdateStatusText("Escuchando...");
            
            // Cambiar el color del botón para indicar que está grabando
            ChangeButtonColor(Color.red);
        }
    }

    void StopRecording()
    {
        if (convaiNPC == null || !isRecording)
        {
            return;
        }

        Debug.Log("Deteniendo grabación de voz...");
        
        // Detener la grabación y enviar al chat
        convaiNPC.StopListening();
        
        isRecording = false;
        isProcessing = true;
        LoaderImage.gameObject.SetActive(true);
        UpdateStatusText("Procesando...");
        
        // Restaurar el color original del botón
        ChangeButtonColor(Color.white);
        
        // Opcional: Agregar un delay antes de permitir otra grabación
        StartCoroutine(ProcessingCooldown());
    }

    void ToggleRecording()
    {
        if (isRecording)
        {
            StopRecording();
        }
        else
        {
            StartRecording();
        }
    }

    IEnumerator ProcessingCooldown()
    {
        // Esperar un poco para evitar spam de grabaciones
        yield return new WaitForSeconds(1f);
        
        isProcessing = false;
        LoaderImage.gameObject.SetActive(false);
        UpdateStatusText("Listo para hablar");
    }

    void ChangeButtonColor(Color color)
    {
        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = color;
            button.colors = colors;
        }
    }

    void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    // Método público para cambiar entre modos
    public void SetPushToTalkMode(bool pushToTalk)
    {
        isPushToTalk = pushToTalk;
        
        // Limpiar eventos existentes
        button.onClick.RemoveAllListeners();
        
        // Reconfigurar según el nuevo modo
        if (isPushToTalk)
        {
            SetupPushToTalkMode();
        }
        else
        {
            button.onClick.AddListener(ToggleRecording);
        }
    }

    // Métodos para eventos de Convai (opcionales - para debugging)
    void OnEnable()
    {
        if (convaiNPC != null)
        {
            // Suscribirse a eventos de Convai si están disponibles
            // Nota: Los nombres exactos de eventos pueden variar según la versión
            // convaiNPC.OnResponseReceived += OnConvaiResponse;
            // convaiNPC.OnTranscriptReceived += OnTranscriptReceived;
        }
    }

    void OnDisable()
    {
        if (convaiNPC != null)
        {
            // Desuscribirse de eventos
            // convaiNPC.OnResponseReceived -= OnConvaiResponse;
            // convaiNPC.OnTranscriptReceived -= OnTranscriptReceived;
        }
    }

    // Métodos de callback para eventos de Convai (opcionales)
    /*
    void OnConvaiResponse(string response)
    {
        Debug.Log("Respuesta de Convai: " + response);
        UpdateStatusText("Respuesta recibida");
    }

    void OnTranscriptReceived(string transcript)
    {
        Debug.Log("Transcripción: " + transcript);
        UpdateStatusText("Texto: " + transcript);
    }
    */

    void OnDestroy()
    {
        // Limpiar listeners
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}