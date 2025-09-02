using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class BackGroundController : MonoBehaviour
{
    public VideoSkyboxData videoSkyboxDataEsp;
    public VideoSkyboxData videoSkyboxDataIng;
    public VideoSkyboxData videoSkyboxDataPor;
    public List<Material> sceneSkyboxes = new List<Material>();
    
    public Color32 color;
    public Color32 color2;
    
    public bool isLoading;
    public VideoPlayer currentVideoPlayer;
    
    [SerializeField] List<VideoPlayer> videoPlayers = new List<VideoPlayer>();
    [SerializeField] List<Material> videoSkyboxes = new List<Material>();
    [SerializeField] bool fade;

    void Awake()
    {
        GameManager.Instance.backGroundController = this;
    }

    void OnDisable()
    {
        videoSkyboxes.Clear();

        for (int i = 0; i < videoSkyboxDataEsp.videoSkyboxes.Count; i++)
        {
            videoSkyboxDataEsp.videoSkyboxes[i].SetColor("_Tint",color);
        }

        for (int i = 0; i < videoSkyboxDataIng.videoSkyboxes.Count; i++)
        {
            videoSkyboxDataIng.videoSkyboxes[i].SetColor("_Tint", color);
        }

        for (int i = 0; i < videoSkyboxDataPor.videoSkyboxes.Count; i++)
        {
            videoSkyboxDataPor.videoSkyboxes[i].SetColor("_Tint", color);
        }

        for (int i = 0; i < sceneSkyboxes.Count; i++)
        {
            sceneSkyboxes[i].SetColor("_Tint", color);
        }
    }

    public void SetBackGroundSkyboxes(Language currentLenguaje)
    {
        videoPlayers.Clear();
        videoSkyboxes.Clear();

        switch (currentLenguaje)
        {
            case Language.Español:
                videoPlayers = new List<VideoPlayer>(videoSkyboxDataEsp.videoPlayers);
                videoSkyboxes = new List<Material>(videoSkyboxDataEsp.videoSkyboxes);
                break;

            case Language.Ingles:
                videoPlayers = new List<VideoPlayer>(videoSkyboxDataIng.videoPlayers);
                videoSkyboxes = new List<Material>(videoSkyboxDataIng.videoSkyboxes);
                break;

            case Language.Portugues:
                videoPlayers = new List<VideoPlayer>(videoSkyboxDataPor.videoPlayers);
                videoSkyboxes = new List<Material>(videoSkyboxDataPor.videoSkyboxes);
                break;
        }
    }

    public void CallChangeVideo(int index)
    {
        isLoading = true;
        StartCoroutine(ChangeNextVideo(index)); 
    }

    public void CallChangeImagen(int index) 
    {
        isLoading = true;
        StartCoroutine(ChangeNextImage(index));
    }

    //  Change Video Skybox
    IEnumerator ChangeNextVideo(int index)
    {
        currentVideoPlayer?.gameObject.SetActive(false);
        Material newSkybox = videoSkyboxes[index];
        yield return new WaitForSeconds(0.1f);
 
        RenderSettings.skybox = newSkybox;
        videoPlayers[index].gameObject.SetActive(true);
        currentVideoPlayer = videoPlayers[index];
        
        yield return new WaitForSeconds(0.1f);
        isLoading = false;        
        Debug.Log("Change Video Skybox");
    }
    
    // Change Image Skybox
    IEnumerator ChangeNextImage(int index)
    {
        Material currentSkybox = RenderSettings.skybox;
        StartCoroutine(FadeSkyboxColor(currentSkybox, color2, 1f));

        Material newSkybox = sceneSkyboxes[index];
        newSkybox.SetColor("_Tint", color2);
        
        yield return new WaitUntil(()=> !fade);

        RenderSettings.skybox = newSkybox;
        StartCoroutine(FadeSkyboxColor(newSkybox,color, 1f));

        yield return new WaitUntil(()=> !fade);
        isLoading = false;
        currentSkybox.SetColor("_Tint", color);
    }

    IEnumerator FadeSkyboxColor(Material skyboxMaterial, Color32 targetColor, float duration)
    {
        fade = true;
        float elapsedTime = 0f;
        Color32 startColor = skyboxMaterial.GetColor("_Tint");

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            skyboxMaterial.SetColor("_Tint", Color32.Lerp(startColor, targetColor, t));
            yield return null;
        }
        
        fade = false;
    }

    public void RestartVideoPlayer(VideoPlayer videoPlayer)
    {
        videoPlayer.Stop();
        videoPlayer.time = 0.0;
        RenderTexture renderTexture;
        renderTexture = videoPlayer.targetTexture;
        renderTexture.Release();
        renderTexture.Create();
        videoPlayer.targetTexture = renderTexture;
    }
}

[Serializable]
public class VideoSkyboxData
{
    public List<VideoPlayer> videoPlayers = new List<VideoPlayer>();
    public List<Material> videoSkyboxes = new List<Material>();
}
