using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GraphManager : Singleton<GraphManager>
{
    public VoiceGraphController voiceGraphController;
    public PieGraphController pieGraphController;
    public HeatMapGraphController heatMapGraphController;
    public ComparativeGraphController comparativeGraphController;
    public List<DetailsGraphController> detailsControllers = new List<DetailsGraphController>();

    public AnimacionUIManager animacionUIManager;
    public ObjectPool pool;

    public TryGraphHolder firstTryGraphHolder;
    public TryGraphHolder secondTryGraphHolder;
    public List<PieGraphHolder> pieGraphHolders;
    public List<VoiceGraphicHolder> voiceGraphHolders;
    public List<HeatMapGraphHolder> heatMapGraphHolders;
    public List<ComparativeGraphicHolder> comparativeGraphicHolders;

    public int currentSessionIndex;

    public void ShowPieGraph(int graphIndex, float value1, float value2)
    {
        pieGraphController.SetGraphSettings(pieGraphHolders[graphIndex]);
        pieGraphController.SetGraphParameters(currentSessionIndex);
        pieGraphController.SetValues(value1, value2);
    }

    void ShowVoiceGraph(int graphIndex)
    {
        voiceGraphController.SetGraphSettings(voiceGraphHolders[graphIndex]);
        voiceGraphController.SetGraphParameters(currentSessionIndex);
    }

    void ShowHeatMapGraph(int graphIndex)
    {
        heatMapGraphController.SetGraphSettings(heatMapGraphHolders[graphIndex]);
        heatMapGraphController.SetGraphParameters(currentSessionIndex);
    }

    public IEnumerator ResetGraph()
    {
        Debug.Log("Reset Graphs");

        for (int i = 0; i < detailsControllers.Count; i++)
        {
            detailsControllers[i].GraphCarouselButtonsEnable(false);
            detailsControllers[i].tipsAreReady = false;
            detailsControllers[i].guideLinesAreReady = false;
        }

        for (int i = 0; i < pieGraphHolders.Count; i++)
        {
            yield return StartCoroutine(pieGraphController.ResetGraphHolderValues(pieGraphHolders[i]));
            Debug.Log("End reset Pie # " + i);
        }

        for (int i = 0; i < heatMapGraphHolders.Count; i++)
        {
            yield return StartCoroutine(heatMapGraphController.ResetGraphHolderValues(heatMapGraphHolders[i]));
            Debug.Log("End reset HeatMap # " + i);
        }

        for (int i = 0; i < comparativeGraphicHolders.Count; i++)
        {
            yield return StartCoroutine(comparativeGraphController.ResetGraphHolderValues(comparativeGraphicHolders[i]));
            Debug.Log("End reset Comparative # " + i);
        }

        for (int i = 0; i < voiceGraphHolders.Count; i++)
        {
            yield return StartCoroutine(voiceGraphController.ResetGraphHolderValues(voiceGraphHolders[i]));
            Debug.Log("End reset Voice # " + i);
        }

        pieGraphController.EndGraph();
        voiceGraphController.EndGraph();
        heatMapGraphController.EndGraph();
        comparativeGraphController.EndGraph();

        GameObject[] graphicPanels =
        {
            firstTryGraphHolder.pieHandsMoveAnswersGraphicPanel,
            firstTryGraphHolder.voiceAnswersGraphicPanel,
            firstTryGraphHolder.heatMapAnswersGraphicPanel,
            secondTryGraphHolder.pieHandsMoveAnswersGraphicPanel,
            secondTryGraphHolder.voiceAnswersGraphicPanel,
            secondTryGraphHolder.heatMapAnswersGraphicPanel
        };

        foreach (var panel in graphicPanels)
        {
            panel.SetActive(false);

            var button = panel.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = false;
            }
        }
    }

    public IEnumerator StartFirstSessionResults()
    {
        UIManager.Instance.practicalResultsGraphicMenu.SetActive(true);
        UIManager.Instance.practicalResultsFristTryPanel.SetActive(true);
        UIManager.Instance.practicalResultsGraphicHeaderNavegationText.GetComponent<TMP_Text>().text = LanguageManager.Instance.GetStringValue("GraphAttemptTitleText01");

        currentSessionIndex = GameManager.Instance.playerStats.GetLastSessionIndex(KindOfAssessment.Diagnosis);
        yield return StartCoroutine(ShowFirstTryGraph());

        UIManager.Instance.practicalResultsGraphicMenuExitButton.SetActive(true);
    }

    public IEnumerator StartSessionResults()
    {
        if (GameManager.Instance.playerStats.sessions.Count == 0)
        {
            UIManager.Instance.practicalResultsGraphicMenuExitButton.SetActive(true);
            yield break;
        }

        currentSessionIndex = GameManager.Instance.playerStats.GetLastSessionIndex(KindOfAssessment.Diagnosis);
        yield return StartCoroutine(ShowFirstTryGraph());

        if (GameManager.Instance.playerStats.sessions.Count > 1)
        {
            UIManager.Instance.practicalResultsGraphicHeaderNavegation.SetActive(true);
            yield return new WaitUntil(() => UIManager.Instance.practicalResultsSecondTryPanel.activeInHierarchy);

            UIManager.Instance.practicalResultsGraphicHeaderNavegation.SetActive(false);

            currentSessionIndex = GameManager.Instance.playerStats.GetLastSessionIndex(KindOfAssessment.Interview);
            yield return StartCoroutine(ShowSecondTryGraph());

            UIManager.Instance.practicalResultsGraphicHeaderNavegation.SetActive(true);
            yield return new WaitUntil(() => UIManager.Instance.practicalResultsComparative.activeInHierarchy);

            UIManager.Instance.practicalResultsGraphicHeaderNavegation.SetActive(false);


            yield return StartCoroutine(ShowComparativeGraph());

            UIManager.Instance.practicalResultsGraphicHeaderNavegation.SetActive(true);
        }

        UIManager.Instance.practicalResultsGraphicMenuExitButton.SetActive(true);
    }


    public IEnumerator CallShowPieGraph(TryGraphHolder currentTryGraph, float value1, float value2, int pieGraphIndex)
    {
        //PieGraph
        // Paso 1: fade general del menú
        yield return StartCoroutine(animacionUIManager.FadeInElemento(UIManager.Instance.practicalResultsGraphicMenu));

        yield return new WaitForSeconds(0.6f);
        currentTryGraph.pieHandsMoveAnswersGraphicPanel.SetActive(true);
        ShowPieGraph(pieGraphIndex, value1, value2);
        yield return StartCoroutine(pieGraphController.GraphMaker());

        //aqui se activa la animacion del panel 

        Debug.Log("Inicia Movimiento");
        yield return StartCoroutine(animacionUIManager.AnimarElemento(currentTryGraph.pieHandsMoveAnswersGraphicPanel, "izquierda", 700f, -45f, -264f));

        Debug.Log("[GraphManager] Animación UI lanzada desde panel final");
    }

    IEnumerator ShowFirstTryGraph()
    {
        animacionUIManager.ResetearAnimaciones();
        //PieGraph
        yield return StartCoroutine(CallShowPieGraph(firstTryGraphHolder, GameManager.Instance.playerStats.sessions[currentSessionIndex].safeMovZone, GameManager.Instance.playerStats.sessions[currentSessionIndex].dangerMovZone, 0));
        detailsControllers[0].GraphCarouselButtonsEnable(true);

        //Voice 
        yield return StartCoroutine(animacionUIManager.FadeInElemento(firstTryGraphHolder.voiceAnswersGraphicPanel));

        firstTryGraphHolder.voiceAnswersGraphicPanel.SetActive(true);
        ShowVoiceGraph(0);
        yield return StartCoroutine(voiceGraphController.GraphMaker());
        detailsControllers[1].GraphCarouselButtonsEnable(true);

        //aqui se activa la animacion del panel 
        Debug.Log("Inicia Movimiento");
        yield return StartCoroutine(animacionUIManager.AnimarElemento(firstTryGraphHolder.voiceAnswersGraphicPanel, "derecha", 700f, 45f, -264f));


        //Vision
        yield return StartCoroutine(animacionUIManager.FadeInElemento(firstTryGraphHolder.heatMapAnswersGraphicPanel));

        firstTryGraphHolder.heatMapAnswersGraphicPanel.SetActive(true);
        ShowPieGraph(1, GameManager.Instance.playerStats.sessions[currentSessionIndex].visualSafeZone, GameManager.Instance.playerStats.sessions[currentSessionIndex].visualDangerZone);
        yield return StartCoroutine(pieGraphController.GraphMaker());

        ShowHeatMapGraph(0);
        yield return StartCoroutine(heatMapGraphController.GraphMaker());
        detailsControllers[2].GraphCarouselButtonsEnable(true);

        //aqui se activa la animacion del panel 
        Debug.Log("Inicia Movimiento");
        yield return StartCoroutine(animacionUIManager.AnimarElemento(firstTryGraphHolder.heatMapAnswersGraphicPanel, "quieto", 0f, 0f, 0f));
    }

    IEnumerator ShowSecondTryGraph()
    {
        //animacionUIManager.ResetearAnimaciones();

        Debug.Log("session #" + currentSessionIndex);
        yield return StartCoroutine(CallShowPieGraph(secondTryGraphHolder, GameManager.Instance.playerStats.sessions[currentSessionIndex].safeMovZone, GameManager.Instance.playerStats.sessions[currentSessionIndex].dangerMovZone, 2));
        detailsControllers[3].GraphCarouselButtonsEnable(true);

        //Voice 
        yield return StartCoroutine(animacionUIManager.FadeInElemento(secondTryGraphHolder.voiceAnswersGraphicPanel));

        secondTryGraphHolder.voiceAnswersGraphicPanel.SetActive(true);
        ShowVoiceGraph(1);
        yield return StartCoroutine(voiceGraphController.GraphMaker());
        detailsControllers[4].GraphCarouselButtonsEnable(true);

        //aqui se activa la animacion del panel 
        Debug.Log("Inicia Movimiento");
        yield return StartCoroutine(animacionUIManager.AnimarElemento(secondTryGraphHolder.voiceAnswersGraphicPanel, "derecha", 700f, 45f, -264f));


        //Vision
        yield return StartCoroutine(animacionUIManager.FadeInElemento(secondTryGraphHolder.heatMapAnswersGraphicPanel));

        secondTryGraphHolder.heatMapAnswersGraphicPanel.SetActive(true);
        ShowHeatMapGraph(1);
        yield return StartCoroutine(heatMapGraphController.GraphMaker());

        //aqui se activa la animacion del panel 
        Debug.Log("Inicia Movimiento");
        yield return StartCoroutine(animacionUIManager.AnimarElemento(secondTryGraphHolder.heatMapAnswersGraphicPanel, "quieto", 0f, 0f, 0f));


        ShowPieGraph(3, GameManager.Instance.playerStats.sessions[currentSessionIndex].visualSafeZone, GameManager.Instance.playerStats.sessions[currentSessionIndex].visualDangerZone);
        yield return StartCoroutine(pieGraphController.GraphMaker());
        detailsControllers[5].GraphCarouselButtonsEnable(true);
    }


    IEnumerator ShowComparativeGraph()
    {
        comparativeGraphController.SetGraphSettings(comparativeGraphicHolders[0]);

        comparativeGraphController.SetGraphParameters();
        StartCoroutine(comparativeGraphController.GraphMaker());

        ShowPieGraph(4, comparativeGraphController.overallPositiveFirstScore, comparativeGraphController.overallNegativeFirstScore);
        yield return StartCoroutine(pieGraphController.GraphMaker());

        ShowPieGraph(5, comparativeGraphController.overallPositiveSecondScore, comparativeGraphController.overallNegativeSecondScore);
        yield return StartCoroutine(pieGraphController.GraphMaker());
        //yield break;

    }

    public ToneOfVoiceRating CalculateVoiceQualification(List<float> readingResultsvoices)
    {
        if (readingResultsvoices == null || readingResultsvoices.Count == 0)
        {
            Debug.LogWarning("[VoiceGraph] No hay datos para calcular el promedio.");
            return ToneOfVoiceRating.Default;
        }

        float average;
        float maxAverage;
        float yGraphSize = 12;
        // 1️⃣ Calcular promedio
        average = 0;
        foreach (float value in readingResultsvoices)
            average += value;

        average /= readingResultsvoices.Count;
        Debug.Log($"[VoiceGraph] Promedio de tono: {average:F2}");

        // 2️⃣ Normalizar valores
        maxAverage = Mathf.RoundToInt(yGraphSize);
        float ratio = average / maxAverage;

        // 3️⃣ Determinar categoría según el ratio
        int toneIndex = 0;
        if (ratio < 0.25f) toneIndex = 0;          // Voz débil
        else if (ratio < 0.33f) toneIndex = 1;     // Conversacional
        else if (ratio < 0.5f) toneIndex = 2;      // Proyectada
        else toneIndex = 3;                        // Gritos

        // 4️⃣ Asignar valores dinámicamente
        ToneOfVoiceRating[] toneTypes = {
        ToneOfVoiceRating.WeakVoice,
        ToneOfVoiceRating.ConversationalVoice,
        ToneOfVoiceRating.ProjectedVoice,
        ToneOfVoiceRating.Screams
        };

        return toneTypes[toneIndex];
    }

    //Kinestessia 
    // 🔄 Versión restaurada (solo porcentajes)

    //Kinestesia
    public KinesthesiaRating CalculateKiesthesiaRating(float value1, float value2)
    {
        // 1️⃣ Validar datos
        if (value1 < 0f && value2 < 0f)
        {
            Debug.LogWarning("[Kinesthesia] Valores no válidos para la evaluación.");
            return KinesthesiaRating.Default;
        }

        // 2️⃣ Calcular la proporción de gestos dentro del área recomendada
        float total = value1 + value2;
        if (total <= 0f) total = 1f;

        float ratio = value1 / total;

        Debug.Log($"[Kinesthesia] Ratio de gestos dentro del área: {ratio:F2}");

        // 3️⃣ Determinar categoría según los rangos
        int kineIndex = 0;

        if (ratio < 0.40f)
            kineIndex = 0; // Bajo
        else if (ratio < 0.70f)
            kineIndex = 1; // Bueno
        else
            kineIndex = 2; // Excelente

        // 🔹 4️⃣ Devolver el tipo correspondiente
        KinesthesiaRating[] kinesthesiasTypes =
        {
        KinesthesiaRating.Low,
        KinesthesiaRating.Good,
        KinesthesiaRating.Excellent
    };

        return kinesthesiasTypes[kineIndex];
    }


    //HeatMap
    public HeatMapRating CalculateHeatMapRating(float visualSafeZone, float visualDangerZone)
    {
        if (visualSafeZone < 0f && visualDangerZone < 0f)
            return HeatMapRating.Default;

        float total = visualSafeZone + visualDangerZone;
        if (total <= 0f) total = 1f;

        float ratio = visualSafeZone / total;

        int heatIndex = 0;
        if (ratio < 0.5f)
            heatIndex = 0; // Bajo
        else if (ratio < 0.8f)
            heatIndex = 1; // Bueno
        else
            heatIndex = 2; // Excelente

        HeatMapRating[] heatMapTypes =
        {
        HeatMapRating.Low,
        HeatMapRating.Good,
        HeatMapRating.Excellent
    };

        return heatMapTypes[heatIndex];
    }
}