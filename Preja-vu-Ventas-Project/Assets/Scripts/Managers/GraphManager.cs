using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GraphManager : Singleton<GraphManager>
{
    public ToneOfVoice currentToneOfVoice;
    public VoiceGraphController voiceGraphController;
    public PieGraphController pieGraphController;
    public HeatMapGraphController heatMapGraphController;
    public ComparativeGraphController comparativeGraphController;
    public DetailsGraphController detailsGraphController;
    public ObjectPool pool;
    public TryGraphHolder firstTryGraphHolder;
    public TryGraphHolder secondTryGraphHolder;
    public List <PieGraphHolder> pieGraphHolders;
    public List <VoiceGraphicHolder> voiceGraphHolders;
    public List <HeatMapGraphHolder> heatMapGraphHolders;
    public List<ComparativeGraphicHolder> comparativeGraphicHolders;

    public void ShowPieGraph(int graphIndex, float value1, float value2)
    {
        pieGraphController.SetGraphSettings(pieGraphHolders[graphIndex]); 
        pieGraphController.SetValues(value1,value2);
    }

    void ShowVoiceGraph(int graphIndex, int sessionIndex)
    {
        voiceGraphController.SetGraphSettings(voiceGraphHolders[graphIndex]);
        voiceGraphController.SetListResults(GameManager.Instance.playerStats.sessions[sessionIndex].readingResultsvoices);
    }

    void ShowHeatMapGraph(int graphIndex, int sessionIndex)
    {
        heatMapGraphController.SetGraphSettings(heatMapGraphHolders[graphIndex]);
        heatMapGraphController.SetGraphImage(sessionIndex);
    }

    public IEnumerator ResetGraph()
    {
        Debug.Log("Reset Graphs");

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
            firstTryGraphHolder.pieVisionAnswersGraphicPanel,
            firstTryGraphHolder.pieGestureAnswersGraphicPanel,
            firstTryGraphHolder.voiceAnswersGraphicPanel,
            firstTryGraphHolder.heatMapAnswersGraphicPanel,
            secondTryGraphHolder.pieHandsMoveAnswersGraphicPanel,
            secondTryGraphHolder.pieVisionAnswersGraphicPanel,
            secondTryGraphHolder.pieGestureAnswersGraphicPanel,
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
        yield return StartCoroutine(ShowFirstTryGraph(GameManager.Instance.playerStats.lastSessionIndex));
        
        UIManager.Instance.practicalResultsGraphicMenuExitButton.SetActive(true);
    }

    public IEnumerator StartSessionResults()
    {
        Debug.Log("XD");

        if (GameManager.Instance.playerStats.sessions.Count == 0)
        {
            UIManager.Instance.practicalResultsGraphicMenuExitButton.SetActive(true);
            yield break;
        }

        int sessionIndex = GameManager.Instance.playerStats.lastSessionIndex - 1;
        sessionIndex = sessionIndex < 0 ? 0 : sessionIndex;

        yield return StartCoroutine(ShowFirstTryGraph(sessionIndex));
        
        Debug.Log("xxxxxx");


        if (GameManager.Instance.playerStats.sessions.Count > 1)
        {
            UIManager.Instance.practicalResultsGraphicHeaderNavegation.SetActive(true);
            yield return new WaitUntil(() => UIManager.Instance.practicalResultsSecondTryPanel.activeInHierarchy);
            
            UIManager.Instance.practicalResultsGraphicHeaderNavegation.SetActive(false);
            yield return StartCoroutine(ShowSecondTryGraph(GameManager.Instance.playerStats.lastSessionIndex));

            UIManager.Instance.practicalResultsGraphicHeaderNavegation.SetActive(true);
            yield return new WaitUntil(() => UIManager.Instance.practicalResultsComparative.activeInHierarchy);
            
            UIManager.Instance.practicalResultsGraphicHeaderNavegation.SetActive(false);
            yield return StartCoroutine(ShowComparativeGraph());
            
            UIManager.Instance.practicalResultsGraphicHeaderNavegation.SetActive(true);
        }

        UIManager.Instance.practicalResultsGraphicMenuExitButton.SetActive(true);

        Debug.Log("End");
    }

    public IEnumerator StartShowPieDetails(int pieGraphIndex)
    {
        detailsGraphController.SetGraphSettings(pieGraphHolders[pieGraphIndex]);
        yield return StartCoroutine(detailsGraphController.GraphMaker());
        
        yield return new WaitUntil(() => !UIManager.Instance.practicalResultsPieDetail.activeInHierarchy);
        yield return StartCoroutine(detailsGraphController.ResetGraphHolderValues(pieGraphHolders[pieGraphIndex]));
    }

    public IEnumerator StartShowVoiceDetails(int voiceGraphIndex)
    {
        detailsGraphController.SetGraphSettings(voiceGraphHolders[voiceGraphIndex]);
        yield return StartCoroutine(detailsGraphController.GraphMaker());

        yield return new WaitUntil(() => !UIManager.Instance.practicalResultsVoiceDetail.activeInHierarchy);
        yield return StartCoroutine(detailsGraphController.ResetGraphHolderValues(voiceGraphHolders[voiceGraphIndex]));
    }  
    
    public IEnumerator StartShowHeatMapDetails(int heatMapGraphIndex)
    {
        detailsGraphController.SetGraphSettings(heatMapGraphHolders[heatMapGraphIndex]);

        if (heatMapGraphIndex == 0)
        {
            detailsGraphController.SetGraphSettings(pieGraphHolders[3]);
        }
        else
        {
            detailsGraphController.SetGraphSettings(pieGraphHolders[7]);
        }

        yield return StartCoroutine(detailsGraphController.GraphMaker());
        yield return new WaitUntil(() => !UIManager.Instance.practicalResultsHeatMapDetail.activeInHierarchy);
        yield return StartCoroutine(detailsGraphController.ResetGraphHolderValues(heatMapGraphHolders[heatMapGraphIndex]));
    }

    IEnumerator ShowFirstTryGraph(int sessionIndex)
    {
        Debug.Log("session #" + sessionIndex);
        //PieGraph
        yield return new WaitForSeconds(0.6f);
        firstTryGraphHolder.pieHandsMoveAnswersGraphicPanel.SetActive(true);
        ShowPieGraph(0, GameManager.Instance.playerStats.sessions[sessionIndex].safeMovZone, GameManager.Instance.playerStats.sessions[sessionIndex].dangerMovZone);
        yield return StartCoroutine(pieGraphController.GraphMaker());
        firstTryGraphHolder.pieHandsMoveAnswersGraphicPanel.GetComponent<Button>().interactable = true;

        firstTryGraphHolder.pieVisionAnswersGraphicPanel.SetActive(true);
        ShowPieGraph(1, GameManager.Instance.playerStats.sessions[sessionIndex].visualSafeZone, GameManager.Instance.playerStats.sessions[sessionIndex].visualDangerZone);
        yield return StartCoroutine(pieGraphController.GraphMaker());
        firstTryGraphHolder.pieVisionAnswersGraphicPanel.GetComponent<Button>().interactable = true;

        firstTryGraphHolder.pieGestureAnswersGraphicPanel.SetActive(true);
        ShowPieGraph(2, GameManager.Instance.playerStats.sessions[sessionIndex].positiveGesture, GameManager.Instance.playerStats.sessions[sessionIndex].negativeGesture);
        yield return StartCoroutine(pieGraphController.GraphMaker());
        firstTryGraphHolder.pieGestureAnswersGraphicPanel.GetComponent<Button>().interactable = true;

        //Voice 
        firstTryGraphHolder.voiceAnswersGraphicPanel.SetActive(true);
        ShowVoiceGraph(0, sessionIndex);
        yield return StartCoroutine(voiceGraphController.GraphMaker());
        firstTryGraphHolder.voiceAnswersGraphicPanel.GetComponent<Button>().interactable = true;

        //Vision
        firstTryGraphHolder.heatMapAnswersGraphicPanel.SetActive(true);
        ShowPieGraph(3, GameManager.Instance.playerStats.sessions[sessionIndex].visualSafeZone, GameManager.Instance.playerStats.sessions[sessionIndex].visualDangerZone);
        yield return StartCoroutine(pieGraphController.GraphMaker());

        ShowHeatMapGraph(0, sessionIndex);
        yield return StartCoroutine(heatMapGraphController.GraphMaker());
        firstTryGraphHolder.heatMapAnswersGraphicPanel.GetComponent<Button>().interactable = true;

    }

    IEnumerator ShowSecondTryGraph(int sessionIndex)
    {
        Debug.Log("session #" + sessionIndex);
        //HandsMove
        secondTryGraphHolder.pieHandsMoveAnswersGraphicPanel.SetActive(true);
        ShowPieGraph(4, GameManager.Instance.playerStats.sessions[sessionIndex].safeMovZone, GameManager.Instance.playerStats.sessions[sessionIndex].dangerMovZone);
        yield return StartCoroutine(pieGraphController.GraphMaker());
        secondTryGraphHolder.pieHandsMoveAnswersGraphicPanel.GetComponent<Button>().interactable = true;

        //HandsMove
        secondTryGraphHolder.pieVisionAnswersGraphicPanel.SetActive(true);
        ShowPieGraph(5, GameManager.Instance.playerStats.sessions[sessionIndex].visualSafeZone, GameManager.Instance.playerStats.sessions[sessionIndex].visualDangerZone);
        yield return StartCoroutine(pieGraphController.GraphMaker());
        secondTryGraphHolder.pieVisionAnswersGraphicPanel.GetComponent<Button>().interactable = true;

        secondTryGraphHolder.pieGestureAnswersGraphicPanel.SetActive(true);
        ShowPieGraph(6, GameManager.Instance.playerStats.sessions[sessionIndex].positiveGesture, GameManager.Instance.playerStats.sessions[sessionIndex].negativeGesture);
        yield return StartCoroutine(pieGraphController.GraphMaker());
        secondTryGraphHolder.pieGestureAnswersGraphicPanel.GetComponent<Button>().interactable = true;

        //Voice 
        secondTryGraphHolder.voiceAnswersGraphicPanel.SetActive(true);
        ShowVoiceGraph(1, sessionIndex);
        yield return StartCoroutine(voiceGraphController.GraphMaker());
        secondTryGraphHolder.voiceAnswersGraphicPanel.GetComponent<Button>().interactable = true;

        //Vision
        secondTryGraphHolder.heatMapAnswersGraphicPanel.SetActive(true);
        ShowHeatMapGraph(1, sessionIndex);
        yield return StartCoroutine(heatMapGraphController.GraphMaker());
        secondTryGraphHolder.heatMapAnswersGraphicPanel.GetComponent<Button>().interactable = true;

        ShowPieGraph(7, GameManager.Instance.playerStats.sessions[sessionIndex].visualSafeZone, GameManager.Instance.playerStats.sessions[sessionIndex].visualDangerZone);
        yield return StartCoroutine(pieGraphController.GraphMaker());
    }

    IEnumerator ShowComparativeGraph()
    {
        comparativeGraphController.SetValues();
        yield return StartCoroutine(comparativeGraphController.GraphMaker());
        
        ShowPieGraph(8, comparativeGraphController.overallPositiveFirstScore, comparativeGraphController.overallNegativeFirstScore);
        yield return StartCoroutine(pieGraphController.GraphMaker());
        
        ShowPieGraph(9, comparativeGraphController.overallPositiveSecondScore, comparativeGraphController.overallNegativeSecondScore);
        yield return StartCoroutine(pieGraphController.GraphMaker());
    }
}