using System;
using System.Collections;
using Unity.VRTemplate;
using UnityEngine;
using Convai.Scripts.Runtime.UI;
using TMPro;
using System.Linq;

public class GameManager : Singleton<GameManager>
{
    [Header("Player & Controllers")] 
    public PlayerStats playerStats;
    public TrackingController trackingController;
    public Screenshot screenshotController;
    public OutputAudioRecorder outputAudioRecorderController;

    [Header("UI Components")] 
    public ChatBoxUI chatAIBoxUI;
    public ConvaiChatUIHandler _chatUIHandler;
    public Spectrum spectrumVisualizer;
    public GameObject handBANTUI;

    [Header("Game Controllers")] 
    public BackGroundController backGroundController;
    public QuestGameManager questGameManager;
    public TimeLineController timeLineController;
    public VideoTimeScrubControl videoPlayer;
    public ElevatorPitchController elevatorPitchController;
    public FinalTestController finalTestController;

    [Header("Audio Effects")] 
    [SerializeField] Sound congratulationsEffect;
    [SerializeField] Sound condolencesEffect;

    #region Unity Lifecycle
    /// <summary>
    /// Inicializa el componente desactivando la interfaz de chat de IA al inicio.
    /// </summary>
    void Start()
    {
        InitializeChatUI();
    }

    void OnDestroy()
    {
        playerStats.Restart();
        base.OnDestroy();
    }
    #endregion

    #region Initialization
    private void InitializeChatUI()
    {
        chatAIBoxUI = GameObject.FindGameObjectWithTag("ChatAIBoxUI").GetComponent<ChatBoxUI>();
        chatAIBoxUI.gameObject.SetActive(false);
        _chatUIHandler = FindObjectOfType<ConvaiChatUIHandler>();
    }
    #endregion

    #region Game Start Sequences
    /// <summary>
    /// Secuencia de inicio que gestiona la reproducción de videos introductorios,
    /// la carga de datos del jugador y la activación del menú principal.
    /// </summary>
    public IEnumerator StartLogos()
    {
        yield return StartCoroutine(PlayInitialVideo());
        yield return StartCoroutine(HandleNewPlayerIntroduction());
        yield return StartCoroutine(SetupGameEnvironment());
        yield return StartCoroutine(HandleNewPlayerDiagnostics());
        yield return StartCoroutine(FinalizeGameSetup());
    }

    private IEnumerator PlayInitialVideo()
    {
        backGroundController.currentVideoPlayer.Play();
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => !backGroundController.currentVideoPlayer.isPlaying);
        backGroundController.RestartVideoPlayer(backGroundController.currentVideoPlayer);
    }

    private IEnumerator HandleNewPlayerIntroduction()
    {
        if (!playerStats.isNewPlayer) yield break;

        backGroundController.CallChangeVideo(1);
        yield return new WaitForSeconds(.5f);
        backGroundController.currentVideoPlayer.Play();

        yield return new WaitUntil(() => backGroundController.currentVideoPlayer.isPlaying);
        Debug.Log("Introduction");

        yield return new WaitUntil(() => !backGroundController.currentVideoPlayer.isPlaying);
        backGroundController.RestartVideoPlayer(backGroundController.currentVideoPlayer);
    }

    private IEnumerator SetupGameEnvironment()
    {
        backGroundController.CallChangeImagen(0);
        UIManager.Instance.ChanceMusicBackGround(0);
        LevelProgressManager.Instance.LoadUnitesButtonsSettings();
        yield return new WaitUntil(() => !backGroundController.isLoading);
    }

    private IEnumerator HandleNewPlayerDiagnostics()
    {
        if (!playerStats.isNewPlayer) yield break;

        SetupInstructionsPanels();
        UIManager.Instance.CallVideoPlayer(UIManager.Instance.videoInstructionsPanel.transform.GetChild(0).gameObject.transform.GetChild(1).transform.gameObject);
        yield return new WaitUntil(() => !UIManager.Instance.videoInstructionsPanel.activeInHierarchy);
        UIManager.Instance.videoIntroduction360Button.SetActive(true);
        //yield return StartCoroutine(StartDaignosis());

        playerStats.isNewPlayer = false;
        BaseDataManager.Instance.Save("/PlayerSalesData.json", playerStats);
        yield return new WaitForSeconds(0.5f);
    }

    private void SetupInstructionsPanels()
    {
        UIManager.Instance.videoIntroduction360Button.SetActive(false);
        UIManager.Instance.videoInstructionsPanel.SetActive(true);
    }

    private IEnumerator FinalizeGameSetup()
    {
        finalTestController.robertaPrefab.SetActive(true);
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(RobertaController.Instance.Spawning());
        yield return new WaitForSeconds(1f);
        UIManager.Instance.mainMenu.SetActive(true);
    }

    /// <summary>
    /// Inicia la introducción desde el botón de multimedia.
    /// </summary>
    public IEnumerator StartIntroductionFromMultimedia()
    {
        yield return StartCoroutine(PlayMultimediaIntroduction());
        yield return StartCoroutine(RestoreMainEnvironment());
    }

    private IEnumerator PlayMultimediaIntroduction()
    {
        UIManager.Instance.ChanceMusicBackGround(1);
        backGroundController.CallChangeVideo(1);
        yield return new WaitForSeconds(0.5f);

        backGroundController.currentVideoPlayer.Play();
        yield return new WaitUntil(() => backGroundController.currentVideoPlayer.isPlaying);
        yield return new WaitUntil(() => !backGroundController.currentVideoPlayer.isPlaying);

        Debug.Log("Introduction");
        yield return new WaitForSeconds(0.5f);
        backGroundController.RestartVideoPlayer(backGroundController.currentVideoPlayer);
    }

    private IEnumerator RestoreMainEnvironment()
    {
        backGroundController.CallChangeImagen(0);
        UIManager.Instance.ChanceMusicBackGround(0);
        yield return new WaitUntil(() => !backGroundController.isLoading);
        UIManager.Instance.mainMenu.SetActive(true);
    }
    #endregion

    #region Diagnostics
    public IEnumerator StartFirstDaignosis()
    {
        yield return StartCoroutine(ShowDiagnosisInstructions());
        yield return StartCoroutine(WaitForDiagnosisCompletion());
    }

    private IEnumerator ShowDiagnosisInstructions()
    {
        UIManager.Instance.diagnosisInstructionsDetailBackButton.SetActive(false);
        UIManager.Instance.diagnosisInstructionsDetailPanel.SetActive(true);
        yield return new WaitUntil(() => !UIManager.Instance.diagnosisInstructionsDetailPanel.activeInHierarchy);
        UIManager.Instance.diagnosisInstructionsDetailBackButton.SetActive(true);
        Debug.Log("Empezó Diagnóstico");
    }

    private IEnumerator WaitForDiagnosisCompletion()
    {
        yield return new WaitUntil(() => UIManager.Instance.practicalResultsGraphicMenu.activeInHierarchy);
        Debug.Log("Inicia Ver Detalles");

        yield return new WaitUntil(() => !UIManager.Instance.practicalResultsGraphicMenu.activeInHierarchy);
        Debug.Log("Terminó Diagnóstico");
    }
    #endregion

    #region Assessment Qualifiers
    public void CallTheoreticalVideoQualifier()
    {
        //int currentLessonListPosition = ModuleUIHelper.GetLessonTypeIndex(
        //    LevelProgressManager.Instance.currentUnit[LevelProgressManager.Instance.currentLevel].levels[LevelProgressManager.Instance.currentSubLevel].lessons,
        //    LessonType.Video);
        ProcessVideoQualificationByUnit(LevelProgressManager.Instance.currentLesson);
    }

    private void ProcessVideoQualificationByUnit(int lessonPosition)
    {
        switch (LevelProgressManager.Instance.currentUnitIndex)
        {
            case 0:
                ProcessToolsModuleVideo(lessonPosition);
                break;
            case 1:
                ProcessMethodologyModuleVideo(lessonPosition);
                break;
            case 2:
                ProcessDoAndDontModuleVideo(lessonPosition);
                break;
        }
    }

    private void ProcessToolsModuleVideo(int lessonPosition)
    {
        var lesson = playerStats.toolsModule[LevelProgressManager.Instance.currentLevel]
            .levels[LevelProgressManager.Instance.currentSubLevel]
            .lessons[lessonPosition];

        if (!lesson.isDone)
        {
            lesson.score = 3;
            lesson.isDone = true;
            StartCoroutine(StartValidateCurrentLevel());
        }
    }

    private void ProcessMethodologyModuleVideo(int lessonPosition)
    {
        var lesson = playerStats.methodologyModule[LevelProgressManager.Instance.currentLevel]
            .levels[LevelProgressManager.Instance.currentSubLevel]
            .lessons[lessonPosition];

        if (!lesson.isDone)
        {
            lesson.score = 3;
            lesson.isDone = true;
            StartCoroutine(StartValidateCurrentLevel());
        }
    }

    private void ProcessDoAndDontModuleVideo(int lessonPosition)
    {
        var lesson = playerStats.doandDontModule[LevelProgressManager.Instance.currentLevel]
            .levels[LevelProgressManager.Instance.currentSubLevel]
            .lessons[lessonPosition];

        if (!lesson.isDone)
        {
            lesson.score = 3;
            lesson.isDone = true;
            StartCoroutine(StartValidateCurrentLevel());
        }
    }

    /// <summary>
    /// Inicia la evaluaci�n de una herramienta y desbloquea el siguiente nivel.
    /// </summary>
    public void CallToolQualifier()
    {
        StartCoroutine(RunQuizFlow());
    }

    /// <summary>
    /// Inicia la evaluaci�n de una metodolog�a y desbloquea el siguiente nivel.
    /// </summary>
    /// <param name="caseIndex">�ndice del caso a evaluar.</param>
    public void CallMethodologyQualifier(int caseIndex)
    {
        StartCoroutine(RunMethodologyQuizFlow(caseIndex));
    }

    /// <summary>
    /// Inicia la secuencia de evaluaci�n de un "Do and Don't" y desbloquea el siguiente nivel.
    /// </summary>
    /// <param name="tipIndex">�ndice del consejo a evaluar.</param>
    public void CallDosAndDotsQualifier(int tipIndex)
    {
        StartCoroutine(RunDosAndDontsQuizFlow(tipIndex));
    }

    public void CallFinalTestFeedBackQualifier(AssessmentModule assessment)
    {
        StartCoroutine(StartFinalTestFeedBack(assessment));
    }

    #endregion

    #region Quiz Flows
    private IEnumerator RunQuizFlow()
    {
        yield return StartCoroutine(WaitForBackgroundLoading());
        yield return StartCoroutine(ExecuteQuiz());
        StartCoroutine(StartValidateCurrentLevel());
    }

    private IEnumerator WaitForBackgroundLoading()
    {
        yield return new WaitUntil(() => backGroundController.isLoading);
        yield return new WaitUntil(() => !backGroundController.isLoading);
    }

    private IEnumerator ExecuteQuiz()
    {
        UIManager.Instance.quizMenu.SetActive(true);
        yield return new WaitForEndOfFrame();

        questGameManager.StartGame();
        yield return new WaitUntil(() => !UIManager.Instance.quizMenu.activeInHierarchy);
    }

    private IEnumerator RunMethodologyQuizFlow(int caseQuestionnaireIndex)
    {
        yield return StartCoroutine(SetupMethodologyTest(caseQuestionnaireIndex));
        yield return StartCoroutine(ExecuteMethodologyAssessment(caseQuestionnaireIndex));
        StartCoroutine(StartValidateCurrentLevel());
        finalTestController.robertaObjects.SetActive(false);
    }

    private IEnumerator SetupMethodologyTest(int caseIndex)
    {
        finalTestController.robertaObjects.SetActive(true);
        yield return StartCoroutine(RobertaController.Instance.RobertaGoToExercise(0, caseIndex));
    }

    private IEnumerator ExecuteMethodologyAssessment(int caseIndex)
    {
        playerStats.methodologyModule[caseIndex].unitStats.score = 3;
        backGroundController.CallChangeImagen(1);
        yield return new WaitUntil(() => !backGroundController.isLoading);
    }

    private IEnumerator RunDosAndDontsQuizFlow(int doAndDontIndex)
    {
        playerStats.doandDontModule[doAndDontIndex].unitStats.score = 3;

        backGroundController.CallChangeImagen(1);
        yield return new WaitUntil(() => !backGroundController.isLoading);

        StartCoroutine(StartValidateCurrentLevel());
        finalTestController.robertaObjects.SetActive(false);
    }

    private IEnumerator StartFinalTestFeedBack(AssessmentModule assessment)
    {
        yield return StartCoroutine(RobertaController.Instance.RobertaGoToFeedBack(assessment, 0));
        yield return new WaitUntil(() => !WebRequestController.Instance.InProgress); 
        
        var robertaAI = RobertaController.Instance.robertaAI;
        robertaAI.isEndingAnalyzeAIResponse = true;
        robertaAI.AnalyzeAIResponse();

        yield return new WaitUntil(() => !robertaAI.isEndingAnalyzeAIResponse);
        yield return StartCoroutine(ShowFeedBackFinalTestResults(robertaAI.isApproved));
    }
    #endregion

    #region Level Validation & Scoring
    public IEnumerator StartValidateCurrentLevel()
    {
        var lpm = LevelProgressManager.Instance;
        var unit = lpm.currentUnit[lpm.currentLevel];

        if (unit.levels.Count > 1)
            yield return StartCoroutine(ValidateLevelFlow(true));
        else
            yield return StartCoroutine(ValidateLevelFlow(false));
    }

    private IEnumerator ValidateLevelFlow(bool hasSubLevel)
    {
        Debug.Log("Entro a validar SubNivel");
        var lpm = LevelProgressManager.Instance;
        var unit = lpm.currentUnit[lpm.currentLevel];
        var sub = unit.levels[lpm.currentSubLevel];

        int lastScore = sub.score;
        bool wasDoneBefore = sub.isDone;
        int newScore = QualifyCurrentSubLevel();

        // Actualizar solo si es una mejor puntuación
        if (newScore > sub.score) sub.score = newScore;

        // Verificar si todas las lecciones están completas
        bool nowCompleted = lpm.CountCompletedLessonsInSubLevel(unit, lpm.currentSubLevel) == sub.lessons.Count;

        // Solo marcar como completado si todas las lecciones están hechas
        if (!wasDoneBefore && nowCompleted)
        {
            sub.isDone = true;
        }

        if (hasSubLevel)
        {
            if ((!wasDoneBefore && nowCompleted) || (wasDoneBefore && newScore > lastScore))
            {
                yield return StartCoroutine(LevelResultsDetails(lastScore, newScore, wasDoneBefore));
                UIManager.Instance.EndShowSubLevelUI();
                Debug.Log("aqui estoy ");
            }
        }

        yield return StartCoroutine(ValidateUnitFlow(hasSubLevel));

        //SaveAndRefresh();
        //// Validar unidad si todos los subniveles están completos
        //if (lpm.CountCompletedSubLevels(unit) == unit.levels.Count)
        //{
        //    Debug.Log("Entro a finalizar la unidad");
        //    yield return new WaitForSeconds(.5f);
            
        //}
    }

    private IEnumerator ValidateUnitFlow(bool hasSubLevel)
    {
        var lpm = LevelProgressManager.Instance;
        var unit = lpm.currentUnit[lpm.currentLevel];

        int lastScoreUnit = unit.unitStats.score;
        bool wasDoneBeforeUnit = unit.unitStats.isDone;
        int newScoreUnit = QualifyCurrentUnit();

        // Solo marcar la unidad como completada si todos sus subniveles están completos
        unit.unitStats.isDone = unit.levels.All(l => l.isDone);


        if ((!wasDoneBeforeUnit && unit.unitStats.isDone) ||
           (wasDoneBeforeUnit && newScoreUnit > lastScoreUnit))
        {
            yield return StartCoroutine(UnitResultsDetails(lastScoreUnit, newScoreUnit, wasDoneBeforeUnit));
            UIManager.Instance.EndShowUnitUI();

            // Validar progreso del módulo
            lpm.SetCurrentProgress();
            yield return StartCoroutine(lpm.ValidateModuleProgress());
        }

        SaveAndRefresh();
    }

    private IEnumerator LevelResultsDetails(int lastScore, int currentScore, bool wasDoneBefore)
    {
        yield return new WaitUntil(() => UIManager.Instance.theoreticalModuleMenu.activeInHierarchy);

        Debug.Log("Mostrando Resultados del Nivel");
        UIManager.Instance.DisableAllTheoreticalContent();

        UIManager.Instance.theoreticalDetailsPanel.SetActive(true);
        ShowSubLevelQualification(!wasDoneBefore, wasDoneBefore && currentScore > lastScore);

        yield return new WaitForSeconds(5f); // duración panel
        UIManager.Instance.theoreticalDetailsPanel.SetActive(false);
    }

    private IEnumerator UnitResultsDetails(int lastScore, int currentScore, bool wasDoneBefore)
    {
        yield return new WaitUntil(() => UIManager.Instance.theoreticalModuleMenu.activeInHierarchy); 
        
        Debug.Log("Mostrando Resultados del Unitresults");
        UIManager.Instance.DisableAllTheoreticalContent();

        UIManager.Instance.theoreticalDetailsPanel.SetActive(true);
        ShowUnitQualification(!wasDoneBefore, wasDoneBefore && currentScore > lastScore);

        yield return new WaitForSeconds(5f); // duración panel
        UIManager.Instance.theoreticalDetailsPanel.SetActive(false);
    }

    private void SaveAndRefresh()
    {
        var lpm = LevelProgressManager.Instance;
        lpm.LoadUnitesButtonsSettings();
        lpm.LoadLessonsData();
        lpm.LoadSubLevelsData();

        BaseDataManager.Instance.Save("/PlayerSalesData.json", playerStats);
    }

    #endregion

    #region Scoring Calculations
    int QualifyCurrentUnit()
    {
        var unit = LevelProgressManager.Instance.currentUnit[LevelProgressManager.Instance.currentLevel];

        int totalPoints = 0;
        int currentPoints = 0;
        bool allLevelsDone = true;

        // Recorremos cada subnivel (level) y sumamos sus lecciones
        foreach (var level in unit.levels)
        {
            int levelTotal = level.lessons.Count * 3;
            int levelCurrent = level.lessons.Sum(l => l.score);

            totalPoints += levelTotal;
            currentPoints += levelCurrent;

            // si algún level no está done todavía, marcamos false
            if (!level.isDone) allLevelsDone = false;
        }

        if (totalPoints == 0)
        {
            unit.unitStats.score = 0;
            unit.unitStats.isDone = false;
            Debug.Log("QualifyCurrentUnit: unit has no lessons (totalPoints == 0).");
            return 0;
        }

        float ratio = (float)currentPoints / (float)totalPoints;
        int result = 0;
        if (ratio >= 1f) result = 3;
        else if (ratio > 2f / 3f) result = 2;
        else if (ratio > 1f / 3f) result = 1;

        // Guardamos la calificación final de la unidad y su estado isDone
        unit.unitStats.score = result;
        unit.unitStats.isDone = allLevelsDone;

        Debug.Log($"Unit qualifying -> totalPoints: {totalPoints}, currentPoints: {currentPoints}, unitScore: {result}, unitDone: {unit.unitStats.isDone}");

        return result;
    }

    int QualifyCurrentSubLevel()
    {
        var unit = LevelProgressManager.Instance.currentUnit[LevelProgressManager.Instance.currentLevel];
        var level = unit.levels[LevelProgressManager.Instance.currentSubLevel];

        int totalPoints = level.lessons.Count * 3;
        int currentPoints = level.lessons.Sum(l => l.score);

        // Evitar división por cero
        if (totalPoints == 0)
        {
            level.score = 0;
            level.isDone = false;
            return 0;
        }

        float ratio = (float)currentPoints / (float)totalPoints;
        int result = 0;
        if (ratio >= 1f) result = 3;
        else if (ratio > 2f / 3f) result = 2;
        else if (ratio > 1f / 3f) result = 1;

        // Actualizamos el subnivel
        level.score = result;
        // Un subnivel se considera done si todas sus lecciones están marcadas isDone
        level.isDone = level.lessons.All(l => l.isDone);

        Debug.Log($"SubLevel qualifying -> totalPoints: {totalPoints}, currentPoints: {currentPoints}, score: {result}, isDone: {level.isDone}");

        return result;
    }
    #endregion

    #region Results Display
    
    
    void ShowUnitQualification(bool firstTime, bool improved)
    {
        var lpm = LevelProgressManager.Instance;
        var unit = lpm.currentUnit[lpm.currentLevel];

        TMP_Text text = UIManager.Instance.theoreticalDetailsPanel.GetComponentInChildren<TMP_Text>();

        if (firstTime)
            text.text = LanguageManager.Instance.GetStringValue("PositiveResponseToolDetailsPanel") + " " + LanguageManager.Instance.GetStringValue(unit.unitStats.moduleName);
        else if (improved)
            text.text = LanguageManager.Instance.GetStringValue("CongratulationsImprovedText") + " " + LanguageManager.Instance.GetStringValue(unit.unitStats.moduleName);
        else
            text.text = LanguageManager.Instance.GetStringValue("NegativeResponseToolDetailsPanel") + " " + LanguageManager.Instance.GetStringValue(unit.unitStats.moduleName);

        // Estrellas según el score de la unidad
        for (int i = 0; i < UIManager.Instance.starsRatingItems.Length; i++)
            UIManager.Instance.starsRatingItems[i].gameObject.SetActive(false);

        for (int i = 0; i < unit.unitStats.score && i < UIManager.Instance.starsRatingItems.Length; i++)
            UIManager.Instance.starsRatingItems[i].gameObject.SetActive(true);

        if (firstTime || improved)
            SoundManager.Instance.PlayNewSound(congratulationsEffect.source);
        else
            SoundManager.Instance.PlayNewSound(condolencesEffect.source);
    }

    void ShowSubLevelQualification(bool firstTime, bool improved)
    {
        var lpm = LevelProgressManager.Instance;
        var sub = lpm.currentUnit[lpm.currentLevel].levels[lpm.currentSubLevel];

        TMP_Text text = UIManager.Instance.theoreticalDetailsPanel.GetComponentInChildren<TMP_Text>();

        if (firstTime)
            text.text = LanguageManager.Instance.GetStringValue("PositiveResponseToolDetailsPanel") + " " + LanguageManager.Instance.GetStringValue(sub.levelName);
        else if (improved)
            text.text = LanguageManager.Instance.GetStringValue("CongratulationsImprovedText") + " " + LanguageManager.Instance.GetStringValue(sub.levelName);
        else
            text.text = LanguageManager.Instance.GetStringValue("NegativeResponseToolDetailsPanel") + " " + LanguageManager.Instance.GetStringValue(sub.levelName);

        // Estrellas según el score del subnivel
        for (int i = 0; i < UIManager.Instance.starsRatingItems.Length; i++)
            UIManager.Instance.starsRatingItems[i].gameObject.SetActive(false);

        for (int i = 0; i < sub.score && i < UIManager.Instance.starsRatingItems.Length; i++)
            UIManager.Instance.starsRatingItems[i].gameObject.SetActive(true);

        // Sonidos
        if (firstTime || improved)
            SoundManager.Instance.PlayNewSound(congratulationsEffect.source);
        else
            SoundManager.Instance.PlayNewSound(condolencesEffect.source);
    }
    
    IEnumerator ShowFeedBackFinalTestResults(bool state)
    {
        UIManager.Instance.moduleResultsMenu.SetActive(true);

        if (state)
        {
            UIManager.Instance.moduleResultsDetailsPanel.GetComponentInChildren<TMP_Text>().text = LanguageManager.Instance.GetStringValue("FinalPositiveTestResultsDetailsPanel");
            SoundManager.Instance.PlayNewSound(congratulationsEffect.source);
        }
        else
        {
            UIManager.Instance.moduleResultsDetailsPanel.GetComponentInChildren<TMP_Text>().text = LanguageManager.Instance.GetStringValue("FinalNegativeTestResultsDetailsPanel");
            SoundManager.Instance.PlayNewSound(condolencesEffect.source);
        }

        yield return new WaitUntil(() => !UIManager.Instance.moduleResultsMenu.activeInHierarchy);
        UIManager.Instance.SetCurrentUIMenu(UIManager.Instance.moduleResultsMenu);
        UIManager.Instance.SetNewUIMenu(UIManager.Instance.mainMenu);
    }

    #endregion
}
