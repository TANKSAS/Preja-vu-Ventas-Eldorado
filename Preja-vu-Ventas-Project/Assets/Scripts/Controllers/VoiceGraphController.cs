using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;


public class VoiceGraphController: Graph
{
    public VoiceGraphicHolder voiceGraphHolder;
    [SerializeField] string[] yAxisPostionsList;
    [SerializeField] List<float> provitionalReadingResultsVoice = new List<float>();
    [SerializeField] Color32[] kindOfVoiceColor;
    Color graphColor = new Color(0f, 229f / 255f, 1f);

    float graphicHeight;
    float graphicWidth;
    float yMaximum;
    float xSize;
    float videoDurationInSeconds;
    float separatorDotsCount;
    float dotXDistance;
    float dotYDistance;
    int separatorXCount;
    int separatorYCount;
    int maxAverage;
    float average;

    public override void SetGraphSettings(GraphHolder graphHolder)
    {
        voiceGraphHolder = ChooseGraph<VoiceGraphicHolder>(graphHolder);
    }

    public void SetListResults(List<float> resultsValuesList)
    {
        provitionalReadingResultsVoice = new List<float>(resultsValuesList);
    }

    public override IEnumerator ResetGraphHolderValues(GraphHolder graphHolder)
    {
        voiceGraphHolder = ChooseGraph<VoiceGraphicHolder>(graphHolder);

        for (int i = 0; i < voiceGraphHolder.xLabelsObjects.Count; i++)
        {
            voiceGraphHolder.xLabelsObjects[i].gameObject.SetActive(false);
            voiceGraphHolder.xLabelsObjects[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            voiceGraphHolder.xLabelsObjects[i].GetComponent<TMP_Text>().text = string.Empty;
        }

        for (int i = 0; i < voiceGraphHolder.xDotSeparationObjects.Count; i++)
        {
            voiceGraphHolder.xDotSeparationObjects[i].gameObject.SetActive(false);
            voiceGraphHolder.xDotSeparationObjects[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }

        for (int i = 0; i < voiceGraphHolder.xAxisObjects.Count; i++)
        {
            voiceGraphHolder.xAxisObjects[i].gameObject.SetActive(false);
            voiceGraphHolder.xAxisObjects[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }

        for (int i = 0; i < voiceGraphHolder.yLabelsObjects.Count; i++)
        {
            voiceGraphHolder.yLabelsObjects[i].gameObject.SetActive(false);
            voiceGraphHolder.yLabelsObjects[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            voiceGraphHolder.yLabelsObjects[i].GetComponent<TMP_Text>().text = string.Empty; 
        }

        for (int i = 0; i < voiceGraphHolder.yDotSeparationObjects.Count; i++)
        {
            voiceGraphHolder.yDotSeparationObjects[i].gameObject.SetActive(false);
            voiceGraphHolder.yDotSeparationObjects[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }

        for (int i = 0; i < voiceGraphHolder.yAxisObjects.Count; i++)
        {
            voiceGraphHolder.yAxisObjects[i].gameObject.SetActive(false);
            voiceGraphHolder.yAxisObjects[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }

        for (int i = 0; i < voiceGraphHolder.objectPoolList.Count; i++)
        {
            voiceGraphHolder.objectPoolList[i].gameObject.SetActive(false);
            voiceGraphHolder.objectPoolList[i].GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            voiceGraphHolder.objectPoolList[i].GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
            voiceGraphHolder.objectPoolList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            voiceGraphHolder.objectPoolList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            voiceGraphHolder.objectPoolList[i].GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, 0);
            voiceGraphHolder.objectPoolList[i].GetComponent<RawImage>().color = Color.white;
        }

        voiceGraphHolder = null;
        yield return null;
        Debug.Log("End Reset VoiceGraph");
    }

    public override IEnumerator GraphMaker()
    {
        //voiceGraphHolder.titleGraphic.text = LanguageManager.Instance.GetStringValue("FinalTestQuestionEnumText") + " #"; 
        videoDurationInSeconds = (float) GameManager.Instance.backGroundController.currentVideoPlayer.length;
        
        if (videoDurationInSeconds <= 0)
        {
            Debug.LogError("Video dura 0 segundos revisar referencia del video");
            yield break;
        }

        graphicHeight = voiceGraphHolder.graphContainer.sizeDelta.y;
        graphicWidth = voiceGraphHolder.graphContainer.sizeDelta.x;
        yMaximum = 12;
        separatorYCount = 6;
        xSize = graphicWidth / provitionalReadingResultsVoice.Count;
        separatorXCount = Mathf.CeilToInt(videoDurationInSeconds / 60f);
        separatorDotsCount = 8f;


        Debug.Log("Duracion del video " + videoDurationInSeconds / 60);
        Debug.Log("Numero de partes en que se divide el size X completo " + separatorXCount);

        CreateLabels();

        Vector2 lastPointPosition = Vector2.zero;

        for (int i = 0; i < provitionalReadingResultsVoice.Count; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = (provitionalReadingResultsVoice[i] / yMaximum) * graphicHeight;
            Vector2 startPointPosition = new Vector2(xPosition, yPosition);
            //// Usa el método de object pooling para obtener o crear un objeto

            GameObject lastPool = GraphManager.Instance.pool.GetOrCreateObjectPooled(i, voiceGraphHolder.objectPoolList, voiceGraphHolder.linesHolder, voiceGraphHolder.lineTemplate.gameObject);

            if (lastPointPosition != Vector2.zero)
            {
                lastPool.gameObject.SetActive(true);
                CreateDotConection(lastPool, lastPointPosition, startPointPosition);
                yield return new WaitForSeconds(.05f);
            }

            lastPointPosition = startPointPosition;
        }
        
        CalculateAverage();
        Debug.Log("EndGraph");
    }

    void CalculateAverage()
    {
        float sum = 0;

        foreach (float number in provitionalReadingResultsVoice)
        {
            sum += number;
        }

        average = sum / provitionalReadingResultsVoice.Count;
        Debug.Log("The average of the numbers is: " + average);

        if (average > 0 && average < maxAverage / 4)
        {
            GraphManager.Instance.currentToneOfVoice = ToneOfVoice.WeakVoice;
            voiceGraphHolder.ratingBoxColor.GetComponent<RawImage>().color = kindOfVoiceColor[0];
            voiceGraphHolder.ratingQualificationText.text = LanguageManager.Instance.GetStringValue("FrequencyRatingLowText");
        }
        else if (average > maxAverage / 4 && average < maxAverage / 3)
        {
            GraphManager.Instance.currentToneOfVoice = ToneOfVoice.ConversationalVoice;
            voiceGraphHolder.ratingBoxColor.GetComponent<RawImage>().color = kindOfVoiceColor[1];
            voiceGraphHolder.ratingQualificationText.text = LanguageManager.Instance.GetStringValue("FrequencyRatingMediumText");
        }
        else if (average > maxAverage / 3 && average < maxAverage / 2)
        {
            GraphManager.Instance.currentToneOfVoice = ToneOfVoice.ProjectedVoice;
            voiceGraphHolder.ratingBoxColor.GetComponent<RawImage>().color = kindOfVoiceColor[2];
            voiceGraphHolder.ratingQualificationText.text = LanguageManager.Instance.GetStringValue("FrequencyRatingPerfectText");
        }
        else if (average > maxAverage)
        {
            GraphManager.Instance.currentToneOfVoice = ToneOfVoice.Screams;
            voiceGraphHolder.ratingBoxColor.GetComponent<RawImage>().color = kindOfVoiceColor[3];
            voiceGraphHolder.ratingQualificationText.text = LanguageManager.Instance.GetStringValue("FrequencyRatingHightText");
        }
    }

    void CreateLabels()
    {
        float startTimeIndex = 0f;
        float widthLabelX = voiceGraphHolder. labelTemplateX.rect.width;
        float widthLabelY = voiceGraphHolder.labelTemplateY.rect.height;
        Debug.Log("Ancho del borde Label X" + widthLabelX);
        Debug.Log("Ancho del borde Label Y" + widthLabelY);
        float xSizeDistance = 1f / separatorXCount * graphicWidth;
        float ySizeDistance = 1f / separatorYCount * graphicHeight;
        Debug.Log("Distancia de cada parte en que se divide el size X completo " + xSizeDistance);
        dotXDistance = (xSizeDistance - widthLabelX) / separatorDotsCount;
        dotYDistance = (ySizeDistance - widthLabelY) / separatorDotsCount;
        Debug.Log("Distancia de cada punto de cada parte en que se divide el size X completo " + dotXDistance);

        for (int i = 0; i < separatorXCount + 1; i++)
        {
            RectTransform labelX = GraphManager.Instance.pool.GetOrCreateObjectPooled(i,voiceGraphHolder.xLabelsObjects, voiceGraphHolder.labelsHolder, voiceGraphHolder.labelTemplateX.gameObject).GetComponent<RectTransform>();
            float normalizedValue = i * 1f / separatorXCount;
            float distanceNormalizedValue = normalizedValue * graphicWidth;
            labelX.anchoredPosition = new Vector2(distanceNormalizedValue - 10f, -30);
            labelX.gameObject.SetActive(true);

            // Convertir a minutos y segundos
            int minutes = Mathf.FloorToInt(startTimeIndex / 60f);
            int seconds = Mathf.FloorToInt(startTimeIndex % 60f);

            // Formatear el texto
            string formattedTime;

            if (seconds != 0)
            {
                formattedTime = string.Format("{0}.{1}m", minutes + i, seconds);
            }
            else
            {
                formattedTime = string.Format("{0}m", minutes + i);
            }

            if (!labelX.GetComponent<TMP_Text>())
            {
                labelX.AddComponent<TMP_Text>();
            }
            
            labelX.GetComponent<TMP_Text>().text = formattedTime;

            RectTransform dashX = GraphManager.Instance.pool.GetOrCreateObjectPooled(i,voiceGraphHolder.xAxisObjects, voiceGraphHolder.axisHolder, voiceGraphHolder.dashTemplateX.gameObject).GetComponent<RectTransform>();
            dashX.anchoredPosition = new Vector2(distanceNormalizedValue, 0f);
            dashX.gameObject.SetActive(true);

            if (i != separatorXCount)
            {
                for (int j = 0; j < separatorDotsCount; j++)
                {
                    RectTransform labelDotSeparatorX = GraphManager.Instance.pool.GetOrCreateObjectPooled(j,voiceGraphHolder.xDotSeparationObjects, voiceGraphHolder.dotsHolder, voiceGraphHolder.doteTemplateX.gameObject).GetComponent<RectTransform>();
                    float startPoint = labelX.anchoredPosition.x + widthLabelX;
                    float dotPositionX = dotXDistance * j;
                    labelDotSeparatorX.anchoredPosition = new Vector2(startPoint + dotPositionX, -30f);
                    labelDotSeparatorX.gameObject.SetActive(true);
                }
            }
        }

        for (int i = 0; i < separatorYCount + 1; i++)
        {
            RectTransform labelY = GraphManager.Instance.pool.GetOrCreateObjectPooled(i, voiceGraphHolder.yLabelsObjects, voiceGraphHolder.labelsHolder, voiceGraphHolder.labelTemplateY.gameObject).GetComponent<RectTransform>();
            labelY.gameObject.SetActive(true);
            
            float normalizedValue = i * 1f / separatorYCount;
            float distanceNormalizedValue = normalizedValue * graphicHeight;
            labelY.anchoredPosition = new Vector2(-50f, distanceNormalizedValue);

            if (!labelY.GetComponent<TMP_Text>())
            {
                Debug.Log("No tiene");
                labelY.AddComponent<TMP_Text>();
            }

            labelY.GetComponent<TMP_Text>().text = yAxisPostionsList[i] + " dB";

            RectTransform dashY = GraphManager.Instance.pool.GetOrCreateObjectPooled(i, voiceGraphHolder.yAxisObjects, voiceGraphHolder.axisHolder, voiceGraphHolder.dashTemplateY.gameObject).GetComponent<RectTransform>(); 
            dashY.anchoredPosition = new Vector2(0, distanceNormalizedValue);
            dashY.gameObject.SetActive(true);

            if (i != separatorYCount)
            {
                for (int j = 0; j < separatorDotsCount; j++)
                {
                    RectTransform labelDotSeparatorY = GraphManager.Instance.pool.GetOrCreateObjectPooled(j,voiceGraphHolder.yDotSeparationObjects, voiceGraphHolder.linesHolder, voiceGraphHolder.doteTemplateY.gameObject).GetComponent<RectTransform>();

                    float startPoint = labelY.anchoredPosition.y + widthLabelY + .1f;
                    float dotPositionY = dotYDistance * j;

                    labelDotSeparatorY.anchoredPosition = new Vector2(-50f, startPoint + dotPositionY);
                    labelDotSeparatorY.GetComponent<TMP_Text>().text = ".";
                    labelDotSeparatorY.gameObject.SetActive(true);
                }
            }
        }
    }

    void CreateDotConection(GameObject newGameObject, Vector2 dotPositionA, Vector2 dotPositionB)
    {
        if (!newGameObject.GetComponent<RawImage>())
        {
            newGameObject.AddComponent<RawImage>();
        }

        newGameObject.GetComponent<RawImage>().color = graphColor;
        
        RectTransform rectTransform = newGameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 2f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
    }

    float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public override void EndGraph()
    {
        graphicHeight = 0;
        graphicWidth = 0;
        yMaximum = 0;
        xSize = 0;
        videoDurationInSeconds = 0;
        separatorDotsCount = 0;
        dotXDistance = 0;
        dotYDistance = 0;
        separatorXCount = 0;
        separatorYCount = 0;
        maxAverage = 0;
        average = 0;
        provitionalReadingResultsVoice.Clear();
    }
}