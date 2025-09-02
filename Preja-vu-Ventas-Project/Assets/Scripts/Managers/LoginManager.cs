using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginManager : Singleton<LoginManager>
{
    public bool startLoginCalling;
    public bool startCheckIdentity;
    public bool playerIdentityCreated;
    public bool playerInfoCreated;
    public bool playerCredetialsCreated;
    public bool playerEnable;
    public bool registeredToken;
    public bool playerDataCreated;
    public bool finishLogin;
    public bool haveAccess;

    public TimeZoneFetcher timeZone;
    public JSONObject infoJson = new JSONObject();
    WebRequestController webRequestController;

    public GameObject loginUI;
    public GameObject welcomePanel;
    public GameObject loginPanel;
    public GameObject loadingPanel;
    public GameObject infoPanel;
    public GameObject duplicatedPanel;
    public GameObject tankComunicationPanel;
    public TMP_Text infoText;
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;

    string url;

    IEnumerator currentCoroutine;
    /// <summary>
    /// Listado de las operaciones asincronas realizadas por el GameManager
    /// </summary>
    [SerializeField] private List<IEnumerator> _loadOperations = new List<IEnumerator>();

    void Start()
    {
        webRequestController = WebRequestController.Instance;
        currentCoroutine = CallLogin();
        StartCoroutine(currentCoroutine);
        _loadOperations.Add(currentCoroutine);
    }

    void OnDisable()
    {
        _loadOperations.Clear();
    }

    public void CallStartCheckIdentity()
    {
        startCheckIdentity = true;
    }

    public void RegisterPlayerDateCredentials(string checkId)
    {
        DateTime startDate = DateTime.Parse(timeZone.GetDeviceTimeZone());
        DateTime endDate = startDate.AddHours(24);

        url = "https://api.tank.com.co/setPrejavu/";
        StartCoroutine(webRequestController.StartUpdateAllPlayerData(url, checkId, startDate.ToString(), endDate.ToString()));
    }

    public void LoadPlayerProgressData(string checkId)
    {
        url = "http://localhost/PruebaUnity/LoadPlayerData.php";
        StartCoroutine(webRequestController.GetRatingsData(url, checkId));
    }

    public void CreateInfoPlayer(JSONArray jsonArrayString)
    {
        if (jsonArrayString != null && jsonArrayString.Count > 0)
        {
            infoJson = jsonArrayString[0].AsObject;
            LoadPlayerIdentityJsonData();
        }
        else
        {
            Debug.Log("JSON Array is empty or null.");
            playerInfoCreated = false;
        }
    }

    public void CheckPlayerCredentials(JSONArray jsonArrayString)
    {
        if (jsonArrayString != null && jsonArrayString.Count > 0)
        {
            infoJson = jsonArrayString[0].AsObject;
            LoadPlayerCredentialsJsonData();
        }
        else
        {
            Debug.Log("JSON Array is empty or null.");
            playerDataCreated = false;
        }
    }

    void StartLocalLoad()
    {
        if (!BaseDataManager.Instance.Load("/PlayerSalesData.json", GameManager.Instance.playerStats))
        {
            Debug.Log("New Player");
            GameManager.Instance.playerStats.isNewPlayer = true;
            BaseDataManager.Instance.Save("/PlayerSalesData.json", GameManager.Instance.playerStats);
        }
        else
        {
            Debug.Log("Old User");
        }

        startLoginCalling = true;
    }

    IEnumerator CallLogin()
    {
        Debug.Log("Inicio");
        StartLocalLoad();
        yield return new WaitForSeconds(0.5f);
        LanguageManager.Instance.StartLanguage(GameManager.Instance.playerStats.language);
        ToggleUIPanel(loginUI, true);
        ToggleUIPanel(welcomePanel, true);

        
        yield return new WaitUntil(() => startLoginCalling);

        yield return StartCoroutine(webRequestController.WaitForConnection(10));

        if (!GameManager.Instance.playerStats.isLoged)
        {
            currentCoroutine = NewUser();
        }
        else
        {
            currentCoroutine = OldUser();
        }

        _loadOperations.Add(currentCoroutine);
        StartCoroutine(currentCoroutine);

        yield return new WaitUntil(() => finishLogin);

        yield return new WaitForSeconds(0.5f);
        
        ToggleUIPanel(loadingPanel, false);

        if (haveAccess)
        {
            ToggleUIPanel(loginUI, false);
            ScenesManager.Instance.EditPrefabsGame();
            //LoadPlayerProgressData(GameManager.Instance.playerStats.playerId);
        }
        else
        {
            ToggleUIPanel(tankComunicationPanel, true);
        }

        EndLogin();
    }

    IEnumerator NewUser()
    {
        Debug.Log("New User");
        yield return new WaitForSeconds(1f);

        ToggleUIPanel(welcomePanel, false);

        while (startLoginCalling)
        {
            emailInputField.text = string.Empty;
            passwordInputField.text = string.Empty;
            ToggleUIPanel(loginPanel, true);
            Debug.Log("Waiting for field the info");
            yield return new WaitUntil(() => startCheckIdentity);

            ToggleUIPanel(loginPanel, false);
            ToggleUIPanel(loadingPanel, true);
            Debug.Log("Start Checking Identity");

            string mail = emailInputField.text;
            string password = passwordInputField.text;

            yield return StartCoroutine(webRequestController.WaitForConnection(10));

            CheckIdentity(mail, password);
            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => !webRequestController.InProgress);

            if (playerIdentityCreated)
            {
                Debug.Log("si existe usuario");

                if (playerInfoCreated)
                {
                    yield return StartCoroutine(webRequestController.WaitForConnection(10));

                    ValidatePlayerCredentials();
                    yield return new WaitForSeconds(1f);
                    yield return new WaitUntil(() => !webRequestController.InProgress);

                    if (playerCredetialsCreated)
                    {
                        if (playerEnable)
                        {
                            yield return StartCoroutine(webRequestController.WaitForConnection(10));

                            ChangePrejaVuTokenState();
                            yield return new WaitForSeconds(1f);
                            yield return new WaitUntil(() => !webRequestController.InProgress);

                            if (registeredToken)
                            {
                                Debug.Log("Device Registred");
                                string startDate = infoJson["download_date"];
                                int characterCount = startDate.Length;

                                if (string.IsNullOrEmpty(startDate) || characterCount <= 1)
                                {
                                    yield return StartCoroutine(webRequestController.WaitForConnection(10));

                                    ChangeDateStates();
                                    yield return new WaitForSeconds(1f);
                                    yield return new WaitUntil(() => !webRequestController.InProgress);
                                    GameManager.Instance.playerStats.isLoged = true;
                                    BaseDataManager.Instance.Save("/PlayerSalesData.json", GameManager.Instance.playerStats);
                                    haveAccess = true;
                                }
                                else
                                {
                                    if (CompareDates())
                                    {
                                        haveAccess = true;
                                        GameManager.Instance.playerStats.isLoged = true;
                                        BaseDataManager.Instance.Save("/PlayerSalesData.json", GameManager.Instance.playerStats);
                                    }
                                    else
                                    {
                                        haveAccess = false;
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log("Device not Registred");
                            }
                        }
                        else
                        {
                            ToggleUIPanel(loadingPanel, false);
                            ToggleUIPanel(duplicatedPanel, true);
                            CancelEvent();
                            yield break;
                        }
                    }
                    else
                    {
                        //error al cargar datos
                        CancelEvent();
                        ScenesManager.Instance.LoadErrorScene();
                        yield break;
                    }
                }
                else
                {
                    CancelEvent();
                    ScenesManager.Instance.LoadErrorScene();
                    yield break;
                }
                
                infoText.text = GameManager.Instance.playerStats.playerName;
                ToggleUIPanel(loadingPanel, false);
                ToggleUIPanel(infoPanel, true);
                yield return new WaitUntil(() => !infoPanel.activeInHierarchy);
                startLoginCalling = false;
            }
            else
            {
                Debug.Log("No existe Usuario");
                startCheckIdentity = playerIdentityCreated = playerInfoCreated = false;
                yield return new WaitForSeconds(1f);
                ToggleUIPanel(loadingPanel, false);
                infoText.text = LanguageManager.Instance.GetStringValue("IncorrectUserText");
                ToggleUIPanel(infoPanel, true);
                yield return new WaitUntil(() => !infoPanel.activeInHierarchy);
            }
        }
        
        Debug.Log("End Checking Identity");
        
        ToggleUIPanel(loadingPanel, true);
        ToggleUIPanel(loginPanel, false);
        finishLogin = true;
    }

    IEnumerator OldUser()
    {
        Debug.Log("Old User");

        ValidatePlayerCredentials();
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => !webRequestController.InProgress);

        if (playerCredetialsCreated)
        {
            yield return StartCoroutine(webRequestController.WaitForConnection(10));

            if (!playerEnable)
            {
                if (CompareDates())
                {
                    haveAccess = true;
                }
                else
                {
                    haveAccess = false;
                }

                startLoginCalling = false;
            }
            else
            {
                ToggleUIPanel(welcomePanel, false);
                ToggleUIPanel(loadingPanel, false);
                ToggleUIPanel(duplicatedPanel, true);
                CancelEvent();
                yield break;
            }

            ToggleUIPanel(welcomePanel, false);
            ToggleUIPanel(loginPanel, false);
            ToggleUIPanel(loadingPanel, false);
            infoText.text = GameManager.Instance.playerStats.playerName;
            ToggleUIPanel(infoPanel, true);
            yield return new WaitUntil(() => !infoPanel.activeInHierarchy);

            ToggleUIPanel(loadingPanel, true);
            ToggleUIPanel(loginPanel, false);
            finishLogin = true;
        }
        else
        {
            CancelEvent();
            ScenesManager.Instance.LoadErrorScene();
            yield break;
        }
    }

    void EndLogin()
    {
        startLoginCalling = false;
        startCheckIdentity = false;
        playerIdentityCreated = false;
        playerInfoCreated = false;
        playerCredetialsCreated = false;
        playerEnable = false;
        registeredToken = false;
        playerDataCreated = false;
        finishLogin = false;
        haveAccess = false;
        webRequestController.IsConnected = false;
        infoJson = null;
        Debug.Log("End Login");
    }
    
    void CheckIdentity(string email, string password)
    {
        url = "https://api.tank.com.co/authUser/";
        StartCoroutine(webRequestController.StartLogin(url, email, password));
    }

    void CheckCredentialsStates(string checkId)
    {
        url = "https://api.tank.com.co/getPrejavu/";
        StartCoroutine(webRequestController.StarGetPlayerData(url, checkId));
    }

    void RegisterDevicePrejaVuToken(string checkId)
    {
        url = "https://api.tank.com.co/setPrejavu/";
        StartCoroutine(webRequestController.StartUpdatePrejavuToken(url, checkId));
    }

    bool CompareDates()
    {
        string start = infoJson["download_date"];
        string end = infoJson["end_date"];

        DateTime startDateTime = DateTime.Parse(start);
        DateTime endDateTime = DateTime.Parse(end);
        DateTime currentDateTime = DateTime.Parse(timeZone.GetDeviceTimeZone());

        Debug.Log(startDateTime);
        Debug.Log(endDateTime);
        Debug.Log(currentDateTime);

        if (currentDateTime >= startDateTime && currentDateTime <= endDateTime)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void LoadPlayerIdentityJsonData()
    {
        GameManager.Instance.playerStats.playerId = infoJson["id"];
        GameManager.Instance.playerStats.playerName = infoJson["display_name"];
        playerInfoCreated = true;
    }

    void LoadPlayerCredentialsJsonData()
    {
        string prejavuValue = infoJson["prejavu"];
        bool isPrejavuValid = prejavuValue == "1";

        if (isPrejavuValid)
        {
            playerEnable = false;
        }
        else
        {
            playerEnable = true;
            Debug.Log("Prejavu is invalid or null!");
        }
    }

    void ValidatePlayerCredentials()
    {
        Debug.Log("Start Check Player Credentials");
        CheckCredentialsStates(GameManager.Instance.playerStats.playerId);
    }

    void ChangePrejaVuTokenState()
    {
        Debug.Log("Start Change Token State");
        RegisterDevicePrejaVuToken(GameManager.Instance.playerStats.playerId);
    }

    void ChangeDateStates()
    {
        Debug.Log("Start Registrer Dates");
        RegisterPlayerDateCredentials(GameManager.Instance.playerStats.playerId);
    }

    void ToggleUIPanel(GameObject panel, bool state)
    {
        panel.SetActive(state);
    }

    public void CancelEvent()
    {
        for (int i = 0; i < _loadOperations.Count; i++)
        {
            if (_loadOperations[i] != null)
            {
                StopCoroutine(_loadOperations[i]);
                Debug.Log("Termmino" + _loadOperations[i]);
            }
        }
    }
}
