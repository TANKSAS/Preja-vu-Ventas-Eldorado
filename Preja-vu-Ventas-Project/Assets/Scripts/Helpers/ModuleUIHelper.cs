using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class ModuleUIHelper
{
    public static void InitializeButtons(List<DisplayUIButton> buttonsList)
    {
        for (int i = 0; i < buttonsList.Count; i++)
        {
            DisplayUIButton displayButton = buttonsList[i];
            GameObject holder = displayButton.levelsHolderButton;

            if (holder == null) break;

            Button button = holder.GetComponentInChildren<Button>();

            if (displayButton.levelButtonSettings == null)
                displayButton.levelButtonSettings = ScriptableObject.CreateInstance<LevelSettings>();

            displayButton.levelButtonSettings.buttonAction = button;
            displayButton.levelButtonSettings.buttonText = button.transform.GetChild(0).gameObject.GetComponentInChildren<TMP_Text>();
            displayButton.levelButtonSettings.iconPanel = button.transform.GetChild(0).transform.GetChild(2).GetComponent<Image>();
            displayButton.levelButtonSettings.blockPanel = button.transform.GetChild(1).gameObject;
            displayButton.levelButtonSettings.progressInfoPanel = button.transform.GetChild(2).gameObject;
            displayButton.levelButtonSettings.startsAnimators.Clear();
            
            for (int j = 0; j < 3; j++)
            {
                Animator starAnimator = button.transform
                    .GetChild(0).GetChild(3).GetChild(j).GetComponentInChildren<Animator>();
                displayButton.levelButtonSettings.startsAnimators.Add(starAnimator);
            }
        }
    }

    public static void SetButtonStyle(DisplayUIButton displayButton, int subLevelIndex)
    {
        Debug.Log("Configurando Buoton " + displayButton.levelsHolderButton.name + "SubLevel Index" + subLevelIndex);
        displayButton.levelButtonSettings.buttonText.text = LanguageManager.Instance.GetStringValue(LevelProgressManager.Instance.currentUnit[LevelProgressManager.Instance.currentLevel].levels[subLevelIndex].levelName);
        displayButton.levelButtonSettings.iconPanel.sprite = LevelProgressManager.Instance.GetSubLevelIcon(subLevelIndex);
    }


    public static void ResetButtons(List<DisplayUIButton> buttonsList)
    {
        for (int i = 0; i < buttonsList.Count; i++)
        {
            DisplayUIButton displayButton = buttonsList[i];
            displayButton.levelButtonSettings.buttonAction = null;
            displayButton.levelButtonSettings.blockPanel = null;
            displayButton.levelButtonSettings.progressInfoPanel = null;
            displayButton.levelButtonSettings.iconPanel = null;
            displayButton.levelButtonSettings.buttonText = null;
            displayButton.levelButtonSettings.startsAnimators.Clear();
        }
    }

    public static void SetUnitButtonSettings(Unit[] currentUnit, LevelSettings levelButtonsSettings, int index)
    {
        if (levelButtonsSettings.buttonAction == null)
            return;

        if (index != 0)
        {
            if (currentUnit[index - 1].unitStats.isDone)
            {
                levelButtonsSettings.buttonAction.interactable = true;
                levelButtonsSettings.blockPanel.SetActive(false);
            }
            else
            {
                levelButtonsSettings.buttonAction.interactable = false;
                levelButtonsSettings.blockPanel.SetActive(true);
            }
        }
        else
        {
            levelButtonsSettings.buttonAction.interactable = true;
            levelButtonsSettings.blockPanel.SetActive(false);
        }

        for (int i = 0; i < levelButtonsSettings.startsAnimators.Count; i++)
        {
            levelButtonsSettings.startsAnimators[i].gameObject.SetActive(false);
        }

        for (int i = 0; i <= currentUnit[index].unitStats.score - 1; i++)
        {
            levelButtonsSettings.startsAnimators[i].gameObject.SetActive(true);
        }
    }

    //public static int GetLessonTypeIndex(List<LessonStats> lessons, LessonType currentLessonType)
    //{
    //    int index = 0;

    //    if (currentLessonType == LessonType.Video)
    //    {
            
    //    }

    //    for (int i = 0; i < lessons.Count; i++)
    //    {
    //        if (lessons[i].lessonType == currentLessonType)
    //        {
    //            index = i;
    //            Debug.Log("Encontrado type " + currentLessonType + "en el index " + index + "");
    //        }
    //    }

    //    return index;
    //}
}

