using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatMapGraphController : Graph
{
    public HeatMapGraphHolder heatMapGraphHolder;
    public Color originalColor;
    public Sprite originalSprite;

    public AnimacionUIManager animacionUIManager;
    public RectTransform miPanel;
    HeatMapRating mapRating;

    public override void SetGraphSettings(GraphHolder newGraphHolder)
    {
        heatMapGraphHolder = ChooseGraph<HeatMapGraphHolder>(newGraphHolder);
    }

    public override IEnumerator ResetGraphHolderValues(GraphHolder graphHolder)
    {
        heatMapGraphHolder = ChooseGraph<HeatMapGraphHolder>(graphHolder);

        // Restaurar correctamente
        //var img = heatMapGraphHolder.heatMapImage.GetComponent<Image>();
        //img.color = originalColor;
        //img.sprite = originalSprite;

        //// Limpiar solo los datos temporales
        //heatMapGraphHolder.heatMapMaterial = null;
        //heatMapGraphHolder.heatMapSprite = null;

        //mapRating = HeatMapRating.Default;


        heatMapGraphHolder = ChooseGraph<HeatMapGraphHolder>(graphHolder);
        heatMapGraphHolder.heatMapImage.GetComponent<Image>().color = originalColor;
        heatMapGraphHolder.heatMapImage.GetComponent<Image>().sprite = originalSprite;
        heatMapGraphHolder.heatMapMaterial = null;
        heatMapGraphHolder.heatMapSprite = null;
        heatMapGraphHolder = null;
        mapRating = HeatMapRating.Default;
        yield return null;
    }

    public override void SetGraphParameters(int index)
    {
        // Limpiar primero para evitar imágenes mezcladas
        if (heatMapGraphHolder != null && heatMapGraphHolder.heatMapImage != null)
        {
            heatMapGraphHolder.heatMapImage.GetComponent<Image>().sprite = null;
            heatMapGraphHolder.heatMapImage.GetComponent<Image>().color = new Color(1, 1, 1, 0); // transparente temporal
            Debug.Log("se supone q mientras carga la imagen");
        }

        mapRating = GameManager.Instance.playerStats.sessions[index].heatMapRating;
        GameManager.Instance.screenshotController.LoadImage(GameManager.Instance.playerStats.sessions[index].imagePath);
        heatMapGraphHolder.heatMapMaterial = GameManager.Instance.screenshotController.GetHeatMapMaterial();
        heatMapGraphHolder.heatMapSprite = GameManager.Instance.screenshotController.GetHeatMapSprite();
    }

    public override IEnumerator GraphMaker()
    {
        // Asegúrate de que el holder esté asignado
        if (heatMapGraphHolder == null)
        {
            Debug.LogWarning("[HeatMap] GraphMaker: holder null.");
            yield break;
        }
        
        yield return null;

        originalColor = heatMapGraphHolder.heatMapImage.GetComponent<Image>().color;
        originalSprite = heatMapGraphHolder.heatMapImage.GetComponent<Image>().sprite;

        yield return null;

        heatMapGraphHolder.heatMapImage.GetComponent<Image>().color = Color.white;
        heatMapGraphHolder.heatMapImage.GetComponent<Image>().sprite = heatMapGraphHolder.heatMapSprite;
        yield return null;

       ShowQualificationTag();
    }

    
    public override void EndGraph()
    {
        //originalSprite = null;
        //originalColor = Color.white;
    }

    public override void ShowQualificationTag()
    {
        // 1️⃣ Obtener los valores del jugador (desde la sesión actual)
        float safe = GameManager.Instance.playerStats.sessions[GraphManager.Instance.currentSessionIndex].visualSafeZone;
        float danger = GameManager.Instance.playerStats.sessions[GraphManager.Instance.currentSessionIndex].visualDangerZone;

        // 2️⃣ Calcular la calificación del HeatMap
        mapRating = GraphManager.Instance.CalculateHeatMapRating(safe, danger);

        // 2️⃣ Definir las claves o textos según la calificación
        string detailKey = "";

        switch (mapRating)
        {
            case HeatMapRating.Excellent:
                detailKey = "HeatMapExcellent";
                Debug.Log(" Excelente");
                break;

            case HeatMapRating.Good:
                detailKey = "HeatMapGood";
                Debug.Log(" medio");
                break;

            case HeatMapRating.Low:
                detailKey = "HeatMapLow";
                Debug.Log(" Bajo");
                break;

            default:
                detailKey = "No hay datos suficientes para evaluar.";
                break;
        }

        Debug.Log($"[HeatMapGraph] Calificación obtenida: {mapRating} → {detailKey}");
        string feedbackText = LanguageManager.Instance.GetStringValue(detailKey);

        // 3️⃣ Mostrar el texto en pantalla
        if (heatMapGraphHolder != null && heatMapGraphHolder.interpretationText != null)
        {
            heatMapGraphHolder.interpretationText.gameObject.SetActive(true);
            heatMapGraphHolder.interpretationText.text = feedbackText;
        }
        else
        {
            Debug.LogWarning("[HeatMapGraph] resultClassificationText no está asignado en el Inspector.");
        }
    }
}
