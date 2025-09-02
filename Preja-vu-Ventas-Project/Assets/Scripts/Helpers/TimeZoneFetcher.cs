using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeZoneFetcher : MonoBehaviour
{
    public string GetDeviceTimeZone()
    {
        string timeZone = "Unknown";
        string dateTimeInOtherZone;


    #if UNITY_EDITOR
        timeZone = TimeZoneInfo.Local.Id; // Obtiene la zona horaria del sistema local
        Debug.Log("Running in the Unity Editor."+ timeZone);


        // Verifica si la zona horaria es diferente a "SA Pacific Standard Time"

        if (timeZone != "SA Pacific Standard Time")
        {
            Debug.Log("Current Date and Time " + timeZone);
            timeZone = "SA Pacific Standard Time";
        }

        dateTimeInOtherZone = GetDateTimeInZone(timeZone);
        return dateTimeInOtherZone;

    #elif UNITY_ANDROID
        try
        {
        Debug.Log("Running in the Oculus Device.");

            using (AndroidJavaClass javaClass = new AndroidJavaClass("java.util.TimeZone"))
            {
                if (javaClass != null)
                {
                    using (AndroidJavaObject timeZoneObject = javaClass.CallStatic<AndroidJavaObject>("getDefault"))
                    {
                        if (timeZoneObject != null)
                        {
                            timeZone = timeZoneObject.Call<string>("getID");
                            Debug.Log(timeZone);
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Exception occurred: " + e.Message);
        }

        // Verifica si la zona horaria es diferente a "SA Pacific Standard Time"

        if (timeZone != "America/Bogota")
        {
            Debug.Log("Current Date and Time " + timeZone);
            timeZone = "America/Bogota";
        }

        dateTimeInOtherZone = GetDateTimeInZone(timeZone);
        Debug.Log("Date and Time: " + dateTimeInOtherZone);
        return dateTimeInOtherZone;
        #else
        Debug.Log("Running on a non-Android device.");
        #endif
    }

    string GetDateTimeInZone(string zoneId)
    {
    #if UNITY_EDITOR 
        try
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
            DateTime utcNow = DateTime.UtcNow;
            DateTime dateTimeInZone = TimeZoneInfo.ConvertTimeFromUtc(utcNow, zone);
            return dateTimeInZone.ToString("yyyy-MM-dd HH:mm:ss");
        }
        catch (TimeZoneNotFoundException)
        {
            return "Time zone not found.";
        }
        catch (Exception e)
        {
            return "Error: " + e.Message;
        }

    #elif UNITY_ANDROID
    
    try
        {
            using (AndroidJavaClass timeZoneClass = new AndroidJavaClass("java.util.TimeZone"))
            {
                using (AndroidJavaObject timeZoneObject = timeZoneClass.CallStatic<AndroidJavaObject>("getTimeZone", zoneId))
                {
                    if (timeZoneObject == null)
                    {
                        return "Time zone not found.";
                    }

                    using (AndroidJavaObject calendar = new AndroidJavaClass("java.util.Calendar").CallStatic<AndroidJavaObject>("getInstance"))
                    {
                        calendar.Call("setTimeZone", timeZoneObject);

                        using (AndroidJavaObject dateFormat = new AndroidJavaObject("java.text.SimpleDateFormat", "yyyy-MM-dd HH:mm:ss"))
                        {
                            dateFormat.Call("setTimeZone", timeZoneObject);

                            using (AndroidJavaObject date = calendar.Call<AndroidJavaObject>("getTime"))
                            {
                                string dateTimeInZone = dateFormat.Call<string>("format", date);
                                Debug.Log(dateTimeInZone);
                                return dateTimeInZone;
                            }
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Exception occurred: " + e.Message);
            return "Error: " + e.Message;
        }
    #else
        return "Platform not supported.";
    #endif
        return "Unable to get date and time.";
    }

    void LogAvailableTimeZones()
    {
    #if UNITY_ANDROID
        try
        {
            using (AndroidJavaClass javaClass = new AndroidJavaClass("java.util.TimeZone"))
            {
                if (javaClass != null)
                {
                    string[] timeZoneIDs = javaClass.CallStatic<string[]>("getAvailableIDs");
                    if (timeZoneIDs != null)
                    {
                        foreach (string timeZoneID in timeZoneIDs)
                        {
                            Debug.Log("Available Time Zone: " + timeZoneID);
                        }
                    }
                    else
                    {
                        Debug.LogError("timeZoneIDs is null");
                    }
                }
                else
                {
                    Debug.LogError("javaClass is null");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Exception occurred: " + e.Message);
        }
    #else
        Debug.Log("This function is only supported on Android.");
    #endif
    }
}
