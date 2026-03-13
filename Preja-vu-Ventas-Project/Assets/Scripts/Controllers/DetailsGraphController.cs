using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class DetailsGraphController : MonoBehaviour
{
    // =============================
    //  ESTADOS INTERNOS
    // =============================

    // 🔒 Evita que el carrusel se mueva mientras la gráfica se construye
    //public bool graphIsReady = false;

    // Controla si los tips y detalles ya fueron generados
    public bool tipsAreReady;
    public bool guideLinesAreReady;
    public bool isTransitioning;

    // =============================
    //   REFERENCIAS
    // =============================

    public AnimacionUIManager animacionUIManager;

    [Header("Texto - TIPS")]
    public TMP_Text tip1;
    public TMP_Text tip2;
    public TMP_Text tip3;

    [Header("Texto - DETALLES")]
    public TMP_Text qualificationDetails;

    [Header("Todos los TIPS agrupados")]
    public List<Tip> tipsHolder;

    [Header("Paneles del carrusel")]
    public List<GameObject> subPanelsList = new List<GameObject>();
    public GameObject NavigationBackButton;
    public GameObject NavigationContinueButton;

    [Header("Tipo de gráfica activa")]
    public bool isPieGraphDetails;
    public bool isVoiceGraphDetails;
    public bool isHeatMapGraphDetails;

    [Header("Título superior dinámico")]
    public TMP_Text titleText;

    // Índice del carrusel
    public int currentCarouselIndex = 0;


    // ============================================================
    //   INICIALIZACIÓN : SE EJECUTA AL ACTIVAR EL PANEL COMPLETO
    // ============================================================
    void OnEnable()
    {
        // Reset panels
        InitializePanels(subPanelsList);

        // Asegurar que el panel inicial tenga su título cargado
        currentCarouselIndex = 0;
        UpdateInitialTitle();
    }


    // =============================
    //  INICIALIZAR PANELES
    // =============================
    private void InitializePanels(List<GameObject> panels)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            if (panels[i] != null)
                panels[i].SetActive(i == 0); // Solo activar el primer panel
        }
    }

    private void UpdateInitialTitle()
    {
        if (titleText != null)
            titleText.text = GetCurrentTitle(0);
    }

    // ============================================================
    //      GENERACIÓN DE DETALLES — PRIMER PANEL DEL CARRUSEL
    // ============================================================
    public void GraphGuidelinesMaker()
    {
        GraphCarouselButtonsEnable(false);

        if (isPieGraphDetails)
            ShowPieQualificationDetails();
        else if (isVoiceGraphDetails)
            ShowVoiceQualificationDetails();
        else if (isHeatMapGraphDetails)
            ShowHeatMapQualificationDetails();
        GraphCarouselButtonsEnable(true);
        guideLinesAreReady = true;
    }


    // ---------- PIE GRAPH DETALLES ----------
    void ShowPieQualificationDetails()
    {
        string detailKey = "";
        KinesthesiaRating kinesthesiaRating =
            GameManager.Instance.playerStats.sessions[GraphManager.Instance.currentSessionIndex].kinesthesiaRating;

        switch (kinesthesiaRating)
        {
            case KinesthesiaRating.Excellent: detailKey = "GesturesDetailExcellent"; break;
            case KinesthesiaRating.Good: detailKey = "GesturesDetailGood"; break;
            case KinesthesiaRating.Low: detailKey = "GesturesDetailAttention"; break;
        }

        qualificationDetails.text = LanguageManager.Instance.GetStringValue(detailKey);
    }


    // ---------- VOICE GRAPH DETALLES ----------
    public void ShowVoiceQualificationDetails()
    {
        string detailKey = "";
        ToneOfVoiceRating voiceRating =
            GameManager.Instance.playerStats.sessions[GraphManager.Instance.currentSessionIndex].toneOfVoiceRating;

        switch (voiceRating)
        {
            case ToneOfVoiceRating.WeakVoice: detailKey = "FrequencyRatingLowText"; break;
            case ToneOfVoiceRating.ConversationalVoice: detailKey = "FrequencyRatingMediumText"; break;
            case ToneOfVoiceRating.ProjectedVoice: detailKey = "FrequencyRatingPerfectText"; break;
            case ToneOfVoiceRating.Screams: detailKey = "FrequencyRatingHightText"; break;
        }

        qualificationDetails.text = LanguageManager.Instance.GetStringValue(detailKey);
        //yield return null;
    }


    // ---------- HEATMAP GRAPH DETALLES ----------
   public void ShowHeatMapQualificationDetails()
    {
        string detailKey = "";
        HeatMapRating heatRating =
            GameManager.Instance.playerStats.sessions[GraphManager.Instance.currentSessionIndex].heatMapRating;

        switch (heatRating)
        {
            case HeatMapRating.Excellent: detailKey = "HeatMapDetailExcellent"; break;
            case HeatMapRating.Good: detailKey = "HeatMapDetailGood"; break;
            case HeatMapRating.Low: detailKey = "HeatMapDetailLow"; break;
        }

        qualificationDetails.text = LanguageManager.Instance.GetStringValue(detailKey);
       
    }


    // ============================================================
    //      GENERACIÓN DE TIPS — TERCER PANEL DEL CARRUSEL
    // ============================================================
    public void GraphTipsMaker()
    {
        GraphCarouselButtonsEnable(false);
        
        if (isPieGraphDetails)
          ShowPieHandsMoveTips(GameManager.Instance.playerStats.sessions[GraphManager.Instance.currentSessionIndex].kinesthesiaRating);
        else if (isVoiceGraphDetails)
          ShowVoiceTips(
                GameManager.Instance.playerStats.sessions[GraphManager.Instance.currentSessionIndex].toneOfVoiceRating);
        else if (isHeatMapGraphDetails)
           ShowHeatMapTips(GameManager.Instance.playerStats.sessions[GraphManager.Instance.currentSessionIndex].heatMapRating);

        GraphCarouselButtonsEnable(true);
        tipsAreReady = true;
    }


    // ---------- TIPS MANOS ----------
    void ShowPieHandsMoveTips(KinesthesiaRating rating)
    {
        ResetTipsDetails();

         int index = rating switch
        {
            KinesthesiaRating.Low => 0,
            KinesthesiaRating.Good => 1,
            KinesthesiaRating.Excellent => 2,
            KinesthesiaRating.Exaggerated => 3, 
            _ => 0
        };

        tip1.text = LanguageManager.Instance.GetStringValue(tipsHolder[index].tips[0]);
        tip1.gameObject.SetActive(true);
        tip2.text = LanguageManager.Instance.GetStringValue(tipsHolder[index].tips[1]);
        tip3.gameObject.SetActive(true);
        tip3.text = LanguageManager.Instance.GetStringValue(tipsHolder[index].tips[2]);
        tip2.gameObject.SetActive(true);
    }


    // ---------- TIPS VOZ ----------
    void  ShowVoiceTips(ToneOfVoiceRating rating)
    {
        ResetTipsDetails();
       // yield return new WaitForSeconds(.3f);

        int index = rating switch
        {
            ToneOfVoiceRating.WeakVoice => 0,
            ToneOfVoiceRating.ConversationalVoice => 1,
            ToneOfVoiceRating.ProjectedVoice => 2,
            ToneOfVoiceRating.Screams => 3,
            _ => 0
        };

        tip1.text = LanguageManager.Instance.GetStringValue(tipsHolder[index].tips[0]);
        tip2.text = LanguageManager.Instance.GetStringValue(tipsHolder[index].tips[1]);
        tip3.text = LanguageManager.Instance.GetStringValue(tipsHolder[index].tips[2]);

        tip1.gameObject.SetActive(true);
       // yield return new WaitForSeconds(.3f);
        tip2.gameObject.SetActive(true);
       // yield return new WaitForSeconds(.3f);
        tip3.gameObject.SetActive(true);
    }


    // ---------- TIPS HEATMAP ----------
   void ShowHeatMapTips(HeatMapRating rating)
    {
        ResetTipsDetails();
       // yield return new WaitForSeconds(.3f);

        int index = rating switch
        {
            HeatMapRating.Low => 0,
            HeatMapRating.Good => 1,
            HeatMapRating.Excellent => 2,
            _ => 0
        };

        tip1.text = LanguageManager.Instance.GetStringValue(tipsHolder[index].tips[0]);
        tip2.text = LanguageManager.Instance.GetStringValue(tipsHolder[index].tips[1]);
        tip3.text = LanguageManager.Instance.GetStringValue(tipsHolder[index].tips[2]);

        tip1.gameObject.SetActive(true);
       // yield return new WaitForSeconds(.3f);
        tip2.gameObject.SetActive(true);
       // yield return new WaitForSeconds(.3f);
        tip3.gameObject.SetActive(true);
    }


    public void ResetTipsDetails()
    {
        tip1.text = "";
        tip2.text = "";
        tip3.text = "";

        tip1.gameObject.SetActive(false);
        tip2.gameObject.SetActive(false);
        tip3.gameObject.SetActive(false);

        tipsAreReady = false;
    }


    // ============================================================
    //           CARRUSEL (NAVEGACIÓN ENTRE PANELES)
    // ============================================================
    public void CarouselNavigation(bool isNext)
    {
        //if (!graphIsReady) return;          // 🔒 No permitir mover si la gráfica no terminó
        if (!isTransitioning) ChangeSubPanel(isNext);
    }


    private void ChangeSubPanel(bool isNext)
    {
        int total = subPanelsList.Count;
        int newIndex = (currentCarouselIndex + (isNext ? 1 : total - 1)) % total;

        GameObject current = subPanelsList[currentCarouselIndex];
        GameObject next = subPanelsList[newIndex];

        current.SetActive(false);
        next.SetActive(true);

        currentCarouselIndex = newIndex;

        if (newIndex == 1 && !guideLinesAreReady)
            // StartCoroutine(GraphGuidelinesMaker());
            GraphGuidelinesMaker();

            if (newIndex == 2 && !tipsAreReady)
            // StartCoroutine(GraphTipsMaker());
            GraphTipsMaker();
            if (titleText != null)
            titleText.text = GetCurrentTitle(newIndex);
    }

    //Activacion de los botones del carrusel

    public void GraphCarouselButtonsEnable(bool isEnable)
    {
        if (NavigationBackButton != null)
            NavigationBackButton.SetActive(isEnable);

        if (NavigationContinueButton != null)
            NavigationContinueButton.SetActive(isEnable);

        Debug.Log("Botones del carrusel ." + isEnable);
    }

    // =============================
    //  TITULOS DINÁMICOS
    // =============================
    private string GetCurrentTitle(int index)
    {
        if (isPieGraphDetails)
        {
            if (index == 0) return LanguageManager.Instance.GetStringValue("GraphTitleKinesthesia");
            if (index == 1) return LanguageManager.Instance.GetStringValue("DetailsButtonText");
            return LanguageManager.Instance.GetStringValue("TipsTitle");
        }

        if (isVoiceGraphDetails)
        {
            if (index == 0) return LanguageManager.Instance.GetStringValue("GraphTitleVoice");
            if (index == 1) return LanguageManager.Instance.GetStringValue("DetailsButtonText");
            return LanguageManager.Instance.GetStringValue("TipsTitle");
        }

        if (isHeatMapGraphDetails)
        {
            if (index == 0) return LanguageManager.Instance.GetStringValue("GraphTitleHeatMap");
            if (index == 1) return LanguageManager.Instance.GetStringValue("DetailsButtonText");
            return LanguageManager.Instance.GetStringValue("TipsTitle");
        }

        return "Título no definido";
    }


    // =============================
    // RESET GENERAL AL SALIR
    // =============================
    public void EndGraph()
    {
        ResetTipsDetails();
        tipsAreReady = false;
        guideLinesAreReady = false;
       // graphIsReady = false;
    }
}
