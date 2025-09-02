using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

[CreateAssetMenu(fileName = "NewPlayerStat", menuName = "ScriptableObjects/PlayerStats", order = 0)]
public class PlayerStats : ScriptableObject
{
    public string playerName;
    public string playerId;
    public bool isLoged = false; 
    public bool isNewPlayer = true;

    public Unit[] toolsModule;
    public Unit[] methodologyModule;
    public Unit[] doandDontModule;

    public List<SessionData> sessions = new List<SessionData>();

    public float volumen;
    public int language;
    public int lastSessionIndex;

    public void Restart()
    {
        isNewPlayer = true;
        isLoged = false;

        ResetModule(toolsModule);
        ResetModule(methodologyModule);
        ResetModule(doandDontModule);

        sessions.Clear();
        volumen = 0;
        language = 0;
        lastSessionIndex = 0;

        playerName = string.Empty;
        playerId = string.Empty;
    }

    private void ResetModule(Unit[] module)
    {
        if (module == null) return;

        foreach (var unit in module)
        {
            if (unit == null) continue;

            unit.unitStats.score = 0;
            unit.unitStats.isDone = false;

            if (unit.levels != null && unit.levels.Count > 0)
            {
                foreach (var subLevel in unit.levels)
                {
                    subLevel.score = 0;
                    subLevel.isDone = false;

                    if (subLevel.lessons != null && subLevel.lessons.Count > 0)
                    {
                        foreach (var lesson in subLevel.lessons)
                        {
                            lesson.score = 0;
                            lesson.isDone = false;
                        }
                    }
                }
            }
        }
    }
}

[Serializable]
public class Unit
{
    public UnitStats unitStats;
    public List<LevelStats> levels = new List<LevelStats>();
}

[Serializable]
public class UnitStats
{
    public string moduleName;
    public bool isDone;
    [Range(0, 3)]
    public int score;
}

[Serializable]
public class LevelStats
{
    public string levelName;
    public int levelIndex;
    public bool isDone;
    [Range(0, 3)]
    public int score;
    public List<LessonStats> lessons = new List<LessonStats>();
}

[Serializable]
public class LessonStats
{
    public string lessonName;
    public int lessonIndex;
    public LessonType lessonType;
    public bool isDone;
    [Range(0, 3)]
    public int score;
}

[Serializable]
public class SessionData
{
    public int moveCounter = 0;
    public int safeMovZone = 0;
    public int dangerMovZone = 0;
    public int gestureCounter = 0;
    public int positiveGesture = 0;
    public int negativeGesture = 0;
    public int visualCounter = 0;
    public int visualSafeZone = 0;
    public int visualDangerZone = 0;
    public string imagePath;
    public List<float> readingResultsvoices = new List<float>();
    public ToneOfVoice kindOfVoice;

    public SessionData(int moveCounter, int safeMovZone, int dangerMovZone, int gestureCounter, int positiveGesture, int negativeGesture, int visualCounter, int visualSafeZone, int visualDangerZone, string imagePath)
    {
        this.moveCounter = moveCounter;
        this.safeMovZone = safeMovZone;
        this.dangerMovZone = dangerMovZone;
        this.gestureCounter = gestureCounter;
        this.positiveGesture = positiveGesture;
        this.negativeGesture = negativeGesture;
        this.visualCounter = visualCounter;
        this.visualSafeZone = visualSafeZone;
        this.visualDangerZone = visualDangerZone;
        this.imagePath = imagePath;
    }
}

