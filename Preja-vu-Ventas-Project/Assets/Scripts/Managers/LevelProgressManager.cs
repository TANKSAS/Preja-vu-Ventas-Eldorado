using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelProgressManager : Singleton<LevelProgressManager>
{
    [Header("Unites Buttons")]
    public List<DisplayUIButton> toolLevelDisplayUIButtons = new List<DisplayUIButton>();
    public List<DisplayUIButton> methodologyLevelDisplayUIButtons = new List<DisplayUIButton>();
    public List<DisplayUIButton> dosdontsLevelDisplayUIButtons = new List<DisplayUIButton>();

    [Header("Helper Buttons")]
    public List<DisplayUIButton> unitPathButtons = new List<DisplayUIButton>();
    public List<DisplayUIButton> subLevelButtons = new List<DisplayUIButton>();

    [Header("Lesson Events")]
    public ButtonBaseEvents videoButtonEvents;
    public ButtonBaseEvents testButtonEvents;
    public ButtonBaseEvents cinematicButtonEvents;
    public ButtonBaseEvents exerciseButtonEvents;
    
    [Header("Buttons Events")]
    public ButtonBaseEvents toolSubLevelButtonEvents;

    [Header("Player Progress")]
    public bool isAllToolsCompleted;
    public bool isAllMethodologyCompleted;
    public bool isTheoricalModuleCompleted;

    [HideInInspector]
    public Unit[] currentUnit; 
    public int currentUnitIndex;
    public int currentLevel;
    public int currentSubLevel;
    public int currentLesson;

    private void Start()
    {
        isAllToolsCompleted = isAllMethodologyCompleted = isTheoricalModuleCompleted =  false;
        InitializeUnitButtons();
    }

    public int CheckLastModule()
    {
        //return (!isAllToolsCompleted && !isAllMethodologyCompleted) ? 0 :
        //(isAllToolsCompleted && !isAllMethodologyCompleted) ? 1 : 2;

        if (!isAllToolsCompleted && !isAllMethodologyCompleted)
            return 0; // Herramientas

        if (isAllToolsCompleted && !isAllMethodologyCompleted)
            return 1; // Metodología

        if (isAllToolsCompleted && isAllMethodologyCompleted)
            return 1; // 👈 Quédate en metodología, no pases a 2 todavía

        return 0;
    }

    public void InitializeUnitButtons()
    {
        ModuleUIHelper.InitializeButtons(toolLevelDisplayUIButtons);
        ModuleUIHelper.InitializeButtons(methodologyLevelDisplayUIButtons);
        ModuleUIHelper.InitializeButtons(dosdontsLevelDisplayUIButtons);
        ModuleUIHelper.InitializeButtons(unitPathButtons);
        ModuleUIHelper.InitializeButtons(subLevelButtons);
        SetCurrentProgress();
    }

    public Sprite GetSubLevelIcon(int indexNewSubLevel)
    {
        int subLevelIndex = currentUnit[currentLevel].levels[indexNewSubLevel].levelIndex;
        
        Sprite newButtonIcon;
        
        switch (currentUnitIndex)
        {
            case 0:
                newButtonIcon = toolLevelDisplayUIButtons[subLevelIndex].levelButtonSettings.buttonStyle.icon;
                break;
            case 1:
                newButtonIcon = methodologyLevelDisplayUIButtons[subLevelIndex].levelButtonSettings.buttonStyle.icon;
                break;
            case 2:
                newButtonIcon = dosdontsLevelDisplayUIButtons[subLevelIndex].levelButtonSettings.buttonStyle.icon;
                break;
            
            default:
                newButtonIcon = null;
                break;
        }

        return newButtonIcon;
    }

    [ContextMenu("Reload")]
    public void LoadUnitesButtonsSettings()
    {
        Debug.Log("Load Current Unit " + currentUnitIndex);

        switch (currentUnitIndex)
        {
            case 0:
                
                for (int i = 0; i < toolLevelDisplayUIButtons.Count; i++)
                {
                    ModuleUIHelper.SetUnitButtonSettings(GameManager.Instance.playerStats.toolsModule, toolLevelDisplayUIButtons[i].levelButtonSettings, i);
                }
             
                break;

            case 1:

                for (int i = 0; i < methodologyLevelDisplayUIButtons.Count; i++)
                {
                    ModuleUIHelper.SetUnitButtonSettings(GameManager.Instance.playerStats.methodologyModule, methodologyLevelDisplayUIButtons[i].levelButtonSettings, i);
                }

                break;

            case 2:
                
                for (int i = 0; i < dosdontsLevelDisplayUIButtons.Count; i++)
                {
                    ModuleUIHelper.SetUnitButtonSettings(GameManager.Instance.playerStats.doandDontModule, dosdontsLevelDisplayUIButtons[i].levelButtonSettings, i);
                }

                break;
        }

    }

    public void LoadSubLevelsData()
    {
        Debug.Log("Load Sublevels Data");

        for (int i = 0; i < currentUnit[currentLevel].levels.Count; i++)
        {
            SetLevelButtonSettings(i);
        }
    }

    public void LoadLessonsData()
    {
        Debug.Log("Load Lessons Data");

        for (int i = 0; i < currentUnit[currentLevel].levels[currentSubLevel].lessons.Count; i++)
        {
            SetPathButtonSettings(i);
        }
    }

    //Muestra informacion del % de progreso para saber cuanto le falta para desbloquear prueba practica
    public void ShowModuleProgressInfo()
    {
        if (!isTheoricalModuleCompleted)
        {
            int completedModules = 0;
            int totalModules = GameManager.Instance.playerStats.toolsModule.Length + GameManager.Instance.playerStats.methodologyModule.Length /*+ GameManager.Instance.playerStats.doandDontModule.Length*/;

            for (int i = 0; i < GameManager.Instance.playerStats.toolsModule.Length; i++)
            {
                if (GameManager.Instance.playerStats.toolsModule[i].unitStats.isDone)
                {
                    completedModules += 1;
                }
            }

            for (int i = 0; i < GameManager.Instance.playerStats.methodologyModule.Length; i++)
            {
                if (GameManager.Instance.playerStats.methodologyModule[i].unitStats.isDone)
                {
                    completedModules += 1;
                }
            }

            //for (int i = 0; i < GameManager.Instance.playerStats.doandDontModule.Length; i++)
            //{
            //    if (GameManager.Instance.playerStats.doandDontModule[i].isDone)
            //    {
            //        completedModules += 1;
            //    }
            //}

            float completionPercentage = (float)completedModules / totalModules * 100;

            UIManager.Instance.percentageTheoreticalModuleResultsPanel.SetActive(true);
            UIManager.Instance.percentageTheoreticalModuleResultsPanel.GetComponentInChildren<TMP_Text>().text = Mathf.RoundToInt(100 - completionPercentage) + " % " + LanguageManager.Instance.GetStringValue("PercentageTheoreticalModuleResultsPanelText");
        }
    }

    public void EndShowModuleProgressInfo()
    {
        UIManager.Instance.percentageTheoreticalModuleResultsPanel.SetActive(false);
        UIManager.Instance.percentageTheoreticalModuleResultsPanel.GetComponentInChildren<TMP_Text>().text =string.Empty;
    }

    public void ShowUnitProgressInfo(int newLevel)
    {
        switch (currentUnitIndex)
        {
            case 0:
                StartShowUnitProgressInfo(newLevel, toolLevelDisplayUIButtons[newLevel].levelButtonSettings, "InfoToolPanelText");
                break;
            case 1:
                StartShowUnitProgressInfo(newLevel, methodologyLevelDisplayUIButtons[newLevel].levelButtonSettings, "InfoMethodologyPanelText");
                break;
            case 2:
                StartShowUnitProgressInfo(newLevel, dosdontsLevelDisplayUIButtons[newLevel].levelButtonSettings, "InfoDosAndDontsPanelText");
                break;
        }
    }

    void StartShowUnitProgressInfo(int level, LevelSettings buttonSetting, string infoTextValue)
    {
        // Verificar si el módulo está marcado como completado
        if (currentUnit[level].unitStats.isDone)
            return;

        Debug.Log("Show Level" + currentUnit[level].unitStats.moduleName);
        int modulesCurrentlyDone = CountCompletedUnites(currentUnit);

        // Verificar si el módulo anterior está completado
        if (!currentUnit[level - 1].unitStats.isDone)
        {
            // Mostrar el panel de información de progreso
            buttonSetting.progressInfoPanel.SetActive(true);

            // Determinar el texto del panel de información de progreso
            string infoText = (level - 1 == modulesCurrentlyDone) ?
                LanguageManager.Instance.GetStringValue(infoTextValue) + " " + LanguageManager.Instance.GetStringValue(currentUnit[level - 1].unitStats.moduleName) :
                LanguageManager.Instance.GetStringValue("LockedPanelText");

            // Asignar el texto al componente TMP_Text del panel de información de progreso
            buttonSetting.progressInfoPanel.GetComponentInChildren<TMP_Text>().text = infoText;
        }
    }

    public void EndShowUnitProgressInfo(int newLevel)
    {
        LevelSettings buttonSettings = null;

        // Determinar las matrices de módulos y ajustes de botón según el índice actual de navegación teórica
        switch (currentUnitIndex)
        {
            case 0:
                buttonSettings = toolLevelDisplayUIButtons[newLevel].levelButtonSettings;
                break;
            case 1:
                buttonSettings = methodologyLevelDisplayUIButtons[newLevel].levelButtonSettings;
                break;
            case 2:
                buttonSettings = dosdontsLevelDisplayUIButtons[newLevel].levelButtonSettings;
                break;
        }

        // Verificar si el módulo está marcado como completado
        if (currentUnit[newLevel].unitStats.isDone)
            return;

        // Desactivar el panel de información de progreso y borrar el texto
        buttonSettings.progressInfoPanel.SetActive(false);
        buttonSettings.progressInfoPanel.GetComponentInChildren<TMP_Text>().text = string.Empty;
    }

    public void ShowLevelProgressInfo(int newLevel)
    {
        switch (currentUnitIndex)
        {
            case 0:
                StartShowLevelProgressInfo(newLevel, subLevelButtons[newLevel].levelButtonSettings, "InfoToolPanelText");
                break;
            case 1:
                StartShowLevelProgressInfo(newLevel, subLevelButtons[newLevel].levelButtonSettings, "InfoMethodologyPanelText");
                break;
            case 2:
                StartShowLevelProgressInfo(newLevel, subLevelButtons[newLevel].levelButtonSettings, "InfoDosAndDontsPanelText");
                break;
        }
    }

    void StartShowLevelProgressInfo(int newSubLevel, LevelSettings buttonSetting, string infoTextValue)
    {
        // Verificar si el módulo está marcado como completado
        if (currentUnit[currentLevel].levels[newSubLevel].isDone)
            return;

        Debug.Log("Show SubLevel" + currentUnit[currentLevel].levels[newSubLevel].levelName);
        int subLevels = CountCompletedSubLevels(currentUnit[currentLevel]);

        // Verificar si el módulo anterior está completado
        if (!currentUnit[currentLevel].levels[newSubLevel - 1].isDone)
        {
            // Mostrar el panel de información de progreso
            buttonSetting.progressInfoPanel.SetActive(true);

            // Determinar el texto del panel de información de progreso
            string infoText = (newSubLevel - 1 == subLevels) ?
                LanguageManager.Instance.GetStringValue(infoTextValue) + " " + LanguageManager.Instance.GetStringValue(currentUnit[currentLevel].levels[newSubLevel - 1].levelName) :
                LanguageManager.Instance.GetStringValue("LockedPanelText");

            // Asignar el texto al componente TMP_Text del panel de información de progreso
            buttonSetting.progressInfoPanel.GetComponentInChildren<TMP_Text>().text = infoText;
        }
    }

    public void EndShowLevelProgressInfo(int newLevel)
    {
        // Verificar si el módulo está marcado como completado
        if (currentUnit[currentLevel].levels[newLevel - 1].isDone)
            return;
        
        LevelSettings buttonSettings = subLevelButtons[newLevel].levelButtonSettings;

        // Desactivar el panel de información de progreso y borrar el texto
        buttonSettings.progressInfoPanel.SetActive(false);
        buttonSettings.progressInfoPanel.GetComponentInChildren<TMP_Text>().text = string.Empty;
    }


    public void ShowPathProgressInfo(int newLesson)
    {
        Debug.Log("Show Path level progress" + currentUnit[currentLevel].levels[currentSubLevel].lessons[newLesson].lessonType);
        
        switch (currentUnitIndex)
        {
            case 0:
            StartShowPathLevelProgressInfo(newLesson, unitPathButtons[newLesson].levelButtonSettings, "InfoToolPanelText");
                break;
            case 1:
            StartShowPathLevelProgressInfo(newLesson, unitPathButtons[newLesson].levelButtonSettings, "InfoMethodologyPanelText");
                break;
            case 2:
            StartShowPathLevelProgressInfo(newLesson, unitPathButtons[newLesson].levelButtonSettings, "InfoToolPanelText");
                break;

        }
    }
    
    void StartShowPathLevelProgressInfo(int newLesson, LevelSettings buttonPathSetting, string infoTextValue)
    {
        // Verificar si el módulo está marcado como completado
        if (currentUnit[currentLevel].levels[currentSubLevel].lessons[newLesson].isDone)
            return;

        Debug.Log("Entro al nivel" + currentUnit[currentLevel].levels[currentSubLevel].lessons[newLesson].lessonType);

        int modulesCurrentlyDone = CountCompletedLevels(currentUnit[currentLevel]);

        // Verificar si el módulo anterior está completado
        if (!currentUnit[currentLevel].levels[currentSubLevel].lessons[newLesson - 1].isDone)
        {
            // Mostrar el panel de información de progreso
            buttonPathSetting.progressInfoPanel.SetActive(true);

            // Determinar el texto del panel de información de progreso
            string infoText = (newLesson - 1 == modulesCurrentlyDone) ?
                LanguageManager.Instance.GetStringValue(infoTextValue) + " " + LanguageManager.Instance.GetStringValue(currentUnit[currentLevel].levels[currentSubLevel].lessons[newLesson - 1].lessonName) :
                LanguageManager.Instance.GetStringValue("LockedPanelText");

            // Asignar el texto al componente TMP_Text del panel de información de progreso
            buttonPathSetting.progressInfoPanel.GetComponentInChildren<TMP_Text>().text = infoText;
        }
    }

    public void EndShowPathProgressInfo(int newLesson)
    {
        // Verificar si el módulo está marcado como completado
        if (currentUnit[currentLevel].levels[currentSubLevel].lessons[newLesson].isDone)
            return;

        // Desactivar el panel de información de progreso y borrar el texto
        unitPathButtons[newLesson].levelButtonSettings.progressInfoPanel.SetActive(false);
        unitPathButtons[newLesson].levelButtonSettings.progressInfoPanel.GetComponentInChildren<TMP_Text>().text = string.Empty;
    }

    public int CountCompletedLessonsInSubLevel(Unit unit, int subLevelIndex)
    {
        int count = 0;
        foreach (var lesson in unit.levels[subLevelIndex].lessons)
            if (lesson.isDone) count++;
        return count;
    }

    public int CountCompletedSubLevels(Unit unit)
    {
        int count = 0;
        foreach (var sub in unit.levels)
            if (sub.isDone) count++;
        return count;
    }

    public int CountCompletedUnites(Unit[] unites)
    {
        int count = 0;
        foreach (var unit in unites)
        {
            if (unit.unitStats.isDone)
            {
                count++;
            }
        }
        return count;
    }

    public int CountCompletedLevels(Unit unit)
    {
        int count = 0;
        foreach (var currentLevel in unit.levels[currentSubLevel].lessons)
        {
            if (currentLevel.isDone)
            {
                count++;
            }
        }
        return count;
    }

    public IEnumerator ValidateModuleProgress()
    {
        if (currentUnit == GameManager.Instance.playerStats.toolsModule && isAllToolsCompleted && !isAllMethodologyCompleted)
        {
            UIManager.Instance.DisableAllTheoreticalContent();
            
            UIManager.Instance.theoreticalModuleMenu.SetActive(false);
            UIManager.Instance.SetCurrentUIMenu(UIManager.Instance.theoreticalModuleMenu);
            UIManager.Instance.ReplaceUIRotation();
            
            yield return new WaitForSeconds(.1f);
            UIManager.Instance.moduleResultsMenu.SetActive(true);
            UIManager.Instance.moduleResultsDetailsPanel.GetComponentInChildren<TMP_Text>().text =
                LanguageManager.Instance.GetStringValue("ToolsModuleResultsDetailsPanel");

            Debug.Log("Mostrando mensaje de finalizacion de tools");
            yield return new WaitUntil(() => !UIManager.Instance.moduleResultsMenu.activeInHierarchy);
            
            UIManager.Instance.theoreticalBackButton.SetActive(true);
            UIManager.Instance.theoreticalModuleMenu.SetActive(true);

            UIManager.Instance.SetCurrentUIMenu(UIManager.Instance.moduleResultsMenu);
            UIManager.Instance.ReplaceUIRotation();

            UIManager.Instance.LoadMenuSettings(0);
        }

        if (currentUnit == GameManager.Instance.playerStats.methodologyModule && isAllMethodologyCompleted && isAllToolsCompleted)
        {
            Debug.Log("Mostrando mensaje de finalizacion de Metodologia");
            UIManager.Instance.DisableAllTheoreticalContent();

            UIManager.Instance.theoreticalModuleMenu.SetActive(false);
            UIManager.Instance.SetCurrentUIMenu(UIManager.Instance.theoreticalModuleMenu);
            UIManager.Instance.ReplaceUIRotation();


            yield return new WaitForSeconds(.1f);
            UIManager.Instance.moduleResultsMenu.SetActive(true);
            UIManager.Instance.moduleResultsDetailsPanel.GetComponentInChildren<TMP_Text>().text =
                LanguageManager.Instance.GetStringValue("MethodologyModuleResultsDetailsPanel");

            yield return new WaitUntil(() => !UIManager.Instance.moduleResultsMenu.activeInHierarchy);
            UIManager.Instance.SetCurrentUIMenu(UIManager.Instance.moduleResultsMenu);
            UIManager.Instance.ReplaceUIRotation(); 
            UIManager.Instance.EndShowModuleUI();
        }
    }

    public void SetPathButtonSettings(int currentLesson)
    {
        // CORRECCIÓN: Usar el estado actualizado de las lecciones
        var lesson = currentUnit[currentLevel].levels[currentSubLevel].lessons[currentLesson];

        if (currentLesson != 0)
        {
            var prevLesson = currentUnit[currentLevel].levels[currentSubLevel].lessons[currentLesson - 1];
            if (prevLesson.isDone)
            {
                unitPathButtons[currentLesson].levelButtonSettings.buttonAction.interactable = true;
                unitPathButtons[currentLesson].levelButtonSettings.blockPanel.SetActive(false);
            }
            else
            {
                unitPathButtons[currentLesson].levelButtonSettings.buttonAction.interactable = false;
                unitPathButtons[currentLesson].levelButtonSettings.blockPanel.SetActive(true);
            }
        }
        else
        {
            unitPathButtons[currentLesson].levelButtonSettings.buttonAction.interactable = true;
            unitPathButtons[currentLesson].levelButtonSettings.blockPanel.SetActive(false);
        }

        // Actualizar estrellas basado en la puntuación actual
        for (int i = 0; i < unitPathButtons[currentLesson].levelButtonSettings.startsAnimators.Count; i++)
        {
            bool shouldShow = i < lesson.score;
            unitPathButtons[currentLesson].levelButtonSettings.startsAnimators[i].gameObject.SetActive(shouldShow);
        }
    }

    public void SetLevelButtonSettings(int newSubLevel)
    {
        if (newSubLevel != 0)
        {
            if (currentUnit[currentLevel].levels[newSubLevel - 1].isDone)
            {
                subLevelButtons[newSubLevel].levelButtonSettings.buttonAction.interactable = true;
                subLevelButtons[newSubLevel].levelButtonSettings.blockPanel.SetActive(false);
            }
            else
            {
                subLevelButtons[newSubLevel].levelButtonSettings.buttonAction.interactable = false;
                subLevelButtons[newSubLevel].levelButtonSettings.blockPanel.SetActive(true);
            }
        }
        else
        {
            subLevelButtons[newSubLevel].levelButtonSettings.buttonAction.interactable = true;
            subLevelButtons[newSubLevel].levelButtonSettings.blockPanel.SetActive(false);
        }

        for (int i = 0; i < subLevelButtons[newSubLevel].levelButtonSettings.startsAnimators.Count; i++)
        {
            subLevelButtons[newSubLevel].levelButtonSettings.startsAnimators[i].gameObject.SetActive(false);
        }

        for (int i = 0; i <= currentUnit[currentLevel].levels[newSubLevel].score - 1; i++)
        {
            subLevelButtons[newSubLevel].levelButtonSettings.startsAnimators[i].gameObject.SetActive(true);
        }
    }

    public void SetPathButtons()
    {
        // Desactivar todos los botones
        foreach (var pathButton in unitPathButtons)
        {
            if (pathButton.levelsHolderButton != null)
            {
                pathButton.levelsHolderButton.SetActive(false);
            }
        }

        // Activar y configurar solo los necesarios
        for (int i = 0; i < currentUnit[currentLevel].levels[currentSubLevel].lessons.Count; i++)
        {
            int lessonIndex = i;
            var displayButton = unitPathButtons[lessonIndex];
            GameObject buttonGO = displayButton.levelsHolderButton;
            Button button = displayButton.levelButtonSettings.buttonAction;

            if (buttonGO == null || button == null)
                continue;

            buttonGO.SetActive(true);

            // Limpieza del listener
            button.onClick.RemoveAllListeners();

            
            button.onClick.AddListener(() =>
            {
                SetCurrentLesson(lessonIndex);
            });
            
            // Agrega evento base del inspector según tipo de botón
            switch (currentUnit[currentLevel].levels[currentSubLevel].lessons[lessonIndex].lessonType)
            {
                case LessonType.Video:
                    if (videoButtonEvents.defaultOnClickEvent != null)
                        button.onClick.AddListener(() => videoButtonEvents.defaultOnClickEvent.Invoke());
                    break;
                case LessonType.Quiz:
                    if (testButtonEvents.defaultOnClickEvent != null)
                        button.onClick.AddListener(() => testButtonEvents.defaultOnClickEvent.Invoke());
                    break;
                case LessonType.Cinematic:
                    if (cinematicButtonEvents.defaultOnClickEvent != null)
                        button.onClick.AddListener(() => cinematicButtonEvents.defaultOnClickEvent.Invoke());
                    break;
                case LessonType.Exercise:
                    if (exerciseButtonEvents.defaultOnClickEvent != null)
                        button.onClick.AddListener(() => exerciseButtonEvents.defaultOnClickEvent.Invoke());
                    break;
            }

        }

        LoadLessonsData();
    }

    public void SetSubLevelButtons()
    {
        // Desactivar todos los botones
        foreach (var subLevelButton in subLevelButtons)
        {
            if (subLevelButton.levelsHolderButton != null)
            {
                subLevelButton.levelsHolderButton.SetActive(false);
            }
        }

        // Activar y configurar solo los necesarios
        for (int i = 0; i < currentUnit[currentLevel].levels.Count; i++)
        {
            int levelIndex = i;

            var displayButton = subLevelButtons[levelIndex];
            GameObject buttonGO = displayButton.levelsHolderButton;
            Button button = displayButton.levelButtonSettings.buttonAction;

            if (buttonGO == null || button == null)
                continue;

            buttonGO.SetActive(true);

            ModuleUIHelper.SetButtonStyle(displayButton, levelIndex);
            
            // Limpieza del listener
            button.onClick.RemoveAllListeners();
            
            // Agrega acción personalizada
            button.onClick.AddListener(() =>
            {
                SetCurrentSubLevel(levelIndex);
                LoadSubLevelsData();
                SetPathButtons();
            });

            if (toolSubLevelButtonEvents.defaultOnClickEvent != null)
                button.onClick.AddListener(() => toolSubLevelButtonEvents.defaultOnClickEvent.Invoke());
        }
    }


    public void SetCurrentUnit(int lastUnit)
    {
        currentUnitIndex = lastUnit;
        currentLevel = 0;
        currentSubLevel = 0;
        currentLesson = 0;
        currentUnit = null;
        
        switch (currentUnitIndex)
        {
            case 0:
                currentUnit = GameManager.Instance.playerStats.toolsModule;
                break;
            case 1:
                currentUnit = GameManager.Instance.playerStats.methodologyModule;
                break;
            case 2:
                currentUnit = GameManager.Instance.playerStats.doandDontModule;
                break;
        }

        Debug.Log("current Unit index " + currentUnitIndex + "current Level Name " + currentUnit[currentLevel].unitStats.moduleName);
    }

    public void SetCurrentLevel(int lastLevel)
    {
        currentLevel = lastLevel;
    }

    public void SetCurrentSubLevel(int lastSubLevel)
    {
        currentSubLevel = lastSubLevel;
    }

    public void SetCurrentLesson(int lastLesson)
    {
        currentLesson = lastLesson;
    }

    public void SetCurrentProgress()
    {
        if (GameManager.Instance.playerStats.toolsModule.All(unit => unit.unitStats.isDone))
        {
            isAllToolsCompleted = true;
        }
        else
        {
            UIManager.Instance.theoreticalModuleNavigationButtonsPanel.SetActive(false);
        }

        if (GameManager.Instance.playerStats.methodologyModule.All(unit => unit.unitStats.isDone))
        {
            isAllMethodologyCompleted = true;
            isTheoricalModuleCompleted = true;
            UIManager.Instance.ResetMenuNavigationIndex();
            UIManager.Instance.practicalModuleButton.GetComponentInChildren<Button>().interactable = true;
        }

        if (GameManager.Instance.playerStats.doandDontModule.All(unit => unit.unitStats.isDone))
        {

        }
    }

    void Restart()
    {
        ModuleUIHelper.ResetButtons(toolLevelDisplayUIButtons);
        ModuleUIHelper.ResetButtons(methodologyLevelDisplayUIButtons);
        ModuleUIHelper.ResetButtons(dosdontsLevelDisplayUIButtons);
        ModuleUIHelper.ResetButtons(unitPathButtons);
        ModuleUIHelper.ResetButtons(subLevelButtons);
    }

    private void OnDisable()
    {
        Restart();
    }
}

[System.Serializable]
public class ButtonBaseEvents
{
    public UnityEvent defaultOnClickEvent;
}