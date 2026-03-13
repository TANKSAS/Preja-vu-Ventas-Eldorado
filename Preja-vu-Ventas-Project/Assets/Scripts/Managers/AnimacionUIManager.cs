using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimacionUIManager : MonoBehaviour
{
    [Header("Duración de animación")]
    public float duracionMovimiento = 1.5f;

    [Header("Delay entre elementos (si deseas usarlos en secuencia)")]
    public float delayEntreElementos = 0.5f;

    [Header("Duración del fade")]
    public float duracionFade = 1f;

    // Guarda posiciones iniciales para resetear
    private Dictionary<RectTransform, Vector3> posicionesIniciales = new Dictionary<RectTransform, Vector3>();
    private Dictionary<RectTransform, Quaternion> rotacionesIniciales = new Dictionary<RectTransform, Quaternion>();

    /// <summary>
    /// Corrutina general: hace fade in, mueve y rota el panel según parámetros.
    /// </summary>
    public IEnumerator FadeInElemento(GameObject rectTransformHolder)
    {
        if (rectTransformHolder == null)
        {
            Debug.LogWarning("[AnimacionUI] FadeInElemento: Elemento no asignado.");
            yield break;
        }

        CanvasGroup cg = rectTransformHolder.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            Debug.LogWarning($"[AnimacionUI] {rectTransformHolder.name} no tiene CanvasGroup -> No se aplicará fade.");
            yield break;
        }

        // si el objeto está inactivo, actívalo antes
        if (!rectTransformHolder.activeInHierarchy)
            rectTransformHolder.SetActive(true);

        float inicial = cg.alpha;              // usa el alpha actual (no forzar 0)
        float objetivo = 1f;
        float elapsed = 0f;
        float duration = Mathf.Max(0.0001f, duracionFade);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            cg.alpha = Mathf.Lerp(inicial, objetivo, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = objetivo;
    }

    public IEnumerator AnimarElemento(GameObject rectTransformHolder, string direccion, float distancia, float rotacionY, float moverEnZ)
    {
        if (rectTransformHolder == null)
        {
            Debug.LogWarning("[AnimacionUI] Elemento no asignado.");
            yield break;
        }
        
        RectTransform newRectTransform = rectTransformHolder.GetComponent<RectTransform>();

        // Guarda posición inicial si aún no está registrada
        if (!posicionesIniciales.ContainsKey(newRectTransform))
        {
            posicionesIniciales[newRectTransform] = newRectTransform.localPosition;
            rotacionesIniciales[newRectTransform] = newRectTransform.localRotation;
        }

        // 🔹 Fase 2: Movimiento y rotación
        Vector3 origen = newRectTransform.localPosition;
        Vector3 destino = origen;

        switch (direccion.ToLower())
        {
            case "izquierda":
                destino += Vector3.left * distancia;
                break;
            case "derecha":
                destino += Vector3.right * distancia;
                break;
            case "quieto":
                break;
        }

        destino += Vector3.forward * moverEnZ;

        Quaternion rotacionInicial = newRectTransform.localRotation;
        Quaternion rotacionFinal = Quaternion.Euler(0f, rotacionY, 0f);

        float tiempo = 0f;
        while (tiempo < duracionMovimiento)
        {
            float t = tiempo / duracionMovimiento;
            newRectTransform.localPosition = Vector3.Lerp(origen, destino, t);
            newRectTransform.localRotation = Quaternion.Lerp(rotacionInicial, rotacionFinal, t);
            tiempo += Time.deltaTime;
            yield return null;
        }

        newRectTransform.localPosition = destino;
        newRectTransform.localRotation = rotacionFinal;

        Debug.Log($"[AnimacionUI] {rectTransformHolder.name} fade in + animación completada.");
     }

    // Hace fade out gradual de un panel
    public IEnumerator FadeOutElemento(GameObject rectTransformHolder)
    {
        if (rectTransformHolder == null)
        {
            Debug.LogWarning("[AnimacionUI] FadeOutElemento: Elemento no asignado.");
            yield break;
        }

        CanvasGroup cg = rectTransformHolder.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            Debug.LogWarning($"[AnimacionUI] {rectTransformHolder.name} no tiene CanvasGroup -> No se aplicará fade out.");
            yield break;
        }

        float inicial = cg.alpha;
        float objetivo = 0f;
        float elapsed = 0f;
        float duration = Mathf.Max(0.0001f, duracionFade);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            cg.alpha = Mathf.Lerp(inicial, objetivo, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cg.alpha = objetivo;
        rectTransformHolder.SetActive(false); // desactivar al final
    }



    /// <summary>
    /// Restaura todos los elementos animados a su posición, rotación y opacidad inicial.
    /// Llamar cuando el jugador vuelve a la sección de resultados.
    /// </summary>
    public void ResetearAnimaciones()
    {
       Debug.Log("Se resetean las animaciones");
        foreach (var kvp in posicionesIniciales)
        {
            RectTransform elemento = kvp.Key;
            elemento.localPosition = kvp.Value;
            elemento.localRotation = rotacionesIniciales[elemento];

            CanvasGroup cg = elemento.GetComponent<CanvasGroup>();
            if (cg != null)
                cg.alpha = 0f; // invisible hasta que vuelva a animarse
        }

        Debug.Log("[AnimacionUI] Todos los paneles reseteados a su estado inicial.");
    }
}