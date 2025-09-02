using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class AdjustUIBasedOnVRHeight : MonoBehaviour
{
    [Header("Settings")]
    public Transform vrHeadset; // Referencia al transform del casco VR
    public List<Transform> uiElements; // Lista de referencias a los UIs que queremos ajustar
    public float initialHeight = 1.7f; // Altura promedio inicial de la persona en metros
    public float heightOffset = 0.1f; // Ajuste extra para mejorar visibilidad

    private List<Vector3> originalUIPositions;

    void Start()
    {
        vrHeadset = GameObject.Find("XR Origin (XR Rig)").transform;
        
        if (vrHeadset == null)
        {
            Debug.LogError("Debes asignar el transform del casco VR en el inspector.");
            enabled = false;
            return;
        }

        if (uiElements == null || uiElements.Count == 0)
        {
            Debug.LogError("Debes asignar una lista de RectTransforms del UI en el inspector.");
            enabled = false;
            return;
        }

        // Guardar las posiciones iniciales de los UIs
        originalUIPositions = new List<Vector3>();
        foreach (var uiElement in uiElements)
        {
            if (uiElement != null)
            {
                originalUIPositions.Add(uiElement.position);
            }
            else
            {
                originalUIPositions.Add(Vector3.zero);
                Debug.LogWarning("Un elemento de la lista de UIs no está asignado.");
            }
        }

        // Ajustar las posiciones iniciales de los UIs con base en la altura inicial
        RecalculateUIPositions();
    }

    void Update()
    {
        // Actualizar dinámicamente las posiciones en caso de que sea necesario
        RecalculateUIPositions();
    }

    private void RecalculateUIPositions()
    {
        // Calcular la diferencia entre la altura inicial y la altura actual del casco VR
        float currentHeight = vrHeadset.position.y;
        float heightDifference = currentHeight - initialHeight;

        // Ajustar la posición en Y de cada UI
        for (int i = 0; i < uiElements.Count; i++)
        {
            if (uiElements[i] != null)
            {
                Vector3 newPosition = originalUIPositions[i];
                newPosition.y += heightDifference + heightOffset;
                uiElements[i].position = newPosition;
            }
        }
    }
}
