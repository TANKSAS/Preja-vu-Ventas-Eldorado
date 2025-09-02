using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PieGraphController : Graph
{
    [SerializeField] float barLength = 1f;
    [SerializeField] float percentMultiplier = 100f;
    [SerializeField] PieGraphHolder pieGraphHolder;
    public List<float> provitionalValuesPie = new List<float>();

    float value1;
    float value2;
    float totalValue;
    float value1Percent;
    float value2Percent;

    public override void SetGraphSettings(GraphHolder graphHolder)
    {
        pieGraphHolder = ChooseGraph<PieGraphHolder>(graphHolder);
    }

    public override IEnumerator ResetGraphHolderValues(GraphHolder graphHolder)
    {
        PieGraphHolder newPieGraphHolder = ChooseGraph<PieGraphHolder>(graphHolder);
        //pieGraphHolder.graphName.text = string.Empty; 
        newPieGraphHolder.value1Bar.fillAmount = newPieGraphHolder.value2Bar.fillAmount = 0;
        newPieGraphHolder.value1Bar.color = newPieGraphHolder.value2Bar.color = UnityEngine.Color.white;
        newPieGraphHolder.value1Text.text = newPieGraphHolder.value2Text.text = string.Empty;

        if (newPieGraphHolder.graphHolder == null)
            yield break;

        newPieGraphHolder.widge[0].fillAmount = newPieGraphHolder.widge[1].fillAmount = 1;
        newPieGraphHolder.widge[0].color = newPieGraphHolder.widge[1].color = UnityEngine.Color.white;
        newPieGraphHolder.widge[0].GetComponent<RectTransform>().rotation = newPieGraphHolder.widge[1].GetComponent<RectTransform>().rotation = Quaternion.Euler(Vector3.zero);

        yield return null;
        Debug.Log("End Reset PieGraph");
    }

    public void SetValues(float newValue1, float newValue2)
    {
        provitionalValuesPie.Clear();
        value1 = value2 = 0;

        value1 = newValue1;
        value2 = newValue2;
        PercentPaiGraph();
    }

    public override IEnumerator GraphMaker()
    {
        float total = 0f;
        float zRotation = 0f;

        // Calcular el total de los valores del gráfico
        for (int i = 0; i < provitionalValuesPie.Count; i++)
        {
            total += provitionalValuesPie[i];
        }

        if (total == 0)
        {
            total = 1f;  // Evitar división por cero
        }

        if (pieGraphHolder.graphHolder != null)
        {
            // Crear cada wedge (sección) del gráfico
            for (int i = 0; i < provitionalValuesPie.Count; i++)
            {
                Image newWedge = pieGraphHolder.widge[i];
                newWedge.gameObject.SetActive(true);
                newWedge.color = pieGraphHolder.colorsPie[i];

                // Rotar el wedge
                newWedge.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, zRotation));

                // Calcular el nuevo valor de rotación para el siguiente wedge
                float targetFill = provitionalValuesPie[i] / total;

                // Llamar a la corutina para animar el relleno
                yield return StartCoroutine(AnimateWedgeFill(newWedge, targetFill, 0.8f)); // 1 segundo de animación (puedes ajustar)

                // Acumular la rotación para el siguiente wedge
                zRotation -= targetFill * 360f;
            }
        }

        yield return StartCoroutine(AnimateBarFill(pieGraphHolder.colorsPie[0], pieGraphHolder.value1Bar, value1Percent * barLength, pieGraphHolder.value1Text, value1Percent, 0.8f));

        yield return StartCoroutine(AnimateBarFill(pieGraphHolder.colorsPie[1], pieGraphHolder.value2Bar, value2Percent * barLength, pieGraphHolder.value2Text, value2Percent, 0.8f));
    }

    void PercentPaiGraph()
    {
        totalValue = value1 + value2;
        value1Percent = (value1 / totalValue);
        value2Percent = (value2 / totalValue);
        provitionalValuesPie.Add(value1Percent);
        provitionalValuesPie.Add(value2Percent);
    }

    IEnumerator AnimateBarFill(UnityEngine.Color color, Image bar, float targetFill, TMP_Text barText, double percentValue, float duration = 1f)
    {
        bar.color = color;
        float elapsedTime = 0f;
        float initialFill = bar.fillAmount;
        float targetPercent = (float)(percentValue * percentMultiplier);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            // Animar el relleno
            bar.fillAmount = Mathf.Lerp(initialFill, targetFill, elapsedTime / duration);
            // Actualizar el texto mientras se anima
            barText.text = $"{Mathf.Round(Mathf.Lerp(initialFill * percentMultiplier, targetPercent, elapsedTime / duration))}% ";
            yield return null;
        }

        // Asegurarse de que los valores finales sean correctos
        bar.fillAmount = targetFill;
        barText.text = $"{Mathf.Round(targetPercent)}%";
    }

    IEnumerator AnimateWedgeFill(Image wedge, float targetFill, float duration)
    {
        if (targetFill != 0)
        {
            float elapsedTime = 0f;
            float initialFill = 0f;  // Comenzamos con el wedge vacío

            while (elapsedTime < duration)
            {
                // Incrementar el tiempo transcurrido
                elapsedTime += Time.deltaTime;

                // Interpolar (lerp) entre el fillAmount inicial y el objetivo
                wedge.fillAmount = Mathf.Lerp(initialFill, targetFill, elapsedTime / duration);

                // Esperar hasta el próximo frame
                yield return null;
            }
        }

        // Asegurarse de que el fillAmount sea exactamente el valor objetivo
        wedge.fillAmount = targetFill;
    }

    public override void EndGraph()
    {
        value1 = 0;
        value2 = 0;
        totalValue = 0;
        value1Percent = 0;
        value2Percent = 0;
        provitionalValuesPie.Clear();
        pieGraphHolder = null;
    }
}