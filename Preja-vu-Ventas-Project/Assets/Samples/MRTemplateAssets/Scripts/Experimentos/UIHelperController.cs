using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIHelperController : MonoBehaviour
{
    [Header("Referencias UI")] 
    public TextMeshProUGUI tituloUI;       // Elemento de título usando TextMeshProUGUI.
    public TextMeshProUGUI textoUI;        // Elemento de texto usando TextMeshProUGUI.
    public TextMeshProUGUI tiempoUI;       // Elemento de tiempo usando TextMeshProUGUI.
    public Image barraProgreso;            // Imagen de la barra de progreso de tipo Filled.

    [Header("Datos del Temporizador")] 
    public string[] titulos;               // Array de títulos que se van a mostrar.
    public string[] textos;                // Array de textos que se van a mostrar.
    public float[] tiemposVisibles;                // Array de tiempos en segundos para cada ciclo de reinicio.
    public bool canContinue;

    [Header("Configuración")]
    public int repeticiones = 4;           // Número de veces que se repite el temporizador.
    public int indiceActual = 0;
    private Coroutine temporizadorCoroutine;

    public void IniciarTemporizador()
    {
        if (temporizadorCoroutine != null)
        {
            StopCoroutine(temporizadorCoroutine);
        }

        repeticiones = tiemposVisibles.Length;

        if (tiemposVisibles.Length > 0)
        {
            temporizadorCoroutine = StartCoroutine(TimerCoroutine());
        }
        else
        {
            Debug.LogError("Los arrays de tiemposVisibles y tiemposDeEspera deben tener al menos el número de repeticiones necesarias.");
        }
    }

    private IEnumerator TimerCoroutine()
    {
        for (int i = 0; i < repeticiones; i++)
        {
            yield return new WaitUntil(() => canContinue);

            // --- Activar panel y mostrar contenido ---
            UIManager.Instance.helperPanel.SetActive(true);

            float tiempoRestante = tiemposVisibles[i];
            float tiempoTotal = tiempoRestante;

            tituloUI.text = titulos[indiceActual];
            textoUI.text = textos[indiceActual];

            while (tiempoRestante > 0)
            {
                tiempoUI.text = $"{Mathf.FloorToInt(tiempoRestante / 60):D2}:{Mathf.FloorToInt(tiempoRestante % 60):D2}";
                barraProgreso.fillAmount = tiempoRestante / tiempoTotal;

                yield return new WaitForSeconds(1f);
                tiempoRestante -= 1f;
            }

            // --- Al terminar el tiempo, ocultar panel ---
            UIManager.Instance.helperPanel.SetActive(false);
            indiceActual = (indiceActual + 1) % textos.Length;
            canContinue = false;
        }

    }

    public void ResetearTemporizador()
    {
        if (temporizadorCoroutine != null)
        {
            StopCoroutine(temporizadorCoroutine);
            temporizadorCoroutine = null;
        }

        indiceActual = 0;

        UIManager.Instance.helperPanel.SetActive(false);
        tituloUI.text = "";
        textoUI.text = "";
        tiempoUI.text = "";
        barraProgreso.fillAmount = 0;
    }
}
