﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : Singleton<ScenesManager> 
{
    /// <summary>
    /// Listado de las operaciones asincronas realizadas por el GameManager
    /// </summary>
    [SerializeField] private List<AsyncOperation> _loadOperations;

    /// <summary>
    /// Lista de prefabs Utiles de la Scena
    /// </summary>
    public List<GameObject> systemPrefabs;

    /// <summary>
    /// Nombre de la escena que se encuentra en ejecución
    /// </summary>
    string _currentLevelName;

    /// <summary>
    /// Nombre del nivel anterior
    /// </summary>
    string _lastLevelName;

    /// <summary>
    /// Barra de carga para la pantalla Loading
    /// </summary>
    GameObject loadingPanel;
    UnityEngine.UI.Slider slider;

    /// <summary>
    /// Variable que define si el juego esta en ejecucion.
    /// </summary>
    [SerializeField] private bool is_pause;

    /// <summary>
    /// Varirable que almacena el nombre de la scen principal a carga.
    /// </summary>
    [SerializeField]
    string mainLevel;

    /// <summary>
    /// Propiedad que retorna el nombre de la escena que se encuentra en ejecución.
    /// </summary>
    /// <value>Nombre de la escena.</value>
    public string CurrentLevelName { get { return _currentLevelName; } set { _currentLevelName = value; } }

    /// <summary>
    /// Propiedad que retorna el estado de ejecución (T&F). 
    /// </summary>
    public bool IsPaused { get { return is_pause; } }

    void OnDisable()
    {
        _loadOperations.Clear();
    }
    
    void Start()
    {
        _currentLevelName = string.Empty;
        _loadOperations = new List<AsyncOperation>();
    }

    /// <summary>
    /// Método que permite editar los prefabs necesarios para la correcta ejecución del juego
    /// </summary>
    public void EditPrefabsGame()
    {
        for (int i = 0; i < systemPrefabs.Count; i++)
        {
            if (systemPrefabs[i].name.Equals("LoadingPanel"))
            {
                systemPrefabs[i].SetActive(false);
            }
            else
            {
                systemPrefabs[i].SetActive(true);
            }
        }

        loadingPanel = systemPrefabs[0];
        
        slider = loadingPanel.GetComponentInChildren<UnityEngine.UI.Slider>();
        LoadLevel(mainLevel);
    }

    /// <summary>
    /// Delegado del método LoadLevel que se ejecuta una vez se ha terminado de cargar una escena asincronamente.
    /// </summary>
    /// <param name="ao">Objeto que contiene la información de la escena cargada.</param>
    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (_loadOperations.Contains(ao))
            _loadOperations.Remove(ao);

        loadingPanel.SetActive(false);
        ValidateLevel();
        Debug.Log("[GameManager] Escena Cargada completamente");
    }

    /// <summary>
    /// Delegado del método UnLoadLevel que se ejecuta una vez se ha terminado de descargar una escena asincronamente.
    /// </summary>
    /// <param name="ao">Objeto que contiene la información de la escena cargada.</param>
    void OnUnLoadOperationComplete(AsyncOperation ao)
    {
        if (_loadOperations.Contains(ao))
            _loadOperations.Remove(ao);

        Debug.Log("[GameManager] Escena descargada completamente");
    }

    /// <summary>
    /// Método que permite cargar una escena de manera asincrona
    /// </summary>
    /// <param name="levelName">Nombre de la escena que se desea cargar.</param>
    public void LoadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        if (ao == null)
        {
            Debug.Log("[GameManager] Error al cargar el nivel" + levelName);
            return;
        }

        _lastLevelName = _currentLevelName;
        _currentLevelName = levelName;
        ao.completed += OnLoadOperationComplete;
        StartCoroutine(LoadingScreen(ao));
        _loadOperations.Add(ao);
    }

    /// <summary>
    /// Método que permite descargar una escena de manera asincrona.
    /// </summary>
    /// <param name="levelName">Nombre de la escena que se desea descargar.</param>
    public void UnLoadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);
        ao.completed += OnUnLoadOperationComplete;
    }

    /// <summary>
    /// Método que permite validar el nivel cargado para determinar las acciones a realizar.
    /// </summary>
    void ValidateLevel()
    {
        if (_lastLevelName != "")
            UnLoadLevel(_lastLevelName);

        switch (CurrentLevelName)
        {
            case "MainMenu":
                SoundManager.Instance.CreateSoundsLevel();
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentLevelName));
                CharacterManager.Instance.SetSettupCharacterController(0);
                LanguageManager.Instance.CallSetLenguaje(GameManager.Instance.playerStats.language);
                StartCoroutine(GameManager.Instance.StartLogos());
                break;

            case "VideoTest":
                SoundManager.Instance.CreateSoundsLevel();
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentLevelName));
                CharacterManager.Instance.SetSettupCharacterController(0);
                LanguageManager.Instance.CallSetLenguaje(GameManager.Instance.playerStats.language);
                break;
        }
    }

    /// <summary>
    /// Método generado por si existe algun error.
    /// </summary>
    public void LoadErrorScene()
    {
        SceneManager.LoadScene("Error");
    }

    /// <summary>
    /// Pausa la ejecución de la aplicación
    /// </summary>
    public void Pause()
    {
        is_pause = !is_pause;
        SoundManager.Instance.PauseAllSounds(is_pause);

        if (is_pause)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// Coroutine para la pantalla de carga
    /// </summary>
    /// <returns>Proceso de carga</returns>
    /// <param name="ao">Asyncronous Operation object</param>
    IEnumerator LoadingScreen(AsyncOperation ao)
    {
        loadingPanel.SetActive(true);
        ao.allowSceneActivation = false;

        while(ao.isDone == false)
        {
            slider.value = ao.progress;
            yield return new WaitUntil(() => ao.progress.Equals(0.9f));
            slider.value = 1f; 
            ao.allowSceneActivation = true;
            yield return new WaitForSeconds(2f);
            loadingPanel.SetActive(false);
        }
    }
    public void Quit()
    {
        Application.Quit();
    }
}