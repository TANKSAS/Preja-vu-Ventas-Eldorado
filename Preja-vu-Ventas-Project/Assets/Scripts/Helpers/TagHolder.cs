using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tags
{
    public const string PLAYER_TAG = "Player";
    public const string MAINPANEL_TAG = "MainPanel";
    public const string VOLUMENMASTER_TAG= "MasterVolumen";
    public const string SELECTABLE_TAG = "Selectable";
}

public enum DialogueState 
{
    Idle,
    Introduction,
    UserInput, 
    Processing, 
    Feedback, 
    Rating, 
    Ended 
}

public enum KindScene
{
    Menu,
    Game
}

public enum MusicType
{
    BackGround,
    SFX,
    Voice
}

public enum Language
{
    Español,
    Ingles,
    Portugues
}

public enum ToneOfVoice
{
    WeakVoice,
    ConversationalVoice,
    ProjectedVoice,
    Screams
}

public enum KindOfPieGraph
{
    HandsMovePie,
    VisionPie, 
    GestureHandsPie,
    Default
}

public enum NarrativeState
{
    Introduction,          // [1] La IA da la consigna inicial
    WaitingForConfirmation, // [2] Esperamos OK para continuar
    AwaitingUserResponse,  // [3] El usuario envía su pitch
    ProcessingResponse,    // [4] La IA da feedback o ejemplo
    AwaitingFinalOk,       // [5] Usuario confirma recepción
    Closing,               // [6] Cierre final de la IA
    Error                  // Escena de error
}

public enum LessonType
{
    Video,
    Quiz,
    Cinematic,
    Exercise,
    Default
}






























