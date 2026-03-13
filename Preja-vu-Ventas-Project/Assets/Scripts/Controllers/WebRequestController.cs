using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestController : Singleton<WebRequestController>
{
    [SerializeField] bool inProgress;
    [SerializeField] bool isConnected;

    public bool InProgress { get => inProgress; set => inProgress = value; }
    public bool IsConnected { get => isConnected; set => isConnected = value; }

    public IEnumerator WaitForConnection(int timeOut)
    {
        while (!isConnected)
        {
            yield return StartCoroutine(CheckInternetConnection(OnInternetCheckComplete));

            if (isConnected)
                break;

            yield return new WaitForSeconds(1f);

            if (timeOut <= 0)
            {
                Debug.LogError("No se pudo Acceder a la Red");
                ScenesManager.Instance.LoadErrorScene();
                StopAllCoroutines();
                yield break;
            }

            timeOut--;
        }

        isConnected = false;
    }

    public IEnumerator StartLogin(string url, string email, string password)
    {
        inProgress = true;
        // Convierte los datos a formato JSON
        PlayerVerificationData myData = new PlayerVerificationData(email, password);
        string jsonData = JsonUtility.ToJson(myData);

        // Crea un UnityWebRequest con método POST
        UnityWebRequest www = new UnityWebRequest(url, "POST");

        // Configura la carga útil del JSON
        www.uploadHandler = new UploadHandlerRaw(ConvertJsonToBytes(jsonData));
        www.downloadHandler = new DownloadHandlerBuffer();

        // Configura los encabezados, indicando que el cuerpo de la solicitud es JSON
        www.SetRequestHeader("Content-Type", "application/json");

        // Envía la solicitud y espera la respuesta
        yield return www.SendWebRequest();

        // Verifica si hubo errores en la solicitud
        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
            Debug.Log("Form upload complete! Status Code: " + www.responseCode);
        }
        else
        {
            if (www.responseCode == 200)
            {
                string jsonArrayString = www.downloadHandler.text;

                if (!string.IsNullOrEmpty(jsonArrayString) && jsonArrayString != "null")
                {
                    JSONArray jsonArray = JSON.Parse(jsonArrayString) as JSONArray;

                    if (jsonArray != null && jsonArray.Count > 0)
                    {
                        Debug.Log(jsonArray);
                        LoginManager.Instance.playerIdentityCreated = true;
                        LoginManager.Instance.CreateInfoPlayer(jsonArray);
                        Debug.Log("Login successful");
                    }
                }
                else
                {
                    LoginManager.Instance.playerIdentityCreated = false;
                    Debug.LogError("Login failed: No data returned");
                }
            }
            else
            {
                JSONNode errorJson = JSON.Parse(www.downloadHandler.text);
                string errorMessage = errorJson["message"];
                Debug.LogError("Error: " + errorMessage);
                LoginManager.Instance.playerIdentityCreated = false;
            }
        }

        inProgress = false;
    }

    public IEnumerator StarGetPlayerData(string url, string id)
    {
        inProgress = true;
        // Convierte los datos a formato JSON
        InfoPlayerID myData = new InfoPlayerID(id);
        string jsonData = JsonUtility.ToJson(myData);

        // Crea un UnityWebRequest con método POST
        UnityWebRequest www = new UnityWebRequest(url, "POST");

        // Configura la carga útil del JSON
        www.uploadHandler = new UploadHandlerRaw(ConvertJsonToBytes(jsonData));
        www.downloadHandler = new DownloadHandlerBuffer();

        // Configura los encabezados, indicando que el cuerpo de la solicitud es JSON
        www.SetRequestHeader("Content-Type", "application/json");

        // Envía la solicitud y espera la respuesta
        yield return www.SendWebRequest();

        // Verifica si hubo errores en la solicitud
        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                string jsonArrayString = www.downloadHandler.text;

                if (!string.IsNullOrEmpty(jsonArrayString) && jsonArrayString != "null")
                {
                    JSONArray jsonArray = JSON.Parse(jsonArrayString) as JSONArray;

                    if (jsonArray != null && jsonArray.Count > 0)
                    {
                        Debug.Log(jsonArray);
                        LoginManager.Instance.playerCredetialsCreated = true;
                        LoginManager.Instance.CheckPlayerCredentials(jsonArray);
                    }
                    else
                    {
                        LoginManager.Instance.playerCredetialsCreated = false;
                        Debug.LogError("Login failed: No data returned");
                    }
                }
                else
                {
                    LoginManager.Instance.playerCredetialsCreated = false;
                    Debug.LogError("Failed : No data returned");
                }
            }
            else
            {
                JSONNode errorJson = JSON.Parse(www.downloadHandler.text);
                string errorMessage = errorJson["message"];
                Debug.LogError("Error: " + errorMessage);
                LoginManager.Instance.playerCredetialsCreated = false;
            }
        }

        inProgress = false;
    }

    public IEnumerator StartUpdatePrejavuToken(string url, string id)
    {
        inProgress = true;
        InfoPlayerPrejavuToken myData = new InfoPlayerPrejavuToken(id, "1");
        // Convierte los datos a formato JSON
        string jsonData = JsonUtility.ToJson(myData);

        // Crea un UnityWebRequest con método POST
        UnityWebRequest www = new UnityWebRequest(url, "POST");

        // Convierte el string JSON a bytes & Configura la carga útil del JSON 
        www.uploadHandler = new UploadHandlerRaw(ConvertJsonToBytes(jsonData));
        www.downloadHandler = new DownloadHandlerBuffer();

        // Configura los encabezados, indicando que el cuerpo de la solicitud es JSON
        www.SetRequestHeader("Content-Type", "application/json");

        // Envía la solicitud y espera la respuesta
        yield return www.SendWebRequest();

        // Verifica si hubo errores en la solicitud
        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
            LoginManager.Instance.registeredToken = false;

        }
        else
        {
            LoginManager.Instance.registeredToken = true;
        }
        inProgress = false;
    }

    public IEnumerator StartUpdateAllPlayerData(string url, string id, string startDate, string endDate)
    {
        Debug.Log("Start Date: " + startDate);
        Debug.Log("End Date: " + endDate);

        inProgress = true;
        InfoPlayerPrejavuDate myData = new InfoPlayerPrejavuDate(id, startDate, endDate);
        // Convierte los datos a formato JSON
        string jsonData = JsonUtility.ToJson(myData);

        // Crea un UnityWebRequest con método POST
        UnityWebRequest www = new UnityWebRequest(url, "POST");

        // Configura la carga útil del JSON
        www.uploadHandler = new UploadHandlerRaw(ConvertJsonToBytes(jsonData));
        www.downloadHandler = new DownloadHandlerBuffer();

        // Configura los encabezados, indicando que el cuerpo de la solicitud es JSON
        www.SetRequestHeader("Content-Type", "application/json");

        // Envía la solicitud y espera la respuesta
        yield return www.SendWebRequest();

        // Verifica si hubo errores en la solicitud
        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete! Status Code: " + www.responseCode);
            Debug.Log(www.downloadHandler.text);
        }

        inProgress = false;
    }

    void OnInternetCheckComplete(bool state)
    {
        isConnected = state;
    }

    public IEnumerator CheckInternetConnection(System.Action<bool> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get("https://www.google.com"))
        {
            request.timeout = 5;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.DataProcessingError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                callback(false);
            }
            else
            {
                callback(true);
            }
        }
    }

    public IEnumerator SendAudioToElevenLabs(AssessmentModule assessmentModule, string path, string apiUrl, string apiKey, string language)
    {
        inProgress = true;
        yield return StartCoroutine(WaitForConnection(10));

        if (!File.Exists(path))
        {
            Debug.LogError("El archivo de audio no se encontró: " + path);
            inProgress = false; // <-- también aquí para no dejar colgado el bool
            yield break;
        }

        byte[] fileBytes = File.ReadAllBytes(path);
        WWWForm form = new WWWForm();
        form.AddField("model_id", "scribe_v1");
        form.AddField("language_code", language);
        form.AddField("tag_audio_events", "false");
        form.AddBinaryData("file", fileBytes, "audio.wav", "audio/wav");

        UnityWebRequest request = UnityWebRequest.Post(apiUrl, form);
        request.SetRequestHeader("xi-api-key", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonArrayString = request.downloadHandler.text;
            Debug.Log(jsonArrayString);
            JSONNode jsonResponse = JSON.Parse(jsonArrayString);

            if (jsonResponse != null)
            {
                string transcriptionText = jsonResponse["text"];

                if (!string.IsNullOrEmpty(transcriptionText))
                {
                    Debug.Log("Texto transcrito: " + transcriptionText);
                    assessmentModule.finalAnswer = transcriptionText;
                    inProgress = false; // ✅ Solo aquí: cuando realmente tienes el texto
                }
                else
                {
                    Debug.LogWarning("Error: La clave 'text' está vacía o no existe.");
                    inProgress = false; // o maneja el reintento si lo necesitas
                }
            }
            else
            {
                Debug.LogError("Error: No se pudo parsear el JSON.");
                inProgress = false;
            }
        }
        else
        {
            Debug.LogError("Error en la request: " + request.error);
            inProgress = false; // <-- importante para no dejar el sistema bloqueado
        }
    }

    public IEnumerator GetRatingsData(string url, string id)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.DataProcessingError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                if (www.responseCode == 200)
                {
                    string jsonArrayString = www.downloadHandler.text;

                    if (!string.IsNullOrEmpty(jsonArrayString) && jsonArrayString != "null")
                    {
                        JSONNode jsonNode = JSON.Parse(jsonArrayString);

                        if (jsonNode != null && jsonNode.Count > 0)
                        {
                            // Process the received data
                            var toolboxRatings = jsonNode["toolboxratings"];
                            Debug.Log(toolboxRatings);

                            var dosAndDontsRatings = jsonNode["dosanddontsratings"];
                            Debug.Log(dosAndDontsRatings);

                            var caseMethodologyRatings = jsonNode["casemethodologyratings"];
                            Debug.Log(caseMethodologyRatings);
                        }
                    }
                }
                else
                {
                    JSONNode errorJson = JSON.Parse(www.downloadHandler.text);
                    string errorMessage = errorJson["message"];
                    Debug.LogError("Error: " + errorMessage);
                }
            }
        }
    }

    byte[] ConvertJsonToBytes(string jsonData)
    {
        // Convierte el string en un array de bytes usando la codificación UTF-8
        return new System.Text.UTF8Encoding().GetBytes(jsonData);
    }
}
public class PlayerVerificationData
{
    public string email;
    public string password;

    public PlayerVerificationData(string email, string password)
    {
        this.email = email;
        this.password = password;
    }
}

public class InfoPlayerID
{
    public string id;

    public InfoPlayerID(string id)
    {
        this.id = id;
    }
}

public class InfoPlayerPrejavuToken
{
    public string id;
    public string prejavu;

    public InfoPlayerPrejavuToken(string id, string prejavuToken)
    {
        this.id = id;
        this.prejavu = prejavuToken;
    }
}

public class InfoPlayerPrejavuDate
{
    public string id;
    public string download_date;
    public string end_date;

    public InfoPlayerPrejavuDate(string id, string download_date = null, string end_date = null)
    {
        this.id = id;
        this.download_date = download_date;
        this.end_date = end_date;
    }
}

