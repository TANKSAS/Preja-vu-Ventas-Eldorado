using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailsGraphController : Graph
{
    public PieGraphHolder pieGraphHolder;
    public VoiceGraphicHolder voiceGraphicHolder;
    public HeatMapGraphHolder heatMapGraphHolder;
    public PieGraphHolder heatMapPieGraphHolder;

    [SerializeField] PieGraphHolder newPieGraphHolder;
    [SerializeField] VoiceGraphicHolder newVoiceGraphicHolder;
    [SerializeField] HeatMapGraphHolder newHeatMapGraphHolder;
    
    public List<Tip> handsMoveTipsHolder;
    public List<Tip> visionTipsHolder;
    public List<Tip> gestureTipsHolder;
    public List<Tip> voiceTipsHolder;
    public List<Tip> heatMapTipsHolder;

    [SerializeField] bool isPieGraphDetails;
    [SerializeField] bool isVoiceGraphDetails;
    [SerializeField] bool isHeatMapGraphDetails;
  
    public override void SetGraphSettings(GraphHolder graph)
    {
        if (graph is PieGraphHolder)
        {
            if (isHeatMapGraphDetails)
            {
                Debug.Log("si es pieGraph de HeatMap");
                newPieGraphHolder = (PieGraphHolder)graph;
            }
            else
            {
                Debug.Log("si es pieGraph");
                newPieGraphHolder = (PieGraphHolder)graph;
                isPieGraphDetails = true;
            }
        }

        if (graph is VoiceGraphicHolder)
        {
            Debug.Log("si es VoiceGraph");
            newVoiceGraphicHolder = (VoiceGraphicHolder)graph;
            isVoiceGraphDetails = true;
        }

        if (graph is HeatMapGraphHolder)
        {
            newHeatMapGraphHolder = (HeatMapGraphHolder)graph;
            isHeatMapGraphDetails = true;
        }
    }

    public void ScrollToTop(ScrollRect scrollRect)
    {
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }

    public override IEnumerator ResetGraphHolderValues(GraphHolder graphHolder)
    {
        ResetTipsDetails();

        if (graphHolder is PieGraphHolder)
        {
            pieGraphHolder.graphName.text = string.Empty;
            yield return StartCoroutine(GraphManager.Instance.pieGraphController.ResetGraphHolderValues(pieGraphHolder));
            newPieGraphHolder = null;
            isPieGraphDetails = false;
        }
        
        if (graphHolder is VoiceGraphicHolder)
        {
            yield return StartCoroutine(GraphManager.Instance.voiceGraphController.ResetGraphHolderValues(voiceGraphicHolder));
            newVoiceGraphicHolder = null;
            isVoiceGraphDetails = false;
        }
        
        if (graphHolder is HeatMapGraphHolder)
        {
            yield return StartCoroutine(GraphManager.Instance.heatMapGraphController.ResetGraphHolderValues(heatMapGraphHolder));
            yield return StartCoroutine(GraphManager.Instance.pieGraphController.ResetGraphHolderValues(heatMapPieGraphHolder));
            newHeatMapGraphHolder = null;
            newPieGraphHolder = null;
            isHeatMapGraphDetails = false;
        }

        yield return null;
    }
    
    public override IEnumerator GraphMaker()
    {
        if (isPieGraphDetails)
        {
            yield return StartCoroutine(ShowPieDetails());
        }
        else if (isVoiceGraphDetails)
        {
            yield return StartCoroutine(ShowVoiceDetails());
        }
        else if (isHeatMapGraphDetails)
        {
            yield return StartCoroutine(ShowHeatMapDetails());
        }
        else
        {
           yield return null;
        }
    }

    IEnumerator ShowPieDetails()
    {
        RectTransform pieRectTransform;
        pieGraphHolder.graphName.text = newPieGraphHolder.graphName.text;
        pieGraphHolder.kindOfPieGraph = newPieGraphHolder.kindOfPieGraph;
        pieGraphHolder.value1NameText.text = newPieGraphHolder.value1NameText.text;
        pieGraphHolder.value2NameText.text = newPieGraphHolder.value2NameText.text;

        pieGraphHolder.widge[0].color = newPieGraphHolder.widge[0].color;
        pieGraphHolder.widge[0].fillAmount = newPieGraphHolder.widge[0].fillAmount;
        pieRectTransform = pieGraphHolder.widge[0].GetComponent<RectTransform>();
        pieRectTransform.localRotation = newPieGraphHolder.widge[0].GetComponent<RectTransform>().localRotation;

        pieGraphHolder.widge[1].color = newPieGraphHolder.widge[1].color;
        pieGraphHolder.widge[1].fillAmount = newPieGraphHolder.widge[1].fillAmount;
        pieRectTransform = pieGraphHolder.widge[1].GetComponent<RectTransform>();
        pieRectTransform.localRotation = newPieGraphHolder.widge[1].GetComponent<RectTransform>().localRotation;

        pieGraphHolder.value1Bar.color = newPieGraphHolder.colorsPie[0];
        pieGraphHolder.value1Bar.fillAmount = newPieGraphHolder.value1Bar.fillAmount;
        pieGraphHolder.value1Text.text = newPieGraphHolder.value1Text.text;

        pieGraphHolder.value2Bar.color = newPieGraphHolder.colorsPie[1];
        pieGraphHolder.value2Bar.fillAmount = newPieGraphHolder.value2Bar.fillAmount;
        pieGraphHolder.value2Text.text = newPieGraphHolder.value2Text.text;

        yield return new WaitForSeconds(0.1f);


        switch (pieGraphHolder.kindOfPieGraph)
        {
            case KindOfPieGraph.HandsMovePie:
                yield return StartCoroutine(ShowPieHandsMoveTips());
                break;

            case KindOfPieGraph.GestureHandsPie:
                yield return StartCoroutine(ShowPieGestureTips());
                break;
            
            case KindOfPieGraph.VisionPie:
                yield return StartCoroutine(ShowPieVisionMoveTips());
                break;
        }
    }

    IEnumerator ShowVoiceDetails()
    {
        for (int i = 0; i < newVoiceGraphicHolder.xLabelsObjects.Count; i++)
        {
            RectTransform poolXLabelObject = GraphManager.Instance.pool.GetOrCreateObjectPooled(i, voiceGraphicHolder.xLabelsObjects,voiceGraphicHolder.labelsHolder ,voiceGraphicHolder.labelTemplateX.gameObject).GetComponent<RectTransform>();
            RectTransform lastXLabelObject = newVoiceGraphicHolder.xLabelsObjects[i].GetComponent<RectTransform>();
           
            poolXLabelObject.anchoredPosition = lastXLabelObject.anchoredPosition;
            poolXLabelObject.GetComponent<TMP_Text>().text = lastXLabelObject.GetComponent<TMP_Text>().text;
            poolXLabelObject.gameObject.SetActive(lastXLabelObject.gameObject.activeSelf);
        }

        for (int i = 0; i < newVoiceGraphicHolder.xDotSeparationObjects.Count; i++)
        {
            RectTransform poolXDotSeparationObject = GraphManager.Instance.pool.GetOrCreateObjectPooled(i, voiceGraphicHolder.xDotSeparationObjects, voiceGraphicHolder.dotsHolder, voiceGraphicHolder.doteTemplateX.gameObject).GetComponent<RectTransform>();
            RectTransform lastXDotSeparationObject = newVoiceGraphicHolder.xDotSeparationObjects[i].GetComponent<RectTransform>();

            poolXDotSeparationObject.anchoredPosition = lastXDotSeparationObject.anchoredPosition;
            poolXDotSeparationObject.gameObject.SetActive(lastXDotSeparationObject.gameObject.activeSelf);
        }

        for (int i = 0; i < newVoiceGraphicHolder.xAxisObjects.Count; i++)
        {
            RectTransform poolXAxisObjectsObject = GraphManager.Instance.pool.GetOrCreateObjectPooled(i, voiceGraphicHolder.xAxisObjects, voiceGraphicHolder.axisHolder, voiceGraphicHolder.dashTemplateX.gameObject).GetComponent<RectTransform>();
            RectTransform lastXAxisObjectsObject = newVoiceGraphicHolder.xAxisObjects[i].GetComponent<RectTransform>();

            poolXAxisObjectsObject.anchoredPosition = lastXAxisObjectsObject.anchoredPosition;
            poolXAxisObjectsObject.gameObject.SetActive(lastXAxisObjectsObject.gameObject.activeSelf);
        }

        for (int i = 0; i < newVoiceGraphicHolder.yLabelsObjects.Count; i++)
        {
            RectTransform poolYLabelObject = GraphManager.Instance.pool.GetOrCreateObjectPooled(i, voiceGraphicHolder.yLabelsObjects, voiceGraphicHolder.labelsHolder, voiceGraphicHolder.labelTemplateY.gameObject).GetComponent<RectTransform>();
            RectTransform lastYLabelObject = newVoiceGraphicHolder.yLabelsObjects[i].GetComponent<RectTransform>();

            poolYLabelObject.anchoredPosition = lastYLabelObject.anchoredPosition;
            poolYLabelObject.GetComponent<TMP_Text>().text = lastYLabelObject.GetComponent<TMP_Text>().text;
            poolYLabelObject.gameObject.SetActive(lastYLabelObject.gameObject.activeSelf);
        }

        for (int i = 0; i < newVoiceGraphicHolder.yDotSeparationObjects.Count; i++)
        {
            RectTransform poolYDotSeparationObject = GraphManager.Instance.pool.GetOrCreateObjectPooled(i, voiceGraphicHolder.yDotSeparationObjects, voiceGraphicHolder.dotsHolder, voiceGraphicHolder.doteTemplateY.gameObject).GetComponent<RectTransform>();
            RectTransform lastYDotSeparationObject = newVoiceGraphicHolder.yDotSeparationObjects[i].GetComponent<RectTransform>();

            poolYDotSeparationObject.anchoredPosition = lastYDotSeparationObject.anchoredPosition;
            poolYDotSeparationObject.gameObject.SetActive(lastYDotSeparationObject.gameObject.activeSelf);
        }

        for (int i = 0; i < newVoiceGraphicHolder.yAxisObjects.Count; i++)
        {
            RectTransform poolYAxisObjectsObject = GraphManager.Instance.pool.GetOrCreateObjectPooled(i, voiceGraphicHolder.yAxisObjects, voiceGraphicHolder.axisHolder, voiceGraphicHolder.dashTemplateY.gameObject).GetComponent<RectTransform>();
            RectTransform lastYAxisObjectsObject = newVoiceGraphicHolder.yAxisObjects[i].GetComponent<RectTransform>();

            poolYAxisObjectsObject.anchoredPosition = lastYAxisObjectsObject.anchoredPosition;
            poolYAxisObjectsObject.gameObject.SetActive(lastYAxisObjectsObject.gameObject.activeSelf);
        }

        for (int i = 0; i < newVoiceGraphicHolder.objectPoolList.Count; i++)
        {
            RectTransform poolLineObject = GraphManager.Instance.pool.GetOrCreateObjectPooled(i, voiceGraphicHolder.objectPoolList, voiceGraphicHolder.linesHolder, voiceGraphicHolder.lineTemplate.gameObject).GetComponent<RectTransform>();
            RectTransform lastLineObject = newVoiceGraphicHolder.objectPoolList[i].GetComponent<RectTransform>();

            poolLineObject.anchorMin = new Vector2(0, 0);
            poolLineObject.anchorMax = new Vector2(0, 0);
            poolLineObject.sizeDelta = new Vector2(lastLineObject.sizeDelta.x, lastLineObject.sizeDelta.y);
            poolLineObject.anchoredPosition = lastLineObject.anchoredPosition;
            poolLineObject.localEulerAngles = lastLineObject.localEulerAngles;
            poolLineObject.GetComponent<RawImage>().color = lastLineObject.GetComponent<RawImage>().color;
            poolLineObject.gameObject.SetActive(lastLineObject.gameObject.activeSelf);
        }

        yield return StartCoroutine(ShowVoiceTips(GraphManager.Instance.currentToneOfVoice));
    }

    IEnumerator ShowHeatMapDetails()
    {
        heatMapGraphHolder.heatMapImage.GetComponent<Image>().color = Color.white;
        heatMapGraphHolder.heatMapImage.GetComponent<Image>().sprite = newHeatMapGraphHolder.heatMapImage.GetComponent<Image>().sprite;

        heatMapPieGraphHolder.value1NameText.text = newPieGraphHolder.value1NameText.text;
        heatMapPieGraphHolder.value2NameText.text = newPieGraphHolder.value2NameText.text;

        heatMapPieGraphHolder.value1Bar.color = newPieGraphHolder.colorsPie[0];
        heatMapPieGraphHolder.value1Bar.fillAmount = newPieGraphHolder.value1Bar.fillAmount;
        heatMapPieGraphHolder.value1Text.text = newPieGraphHolder.value1Text.text;
            
        heatMapPieGraphHolder.value2Bar.color = newPieGraphHolder.colorsPie[1];
        heatMapPieGraphHolder.value2Bar.fillAmount = newPieGraphHolder.value2Bar.fillAmount;
        heatMapPieGraphHolder.value2Text.text = newPieGraphHolder.value2Text.text;

        yield return StartCoroutine(ShowHeatMapTips());
    }

    IEnumerator ShowPieHandsMoveTips()
    {
        TMP_Text tip1 = UIManager.Instance.practicalResultsPieDetailTip1Text.GetComponent<TMP_Text>();
        tip1.text = string.Empty;
        TMP_Text tip2 = UIManager.Instance.practicalResultsPieDetailTip2Text.GetComponent<TMP_Text>();
        tip2.text = string.Empty;
        TMP_Text tip3 = UIManager.Instance.practicalResultsPieDetailTip3Text.GetComponent<TMP_Text>();
        tip3.text = string.Empty;

        //averageText.text = LanguageManager.Instance.GetStringValue("VoiceToneWeak");
        yield return new WaitForSeconds(.5f);

        tip1.text = LanguageManager.Instance.GetStringValue(handsMoveTipsHolder[0].tips[0]);
        tip1.gameObject.SetActive(true);
        yield return new WaitForSeconds(.5f);

        tip2.text = LanguageManager.Instance.GetStringValue(handsMoveTipsHolder[0].tips[1]);
        tip2.gameObject.SetActive(true);
        yield return new WaitForSeconds(.5f);

        tip3.text = LanguageManager.Instance.GetStringValue(handsMoveTipsHolder[0].tips[2]);
        tip3.gameObject.SetActive(true);
        yield return new WaitForSeconds(.5f);
    }
    
    IEnumerator ShowPieGestureTips()
    {
        TMP_Text tip1 = UIManager.Instance.practicalResultsPieDetailTip1Text.GetComponent<TMP_Text>();
        tip1.text = string.Empty;
        TMP_Text tip2 = UIManager.Instance.practicalResultsPieDetailTip2Text.GetComponent<TMP_Text>();
        tip2.text = string.Empty;
        TMP_Text tip3 = UIManager.Instance.practicalResultsPieDetailTip3Text.GetComponent<TMP_Text>();
        tip3.text = string.Empty;

        //averageText.text = LanguageManager.Instance.GetStringValue("VoiceToneWeak");
        yield return new WaitForSeconds(.5f);

        tip1.text = LanguageManager.Instance.GetStringValue(gestureTipsHolder[0].tips[0]);
        tip1.gameObject.SetActive(true);
        yield return new WaitForSeconds(.5f);

        tip2.text = LanguageManager.Instance.GetStringValue(gestureTipsHolder[0].tips[1]);
        tip2.gameObject.SetActive(true);
        yield return new WaitForSeconds(.5f);

        tip3.text = LanguageManager.Instance.GetStringValue(gestureTipsHolder[0].tips[2]);
        tip3.gameObject.SetActive(true);
        yield return new WaitForSeconds(.5f);
    }

    IEnumerator ShowPieVisionMoveTips()
    {
        TMP_Text tip1 = UIManager.Instance.practicalResultsPieDetailTip1Text.GetComponent<TMP_Text>();
        tip1.text = string.Empty;
        TMP_Text tip2 = UIManager.Instance.practicalResultsPieDetailTip2Text.GetComponent<TMP_Text>();
        tip2.text = string.Empty;
        TMP_Text tip3 = UIManager.Instance.practicalResultsPieDetailTip3Text.GetComponent<TMP_Text>();
        tip3.text = string.Empty;

        //averageText.text = LanguageManager.Instance.GetStringValue("VoiceToneWeak");
        yield return new WaitForSeconds(.5f);

        tip1.text = LanguageManager.Instance.GetStringValue(visionTipsHolder[0].tips[0]);
        tip1.gameObject.SetActive(true);
        yield return new WaitForSeconds(.5f);

        tip2.text = LanguageManager.Instance.GetStringValue(visionTipsHolder[0].tips[1]);
        tip2.gameObject.SetActive(true);
        yield return new WaitForSeconds(.5f);

        tip3.text = LanguageManager.Instance.GetStringValue(visionTipsHolder[0].tips[2]);
        tip3.gameObject.SetActive(true);
        yield return new WaitForSeconds(.5f);
    }

    IEnumerator ShowVoiceTips(ToneOfVoice kindOfVoice)
    {
        //TMP_Text averageText = UIManager.Instance.practicalResultsAverageText.GetComponent<TMP_Text>();
        //averageText.text = string.Empty;

        TMP_Text tip1 = UIManager.Instance.practicalResultsVoiceDetailTip1Text.GetComponent<TMP_Text>();
        tip1.text = string.Empty;

        TMP_Text tip2 = UIManager.Instance.practicalResultsVoiceDetailTip2Text.GetComponent<TMP_Text>();
        tip2.text = string.Empty;

        TMP_Text tip3 = UIManager.Instance.practicalResultsVoiceDetailTip3Text.GetComponent<TMP_Text>();
        tip3.text = string.Empty;

        switch (kindOfVoice)
        {
            case ToneOfVoice.WeakVoice:
               // averageText.text = LanguageManager.Instance.GetStringValue("VoiceToneWeak");
                yield return new WaitForSeconds(.5f);

                tip1.gameObject.SetActive(true);
                tip1.text = LanguageManager.Instance.GetStringValue(voiceTipsHolder[0].tips[0]);
                yield return new WaitForSeconds(.5f);

                tip2.gameObject.SetActive(true);
                tip2.text = LanguageManager.Instance.GetStringValue(voiceTipsHolder[0].tips[1]);
                yield return new WaitForSeconds(.5f);

                tip3.gameObject.SetActive(true);
                tip3.text = LanguageManager.Instance.GetStringValue(voiceTipsHolder[0].tips[2]);
                yield return new WaitForSeconds(.5f);
                break;

            case ToneOfVoice.ConversationalVoice:
               // averageText.text = LanguageManager.Instance.GetStringValue("VoiceToneConversational");
                yield return new WaitForSeconds(.5f);

                tip1.gameObject.SetActive(true);
                tip1.text = LanguageManager.Instance.GetStringValue(voiceTipsHolder[1].tips[0]);
                yield return new WaitForSeconds(.5f);

                tip2.gameObject.SetActive(true);
                tip2.text = LanguageManager.Instance.GetStringValue(voiceTipsHolder[1].tips[1]);
                yield return new WaitForSeconds(.5f);

                tip3.gameObject.SetActive(true);
                tip3.text = LanguageManager.Instance.GetStringValue(voiceTipsHolder[1].tips[2]);
                yield return new WaitForSeconds(.5f);
                break;

            case ToneOfVoice.ProjectedVoice:
               // averageText.text = LanguageManager.Instance.GetStringValue("VoiceToneProjected");
                yield return new WaitForSeconds(.5f);

                tip1.gameObject.SetActive(true);
                tip1.text = LanguageManager.Instance.GetStringValue(voiceTipsHolder[2].tips[0]);
                yield return new WaitForSeconds(1f);

                tip2.gameObject.SetActive(true);
                tip2.text = LanguageManager.Instance.GetStringValue(voiceTipsHolder[2].tips[1]);
                yield return new WaitForSeconds(1f);

                tip3.gameObject.SetActive(true);
                tip3.text = LanguageManager.Instance.GetStringValue(voiceTipsHolder[2].tips[2]);
                yield return new WaitForSeconds(1f);
                break;

            case ToneOfVoice.Screams:
               // averageText.text = LanguageManager.Instance.GetStringValue("VoiceToneShouting");
                yield return new WaitForSeconds(.5f);

                tip1.gameObject.SetActive(true);
                tip1.text = LanguageManager.Instance.GetStringValue(voiceTipsHolder[3].tips[0]);
                yield return new WaitForSeconds(1f);

                tip2.gameObject.SetActive(true);
                tip2.text = LanguageManager.Instance.GetStringValue(voiceTipsHolder[3].tips[1]);
                yield return new WaitForSeconds(1f);

                tip3.gameObject.SetActive(true);
                tip3.text = LanguageManager.Instance.GetStringValue(voiceTipsHolder[3].tips[2]);
                yield return new WaitForSeconds(1f);
                break;
        }

        yield return new WaitForSeconds(1f);
    }

    IEnumerator ShowHeatMapTips()
    {
        TMP_Text tip1 = UIManager.Instance.practicalResultsHeatMapDetailTip1Text.GetComponent<TMP_Text>();
        tip1.text = string.Empty;

        TMP_Text tip2 = UIManager.Instance.practicalResultsHeatMapDetailTip2Text.GetComponent<TMP_Text>();
        tip2.text = string.Empty;

        TMP_Text tip3 = UIManager.Instance.practicalResultsHeatMapDetailTip3Text.GetComponent<TMP_Text>();
        tip3.text = string.Empty;

        //averageText.text = LanguageManager.Instance.GetStringValue("VoiceToneWeak");
        yield return new WaitForSeconds(.5f);

        tip1.gameObject.SetActive(true);
        tip1.text = LanguageManager.Instance.GetStringValue(heatMapTipsHolder[0].tips[0]);
        yield return new WaitForSeconds(.5f);

        tip2.gameObject.SetActive(true);
        tip2.text = LanguageManager.Instance.GetStringValue(heatMapTipsHolder[0].tips[1]);
        yield return new WaitForSeconds(.5f);

        tip3.gameObject.SetActive(true);
        tip3.text = LanguageManager.Instance.GetStringValue(heatMapTipsHolder[0].tips[2]);
        yield return new WaitForSeconds(.5f);
    }

    void ResetTipsDetails()
    {
        if (isPieGraphDetails)
        {
            UIManager.Instance.practicalResultsPieDetailTip1Text.GetComponent<TMP_Text>().text = string.Empty;
            UIManager.Instance.practicalResultsPieDetailTip2Text.GetComponent<TMP_Text>().text = string.Empty;
            UIManager.Instance.practicalResultsPieDetailTip3Text.GetComponent<TMP_Text>().text = string.Empty;
            UIManager.Instance.practicalResultsPieDetailTip1Text.SetActive(false);
            UIManager.Instance.practicalResultsPieDetailTip2Text.SetActive(false);
            UIManager.Instance.practicalResultsPieDetailTip3Text.SetActive(false);
        }

        if (isHeatMapGraphDetails)
        {
            //UIManager.Instance.practicalResultsAverageText.GetComponent<TMP_Text>().text = string.Empty;
            UIManager.Instance.practicalResultsHeatMapDetailTip1Text.GetComponent<TMP_Text>().text = string.Empty;
            UIManager.Instance.practicalResultsHeatMapDetailTip2Text.GetComponent<TMP_Text>().text = string.Empty;
            UIManager.Instance.practicalResultsHeatMapDetailTip3Text.GetComponent<TMP_Text>().text = string.Empty;
            UIManager.Instance.practicalResultsHeatMapDetailTip1Text.SetActive(false);
            UIManager.Instance.practicalResultsHeatMapDetailTip2Text.SetActive(false);
            UIManager.Instance.practicalResultsHeatMapDetailTip3Text.SetActive(false);
        }

        if (isVoiceGraphDetails)
        {
            UIManager.Instance.practicalResultsVoiceDetailTip1Text.GetComponent<TMP_Text>().text = string.Empty;
            UIManager.Instance.practicalResultsVoiceDetailTip2Text.GetComponent<TMP_Text>().text = string.Empty;
            UIManager.Instance.practicalResultsVoiceDetailTip3Text.GetComponent<TMP_Text>().text = string.Empty;
            UIManager.Instance.practicalResultsVoiceDetailTip1Text.SetActive(false);
            UIManager.Instance.practicalResultsVoiceDetailTip2Text.SetActive(false);
            UIManager.Instance.practicalResultsVoiceDetailTip3Text.SetActive(false);
        }
    }

    public override void EndGraph()
    {
        isPieGraphDetails = isVoiceGraphDetails = isHeatMapGraphDetails = false;
    }

}
