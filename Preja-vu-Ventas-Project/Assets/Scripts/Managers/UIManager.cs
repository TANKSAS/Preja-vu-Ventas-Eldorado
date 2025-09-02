using System;
using System.Collections;
using TMPro;
using Unity.VRTemplate;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class UIManager : Singleton<UIManager>
{
    public AudioMixer audioMixer;

    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject quizMenu;
    
    public GameObject theoreticalModuleNavigationButtonsPanel;

    public GameObject theoreticalModuleMenu;
    public GameObject theoreticalModuleNavigationNextButton;
    public GameObject theoreticalModuleNavigationPreviusButton;
    public GameObject percentageTheoreticalModuleResultsPanel;
    public GameObject theoreticalBackButton;
    public GameObject theoreticalDetailsPanel;

    public GameObject toolsPanel;
    public GameObject dosAndDontsPanel;
    public GameObject methodologyPanel;

    public GameObject modulePathPanel;
    public GameObject modulePathBackButton;

    public GameObject levelBackButton;

    public GameObject subLevelsPanel;
    public GameObject subLevelBackButton;


    public GameObject videoPlayer2DMenu;
    
    public GameObject videoInstructionsPanel;
    public GameObject videoInstructionsBackButton;
    public GameObject videoIntroduction360Button;

    public GameObject modulePracticalMenu;
    public GameObject helperPanel;
    public GameObject countDownPanel;
    public GameObject timer360Panel;
    public GameObject timer2DPanel;

    public GameObject moduleResultsMenu;
    public GameObject moduleResultsDetailsPanel;

    public GameObject practicalModuleButton;
    public GameObject practicalModuleCaseDetailPanel;
    public GameObject practicalResultsGraphicMenu;
    public GameObject practicalResultsGraphicMenuExitButton;
    public GameObject practicalResultsGraphicHeaderNavegation;
    public GameObject practicalResultsGraphicHeaderNavegationText;
    public GameObject practicalResultsGraphicHeaderNavigationNextButton;
    public GameObject practicalResultsGraphicHeaderNavigationPreviusButton;

    public GameObject practicalResultsFristTryPanel;
    public GameObject practicalResultsSecondTryPanel;
    public GameObject practicalResultsComparative;

    public GameObject practicalResultsHeatMapDetail;
    public GameObject practicalResultsHeatMapDetailTip1Text;
    public GameObject practicalResultsHeatMapDetailTip2Text;
    public GameObject practicalResultsHeatMapDetailTip3Text;

    public GameObject practicalResultsVoiceDetail;
    public GameObject practicalResultsVoiceDetailTip1Text;
    public GameObject practicalResultsVoiceDetailTip2Text;
    public GameObject practicalResultsVoiceDetailTip3Text;

    public GameObject practicalResultsPieDetail;
    public GameObject practicalResultsPieDetailTip1Text;
    public GameObject practicalResultsPieDetailTip2Text;
    public GameObject practicalResultsPieDetailTip3Text;

    public GameObject practicalResultsQualificationDetailsPanel;
    public GameObject practicalResultsIndividualGraphicPanel;
    public GameObject practicalResultsGroupGraphicPanel;

    public GameObject diagnosisInstructionsDetailPanel;
    public GameObject diagnosisInstructionsDetailBackButton;

    IEnumerator currentCoroutine;

    public Animator[] starsRatingItems;

    GameObject currentUI;
    GameObject newUI;

    public int currentIndexTheoreticalMenuNavigation;
    public int currentIndexGraphMenuNavigation;

    public void DisableAllTheoreticalContent()
    {
        Debug.Log("Desactivando todo");

        theoreticalModuleNavigationButtonsPanel.SetActive(false);
        theoreticalBackButton.SetActive(false);
        theoreticalDetailsPanel.SetActive(false);
        
        toolsPanel.SetActive(false);
        dosAndDontsPanel.SetActive(false);
        methodologyPanel.SetActive(false);
        
        modulePathPanel.SetActive(false);
        modulePathBackButton.SetActive(false);
        
        levelBackButton.SetActive(false);
        
        subLevelsPanel.SetActive(false);
        subLevelBackButton.SetActive(false);
    }

    /// <summary>
    /// Cuando terminas un subnivel → volver al panel de subniveles
    /// </summary>
    public void EndShowSubLevelUI()
    {
        Debug.Log("Finalizo de mostrar un SubNivel");
        subLevelsPanel.SetActive(true);
        levelBackButton.SetActive(true);
    }


    /// <summary>
    /// Cuando completas una unidad → volver al panel de las 9 herramientas
    /// </summary>
    public void EndShowUnitUI()
    {
        Debug.Log("Finalizo de mostrar una Unidad");
        theoreticalBackButton.SetActive(true);
        theoreticalModuleNavigationButtonsPanel.SetActive(true);
        LoadMenuSettings(3);
    }

    /// <summary>
    /// Cuando completas todas las unidades → volver al menú de módulos
    /// </summary>
    public void EndShowModuleUI()
    {
        GameManager.Instance.backGroundController.CallChangeImagen(0); 
        SetCurrentUIMenu(theoreticalModuleMenu);
        SetNewUIMenu(mainMenu);
    }

    /// <summary>
    /// Método que permite cambiar Idioma de la app.
    /// </summary>
    /// <param name="languageIndex">Numero de calidad Idioma.</param>
    public void SetLenguaje(int languageIndex)
    {
        LanguageManager.Instance.CallSetLenguaje(languageIndex);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat(Tags.VOLUMENMASTER_TAG, volume);
        audioMixer.GetFloat(Tags.VOLUMENMASTER_TAG, out GameManager.Instance.playerStats.volumen);
    }

    public void SetNewUIMenu(GameObject newUIObject)
    {
        newUI = newUIObject;
        StartCoroutine(ChangeUIMenu());
    }

    public void SetCurrentUIMenu(GameObject currentUIObject)
    {
        currentUI = currentUIObject;
    }

    // esta funcion determina si algo funciona cuando el usuario es nuevo 
    public void CheckNewPlayerObject(GameObject gameObject)
    {
        if (GameManager.Instance.playerStats.isNewPlayer)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void ChanceMusicBackGround(int value)
    {
        switch (value)
        {
            case 0:
                SoundManager.Instance.PlayNewSound(SoundManager.Instance.soungs[0].source);
                break;
            case 1:
                SoundManager.Instance.EndSound(SoundManager.Instance.soungs[0].source);
                break;
            case 2:
                SoundManager.Instance.PlayNewSound(SoundManager.Instance.soungs[1].source);
                break;
            case 3:
                SoundManager.Instance.EndSound(SoundManager.Instance.soungs[1].source);
                break;
            case 4:
                SoundManager.Instance.PlayNewSound(SoundManager.Instance.soungs[2].source);
                break;
            case 5:
                SoundManager.Instance.EndSound(SoundManager.Instance.soungs[2].source);
                break;
        }
    }

    public void TheoreticalMenuNavigation(bool isNext)
    {
        int totalPanels = 3;

        // Calcular el nuevo índice
        currentIndexTheoreticalMenuNavigation = (currentIndexTheoreticalMenuNavigation + (isNext ? 1 : totalPanels - 1)) % totalPanels;
        LoadMenuSettings(currentIndexTheoreticalMenuNavigation);
    }

    public void ResetMenuNavigationIndex()
    {
        currentIndexTheoreticalMenuNavigation = 0;
        currentIndexGraphMenuNavigation = 0;
    }

    public void BackCurrentUnitMenu()
    {
        switch (LevelProgressManager.Instance.currentUnitIndex)
        {
            case 0:
                toolsPanel.SetActive(true);
                break;

            case 1:
                methodologyPanel.SetActive(true);
                break;

            case 2:
                dosAndDontsPanel.SetActive(true);
                break;
        }

        UpdateNavigationButtons();
    }
        
    public void LoadMenuSettings(int index)
    {
        // Desactivar todos los paneles
        toolsPanel.SetActive(false);
        methodologyPanel.SetActive(false);
        dosAndDontsPanel.SetActive(false);

        if (index == 3)
        {
            index = currentIndexTheoreticalMenuNavigation = LevelProgressManager.Instance.CheckLastModule();
            Debug.Log(currentIndexTheoreticalMenuNavigation);
        }

        //Activar el panel correspondiente al nuevo índice
        switch (index)
        {
            case 0:
                toolsPanel.SetActive(true);
                break;

            case 1:
                methodologyPanel.SetActive(true);
                break;

            case 2:
                dosAndDontsPanel.SetActive(true);
                break;
        }


        CallSetCurrentUnit(index);
        UpdateNavigationButtons();
    }


    public void UpdateNavigationButtons()
    {
        if (!LevelProgressManager.Instance.isAllToolsCompleted)
            return;

        if (!theoreticalModuleNavigationButtonsPanel.activeInHierarchy)
            theoreticalModuleNavigationButtonsPanel.SetActive(true);

        // Deshabilitar ambos botones de navegación
        theoreticalModuleNavigationPreviusButton.SetActive(false);
        theoreticalModuleNavigationNextButton.SetActive(false);

        // Habilitar el botón de navegación correspondiente al último módulo completado
        switch (LevelProgressManager.Instance.currentUnitIndex)
        {
            case 0:
                 // Si el último módulo completado es el 0, activar solo el botón "Siguiente"
                theoreticalModuleNavigationNextButton.SetActive(true);
                Debug.Log("Unidad 1");
                break;
            case 1:
                Debug.Log("Unidad 2");

                // Si el último módulo completado es el 1, activar ambos botones
                theoreticalModuleNavigationPreviusButton.SetActive(true);
               break; 
                //    if (LevelProgressManager.Instance.isAllMethodologyCompleted)
                //    {
                //        theoreticalModuleNavigationNextButton.SetActive(true);
                //    }


                //case 2:
                //    // Si el último módulo completado es el 2, activar solo el botón "Anterior"
                //    theoreticalModuleNavigationPreviusButton.SetActive(true);
                //    break;
        }
    }

    public void LoadSettingsData(GameObject settingsMenu)
    {
        float volume = GameManager.Instance.playerStats.volumen;
        audioMixer.SetFloat(Tags.VOLUMENMASTER_TAG, volume);
        settingsMenu.transform.GetChild(1).transform.GetChild(3).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponentInChildren<Slider>().value = volume;
        int language = GameManager.Instance.playerStats.language;
        settingsMenu.transform.GetChild(1).transform.GetChild(3).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponentInChildren<TMP_Dropdown>().value = language;
    }

    public void CallSetPathButtonsEvent()
    {
        LevelProgressManager.Instance.SetPathButtons();
    }

    public void CallSetSubLevelButtonsEvent()
    {
        LevelProgressManager.Instance.SetSubLevelButtons();
    }

    public void CallSetCurrentUnit(int lastUnitIndex)
    {
        LevelProgressManager.Instance.SetCurrentUnit(lastUnitIndex);
        LevelProgressManager.Instance.LoadUnitesButtonsSettings();
    }

    public void CallSetCurrentLevel(int index) 
    { 
        LevelProgressManager.Instance.SetCurrentLevel(index);
    }   

    public void CallSetCurrentSubLevel(int index) 
    { 
        LevelProgressManager.Instance.SetCurrentSubLevel(index);
        LevelProgressManager.Instance.LoadSubLevelsData();
    }

    public void CancelShowGraphEvent()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            Debug.Log("Termmino"+ currentCoroutine);
        }
    }

    public void SaveSettingsInfo()
    {
        BaseDataManager.Instance.Save("/PlayerSalesData.json", GameManager.Instance.playerStats);
    }

    public void CallVideoPlayer(GameObject videoPLayer)
    {
        StartCoroutine(StartVideoPlayer(videoPLayer));
    }

    public void CallContinueShowingUIHelper(UIHelperController currentUIHelper)
    {
        currentUIHelper.canContinue = true;
    }

    public void CallIntroduction()
    {
        StartCoroutine(GameManager.Instance.StartIntroductionFromMultimedia());
    }
    
    public void CallQuestEvent()
    {
        GameManager.Instance.CallToolQualifier();
    }

    public void CallRobertaAIPracticeExercise(int caseMethodologyIndex)
    {
        GameManager.Instance.CallMethodologyQualifier(caseMethodologyIndex);
    }

    // signal probablemente por ahora para calificar el nivel do and dont
    public void CallQuestPresentation(int buttonIndex)
    {
        GameManager.Instance.CallDosAndDotsQualifier(buttonIndex);
    }

    public void CallRobertaCinematic(int cinematicIndex)
    {
        GameManager.Instance.finalTestController.robertaObjects.SetActive(true);
        StartCoroutine(RobertaController.Instance.RobertaGoToCinematic(cinematicIndex));
    }

    IEnumerator StartVideoPlayer(GameObject videoPlayer)
    {
        yield return new WaitUntil(() => videoPlayer.activeInHierarchy);
        videoPlayer.GetComponent<VideoTimeScrubControl>().First();
    }

    public void CallStartElevatorPitch()
    {
        GameManager.Instance.elevatorPitchController.StartPitch();
    }

    public void CallShowGraph()
    {
        currentIndexGraphMenuNavigation = 0;
        practicalResultsGraphicMenu.SetActive(true);
        LoadGraphMenuSettings();
        currentCoroutine = GraphManager.Instance.StartSessionResults();
        StartCoroutine(currentCoroutine);
    }

    public void CallShowPieDetailsGraph(int buttonIndex)
    {
        currentCoroutine = GraphManager.Instance.StartShowPieDetails(buttonIndex);
        StartCoroutine(currentCoroutine);
    }

    public void CallVoiceDetailsGraph(int buttonIndex)
    {
        currentCoroutine = GraphManager.Instance.StartShowVoiceDetails(buttonIndex);
        StartCoroutine(currentCoroutine);
    }

    public void CallHeatMapDetailsGraph(int buttonIndex)
    {
        currentCoroutine = GraphManager.Instance.StartShowHeatMapDetails(buttonIndex);
        StartCoroutine(currentCoroutine);
    }

    public void GraphMenuNavigation(bool isNext)
    {
        int totalPanels = 3;
        // Calcular el nuevo �ndice
        currentIndexGraphMenuNavigation = (currentIndexGraphMenuNavigation + (isNext ? 1 : totalPanels - 1)) % totalPanels;
        LoadGraphMenuSettings();
    }

    public void LoadGraphMenuSettings()
    {
        // Desactivar todos los paneles
        practicalResultsFristTryPanel.SetActive(false);
        practicalResultsSecondTryPanel.SetActive(false);
        practicalResultsComparative.SetActive(false);
        string graphName = string.Empty;

        //Activar el panel correspondiente al nuevo �ndice
        switch (currentIndexGraphMenuNavigation)
        {
            case 0:
                graphName = LanguageManager.Instance.GetStringValue("GraphAttemptTitleText01");
                practicalResultsFristTryPanel.SetActive(true);
                break;

            case 1:
                graphName = LanguageManager.Instance.GetStringValue("GraphAttemptTitleText02");
                practicalResultsSecondTryPanel.SetActive(true);
                break;

            case 2:
                graphName = LanguageManager.Instance.GetStringValue("GraphComparativeTitleText");
                practicalResultsComparative.SetActive(true);
                break;
        }

        practicalResultsGraphicHeaderNavegationText.GetComponent<TMP_Text>().text = graphName;
        UpdateNavigationButtons(currentIndexGraphMenuNavigation, true, practicalResultsGraphicHeaderNavigationNextButton, practicalResultsGraphicHeaderNavigationPreviusButton);
    }
    
    public void DeleteGraphData()
    {
        currentCoroutine = GraphManager.Instance.ResetGraph();
        StartCoroutine(currentCoroutine);
    }

    public void UpdateNavigationButtons(int index, bool isTheNextLevelEnabled, GameObject navigationNextButton, GameObject navigationPreviusButton)
    {
        // Deshabilitar ambos botones de navegaci�n
        navigationPreviusButton.SetActive(false);
        navigationNextButton.SetActive(false);
        Debug.Log(index);
        // Habilitar el bot�n de navegaci�n correspondiente al �ltimo m�dulo completado
        switch (index)
        {
            case 0:
                // Si el �ltimo m�dulo completado es el 0, activar solo el bot�n "Siguiente"
                navigationNextButton.SetActive(true);
                break;
            case 1:
                // Si el �ltimo m�dulo completado es el 1, activar ambos botones
                navigationPreviusButton.SetActive(true);

                if (isTheNextLevelEnabled)
                {
                    navigationNextButton.SetActive(true);
                }

                break;
            case 2:
                // Si el �ltimo m�dulo completado es el 2, activar solo el bot�n "Anterior"
                navigationPreviusButton.SetActive(true);
                break;
        }
    }

    IEnumerator ChangeUIMenu()
    {
        Debug.Log("Desactivo UI: " + currentUI.name);
        currentUI?.SetActive(false);
        ReplaceUIRotation();
        yield return new WaitForSeconds(2f);
        newUI.SetActive(true);
        Debug.Log("Activo UI: " + newUI.name);
    }

    public void ReplaceUIRotation()
    {
        if (currentUI.GetComponent<RectTransform>())
        {
            currentUI.GetComponent<RectTransform>().eulerAngles = new Vector3(0, -90, 0);
        }
        else
        {
            currentUI.GetComponent<Transform>().eulerAngles = new Vector3(0, 90, 0);
        }

        Destroy(currentUI.GetComponent<LazyFollow>());
        LazyFollow newLazyFollow = currentUI.AddComponent<LazyFollow>();
        newLazyFollow.targetOffset = Vector3.zero;
        newLazyFollow.positionFollowMode = LazyFollow.PositionFollowMode.None;
        newLazyFollow.rotationFollowMode = LazyFollow.RotationFollowMode.LookAtWithWorldUp;
    }

    /// <summary>
    /// Método que permite salir del juego.
    /// </summary>
    public void Quit()
    {
       ScenesManager.Instance.Quit();
    }
}
