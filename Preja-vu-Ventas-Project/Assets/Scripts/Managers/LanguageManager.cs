using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageManager : Singleton<LanguageManager>
{
    public Language currentLenguaje;
    public LocalizationSettings settings;
    int currentLanguageIndex;
    
    public void StartLanguage(int index)
    {
        currentLanguageIndex = index;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[currentLanguageIndex];
        ChangeLanguageEnum();
    }

    public AudioClip GetAudioClipValue(string key)
    {
        if (LocalizationSettings.AssetDatabase != null)
        {
            Debug.Log(key);
            return LocalizationSettings.AssetDatabase.GetLocalizedAsset<AudioClip>("Assets",key);
        }
        else
        {
            Debug.Log("No track");
            return null;
        }
    }

    public string GetStringValue(string keyName)
    {
        if (LocalizationSettings.StringDatabase!= null)
        {
            return LocalizationSettings.StringDatabase.GetLocalizedString(keyName);
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// Método que permite cambiar Idioma de la app.
    /// </summary>
    /// <param name="languageIndex">Numero de calidad Idioma.</param>
    public void CallSetLenguaje(int languageIndex)
    {
        currentLanguageIndex = languageIndex;
        ChangeLanguageEnum();
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[currentLanguageIndex];
        GameManager.Instance.playerStats.language = currentLanguageIndex;
        GameManager.Instance.timeLineController.SetCinematics(currentLenguaje);
        GameManager.Instance.backGroundController.SetBackGroundSkyboxes(currentLenguaje);
        SoundManager.Instance.SetVoiceAudioLanguage();
        Debug.Log("Selected Locale Changed: " + LocalizationSettings.SelectedLocale.Identifier);
    }
    
    public int GetLanguage()
    {
        return currentLanguageIndex;
    }

    public void ChangeLanguageEnum()
    {
        switch (currentLanguageIndex)
        {
            case 0:
                currentLenguaje = Language.Español;
                break;
            case 1:
                currentLenguaje = Language.Ingles;
                break;
            case 2:
                currentLenguaje = Language.Portugues;
                break;
        }
    }

}
