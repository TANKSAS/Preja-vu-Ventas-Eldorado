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