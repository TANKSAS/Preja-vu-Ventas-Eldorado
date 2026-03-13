using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class LoadVideoStreaming : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    [SerializeField] string VideoName;

    public void Start()
    {
        videoPlayer = this.GetComponent<VideoPlayer>();
        SetUpVideoData();
    }
    private void OnVideoPrepared(VideoPlayer vp)
    {
        // Ahora sí, length devuelve la duración en segundos
        float duration = (float)vp.length;
        Debug.Log("Duración del video: " + duration + " segundos");

        GameManager.Instance.backGroundController.currentVideoDuration = duration;
        Debug.Log(GameManager.Instance.backGroundController.currentVideoDuration);
    }


    public void SetUpVideoData()
    {
        if (Application.isEditor)
        {
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, VideoName);
            videoPlayer.url = videoPath;
        }
        else
        {
#if UNITY_ANDROID
            // Ruta con "jar:file://" para acceder al archivo dentro del APK en Android
            string videoPath = "jar:file://" + Application.dataPath + "!/assets/" + VideoName;
#else
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, VideoName);
#endif
            videoPlayer.url = videoPath;
        }

        // Suscribirse al evento cuando termine de prepararse
        videoPlayer.prepareCompleted += OnVideoPrepared;

        // Inicia la preparación (no reproduce todavía)
        videoPlayer.Prepare();
    }


    public void SetUpVideoData(string videoUrl)
    {
        VideoName = videoUrl;
    }

    public void SetVideoUrl(string url)
    {
        VideoName = url;
    }

    void OnApplicationQuit()
    {
        // Liberar recursos al salir
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer = null;
        }
    }

    public class AndroidHelper
    {
        public static string GetFilesDir
        {
            get
            {
                using var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
                return currentActivity.Call<AndroidJavaObject>("getFilesDir").Call<string>("getAbsolutePath");
            }
        }
    }
}