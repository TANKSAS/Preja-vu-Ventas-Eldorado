using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Graph : MonoBehaviour
{
    public abstract void SetGraphSettings(GraphHolder graphHolder);

    public abstract IEnumerator ResetGraphHolderValues(GraphHolder graphHolder);

    public virtual T ChooseGraph<T>(GraphHolder graphHolder) where T : GraphHolder
    {
        return graphHolder switch
        {
            T matchedGraph => matchedGraph,
            _ => null
        };
    }

    public abstract IEnumerator GraphMaker();

    public abstract void EndGraph();
}

[Serializable]
public abstract class GraphHolder
{

}

[Serializable]
public class TryGraphHolder: GraphHolder
{
    public GameObject pieHandsMoveAnswersGraphicPanel;
    public GameObject pieVisionAnswersGraphicPanel;
    public GameObject pieGestureAnswersGraphicPanel;
    public GameObject voiceAnswersGraphicPanel;
    public GameObject heatMapAnswersGraphicPanel;
}

[Serializable]
public class ComparativeGraphicHolder : GraphHolder
{
    public TMP_Text finalGradeText;
    public TMP_Text finalVerdictText;
    public GameObject finalGradePositiveIndicator;
    public GameObject finalGradeNegativeIndicator;
    public GameObject finalGradeIqualIndicator;

    public GameObject handsGesturesHolder;
    public GameObject handsGesturesPositiveIndicator;
    public GameObject handsGesturesNegativeIndicator;
    public GameObject handsGesturesIqualIndicator;
    public TMP_Text handsGesturesValueText;

    public GameObject handsMovementHolder;
    public GameObject handsMovementPositiveIndicator;
    public GameObject handsMovementNegativeIndicator;
    public GameObject handsMovementIqualIndicator;
    public TMP_Text handsMovementValueText;

    public GameObject eyesVisualMovHolder;
    public GameObject eyesVisualMovPositiveIndicator;
    public GameObject eyesVisualMovNegativeIndicator;
    public GameObject eyesVisualMovIqualIndicator;
    public TMP_Text eyesVisualMovValueText;
}

[Serializable]
public class VoiceGraphicHolder : GraphHolder
{
    public RectTransform graphContainer;
    public RectTransform lineTemplate;
    public RectTransform labelTemplateX;
    public RectTransform labelTemplateY;
    public RectTransform doteTemplateX;
    public RectTransform doteTemplateY;
    public RectTransform dashTemplateX;
    public RectTransform dashTemplateY;
    public GameObject labelsHolder;
    public GameObject dotsHolder;
    public GameObject axisHolder;
    public GameObject linesHolder;
    public GameObject ratingBoxColor;
    public TMP_Text titleGraphic;
    public TMP_Text ratingQualificationText;
    public List<GameObject> objectPoolList = new List<GameObject>();
    public List<GameObject> xLabelsObjects = new List<GameObject>();
    public List<GameObject> yLabelsObjects = new List<GameObject>();
    public List<GameObject> xDotSeparationObjects = new List<GameObject>();
    public List<GameObject> yDotSeparationObjects = new List<GameObject>();    
    public List<GameObject> xAxisObjects = new List<GameObject>();
    public List<GameObject> yAxisObjects = new List<GameObject>();

    public VoiceGraphicHolder(GameObject graphHolder)
    {
        graphContainer = graphHolder.transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).GetComponent<RectTransform>();

        labelsHolder = graphContainer.transform.GetChild(1).gameObject;
        dotsHolder = graphContainer.transform.GetChild(2).gameObject;
        axisHolder = graphContainer.transform.GetChild(3).gameObject;
        ratingBoxColor = graphContainer.transform.GetChild(4).transform.GetChild(0).transform.GetChild(0).gameObject;

        titleGraphic = graphHolder.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
        ratingQualificationText = graphContainer.transform.GetChild(5).transform.GetComponent<TMP_Text>();

        labelTemplateX = labelsHolder.transform.GetChild(0).GetComponent<RectTransform>();
        labelTemplateY = labelsHolder.transform.GetChild(1).GetComponent<RectTransform>();

        dashTemplateX = axisHolder.transform.GetChild(0).GetComponent<RectTransform>();
        dashTemplateY = axisHolder.transform.GetChild(1).GetComponent<RectTransform>();

        doteTemplateX = dotsHolder.transform.GetChild(0).GetComponent<RectTransform>();
        doteTemplateY = dotsHolder.transform.GetChild(1).GetComponent<RectTransform>();
    }
}

[Serializable]
public class PieGraphHolder : GraphHolder
{
    public TMP_Text graphName;
    public GameObject graphHolder;
    public Image[] widge;
    public TMP_Text value1NameText;
    public TMP_Text value1Text;
    public Image value1Bar;
    public TMP_Text value2NameText;
    public TMP_Text value2Text;
    public Image value2Bar;
    public Color[] colorsPie;
    public KindOfPieGraph kindOfPieGraph;
}

[Serializable]
public class HeatMapGraphHolder : GraphHolder
{
    public GameObject heatMapHolder;
    public GameObject heatMapImage;
    public Material heatMapMaterial;
    public Sprite heatMapSprite;
}

[Serializable]
public class Tip
{
    public string[] tips;
}