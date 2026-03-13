using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PieGraphController : Graph
{
    //======================================================================
    //  CONFIGURACIÓN DEL GRÁFICO (Barras, colores, texto)
    //======================================================================

    [Header(" Ajustes visuales del gráfico")]
    [SerializeField] float barLength = 1f;
    [SerializeField] float percentMultiplier = 100f;
    [SerializeField] PieGraphHolder pieGraphHolder;

    //======================================================================
    //  VARIABLES TEMPORALES (Lo que se calcula antes de dibujar)
    //======================================================================

    [Header("Valores temporales")]
    List<float> provitionalValuesPie = new List<float>();

    float valueSafeMovement;        // handsSafeZonaMovCounter
    float valueDangerMovement;      // handsDangerMovCounter

    float totalMovement;            // safe + danger
    float safePercent;              // %
    float dangerPercent;            // %

    KinesthesiaRating kinesthesiaRating;

    //======================================================================
    //  CONFIGURAR DESDE OUTSIDE (GraphManager llama esto)
    //======================================================================

    public override void SetGraphSettings(GraphHolder graphHolder)
    {
        pieGraphHolder = ChooseGraph<PieGraphHolder>(graphHolder);
    }

    public void SetValues(float safeMov, float dangerMov)
    {
        valueSafeMovement = safeMov;
        valueDangerMovement = dangerMov;
        PercentPieGraph();
    }

    public override void SetGraphParameters(int index)
    {
        kinesthesiaRating = GameManager.Instance.playerStats.sessions[index].kinesthesiaRating;
    }

    //======================================================================
    //   CÁLCULO DE LOS PORCENTAJES DEL PIEGRAPH (solo movimiento)
    //======================================================================

    void PercentPieGraph()
    {
        provitionalValuesPie.Clear();

        totalMovement = valueSafeMovement + valueDangerMovement;

        if (totalMovement <= 0)
            totalMovement = 1;

        safePercent = valueSafeMovement / totalMovement;
        dangerPercent = valueDangerMovement / totalMovement;

        provitionalValuesPie.Add(safePercent);
        provitionalValuesPie.Add(dangerPercent);
    }

    //======================================================================
    //  RESET VISUAL (cuando se cambia de panel)
    //======================================================================

    public override IEnumerator ResetGraphHolderValues(GraphHolder graphHolder)
    {
        PieGraphHolder newHolder = ChooseGraph<PieGraphHolder>(graphHolder);

        newHolder.value1Bar.fillAmount = newHolder.value2Bar.fillAmount = 0;
        newHolder.value1Bar.color = newHolder.value2Bar.color = Color.white;
        newHolder.value1Text.text = newHolder.value2Text.text = "";

        if (newHolder.graphHolder != null)
        {
            newHolder.widge[0].fillAmount = 1;
            newHolder.widge[1].fillAmount = 1;
            newHolder.widge[0].color = newHolder.widge[1].color = Color.white;

            newHolder.widge[0].transform.rotation = Quaternion.identity;
            newHolder.widge[1].transform.rotation = Quaternion.identity;
        }

        kinesthesiaRating = KinesthesiaRating.Default;

        provitionalValuesPie.Clear();

        yield return null;
    }

    //======================================================================
    //  RENDER: DIBUJA EL PIEGRAPH + BARRAS
    //======================================================================

    public override IEnumerator GraphMaker()
    {
        float total = provitionalValuesPie.Sum();

        if (total == 0)
            total = 1;

        float zRotation = 0f;

        // --- DIBUJAR PIE ---
        if (pieGraphHolder.graphHolder != null)
        {
            for (int i = 0; i < provitionalValuesPie.Count; i++)
            {
                Image wedge = pieGraphHolder.widge[i];
                wedge.gameObject.SetActive(true);
                wedge.color = pieGraphHolder.colorsPie[i];

                wedge.transform.localRotation = Quaternion.Euler(0, 0, zRotation);

                float targetFill = provitionalValuesPie[i] / total;

                yield return StartCoroutine(AnimateWedgeFill(wedge, targetFill, 0.8f));

                zRotation -= targetFill * 360f;
            }
        }

        // --- DIBUJAR BARRAS ---
        yield return StartCoroutine(
            AnimateBarFill(pieGraphHolder.colorsPie[0], pieGraphHolder.value1Bar,
                safePercent * barLength, pieGraphHolder.value1Text, safePercent, 0.8f)
        );

        yield return StartCoroutine(
            AnimateBarFill(pieGraphHolder.colorsPie[1], pieGraphHolder.value2Bar,
                dangerPercent * barLength, pieGraphHolder.value2Text, dangerPercent, 0.8f)
        );

        // --- MOSTRAR INTERPRETACIÓN ---
        ShowQualificationTag();
    }

    //======================================================================
    //  CALIFICACIÓN 
    //======================================================================

    public override void ShowQualificationTag()
    {

        string detailKey = "";

        switch(kinesthesiaRating) 
        {
            case KinesthesiaRating.Excellent:
                detailKey = "GesturesExcellent";
                Debug.Log(" Execelente movimiento de manos");
                break;
            case KinesthesiaRating.Good:
                detailKey = "GesturesGood";
                Debug.Log(" Buen movimiento de manos");
                break;
            case KinesthesiaRating.Low:
                detailKey = "GesturesAttention";
                Debug.Log(" Bajo movimiento de manos");
                break;
            default: detailKey = "GesturesDefault"; 
                break;
        } // 3️⃣ Obtener el texto traducido
            string feedbackText = LanguageManager.Instance.GetStringValue(detailKey);
        // 4️⃣ Mostrar el texto en pantalla
        if (pieGraphHolder != null && pieGraphHolder.interpretationText != null) 
        { 
            pieGraphHolder.interpretationText.gameObject.SetActive(true); 
            pieGraphHolder.interpretationText.text = feedbackText; Debug.Log($"[GesturesGraph] Mostrando feedback: {feedbackText}");
         } 
            else 
        { 
            Debug.LogWarning("[GesturesGraph] interpretationText no está asignado en el Inspector."); 
        } 
       
    }

    

    //======================================================================
    //  ANIMACIONES
    //======================================================================

    IEnumerator AnimateBarFill(Color color, Image bar, float targetFill, TMP_Text text, float percent, float duration)
    {
        bar.color = color;

        float elapsed = 0f;
        float initial = bar.fillAmount;
        float targetPercent = percent * percentMultiplier;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            bar.fillAmount = Mathf.Lerp(initial, targetFill, elapsed / duration);
            text.text = $"{Mathf.Round(Mathf.Lerp(initial * percentMultiplier, targetPercent, elapsed / duration))}%";

            yield return null;
        }

        bar.fillAmount = targetFill;
        text.text = $"{Mathf.Round(targetPercent)}%";
    }

    IEnumerator AnimateWedgeFill(Image wedge, float targetFill, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            wedge.fillAmount = Mathf.Lerp(0, targetFill, elapsed / duration);
            yield return null;
        }

        wedge.fillAmount = targetFill;
    }

    //======================================================================
    //  ░░░ 9. FINALIZACIÓN
    //======================================================================

    public override void EndGraph()
    {
        provitionalValuesPie.Clear();

        valueSafeMovement = 0;
        valueDangerMovement = 0;

        safePercent = 0;
        dangerPercent = 0;
    }
}
