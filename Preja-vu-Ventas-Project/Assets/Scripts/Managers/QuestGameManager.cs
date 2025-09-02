using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class QuestGameManager : MonoBehaviour
{
    public DialogueController textController;
    public TimerController timer;
    public GameObject bodyPanel;
    public GameObject resultsPanel;
    public GameObject observationsPanel;
    public int currentQualification;
    public int newQualification;

    [SerializeField] TMP_Text textTitle;
    [SerializeField] TMP_Text textIndicator;
    [SerializeField] Questionnaire[] questionnaires;
    [SerializeField] List<Button> buttons = new List<Button>();
    [SerializeField] string[] positionIndicator;

    [SerializeField] List<Quest> provisionalQuestionsList;
    [SerializeField] List<string> answersProvisionalList = new List<string>();
    List<int> takeList;
    
    [SerializeField] int indexQuestionnaires;
    [SerializeField] int indexQuestion;
    [SerializeField] AudioSource correctSFX;
    [SerializeField] AudioSource wrongSFX;

    int randomIndex;
    bool startGame;
    bool isAnswered;
    ColorBlock colors;
    Quest currentQuest;

    private void Start()
    {
        GameManager.Instance.questGameManager = this;
    }

    public void SetQuestionnaireIndex()
    {
        indexQuestionnaires = LevelProgressManager.Instance.currentUnit[LevelProgressManager.Instance.currentLevel].levels[LevelProgressManager.Instance.currentSubLevel].lessons[LevelProgressManager.Instance.currentLesson].lessonIndex;
    }

    public void StartGame()
    {
        SetQuestionnaireIndex();
        currentQualification = 0;
        UIManager.Instance.ChanceMusicBackGround(1);
        UIManager.Instance.ChanceMusicBackGround(2);
        UIManager.Instance.timer2DPanel.SetActive(true);

        int position = indexQuestion + 1;
        textIndicator.text = LanguageManager.Instance.GetStringValue("QuestIndicator")+ " " + position +"/3";
        bodyPanel.SetActive(true);
        provisionalQuestionsList = ShuffleQuestions();
        ChangeStateGame();
        StartCoroutine(GetQuestion());

        for (int i = 0; i < questionnaires[indexQuestionnaires].questions.Count; i++)
        {
            questionnaires[indexQuestionnaires].questions[i].isAnswerCorrect = false;
        }
    }
    
    void ChangeStateGame()
    {
        startGame = !startGame;
    }

    IEnumerator GetQuestion()
    {
        ResetButtons();
        currentQuest = provisionalQuestionsList[indexQuestion];

        foreach (string answer in currentQuest.answers)
        {
            string currentTranslateAnswer = LanguageManager.Instance.GetStringValue(answer);
            answersProvisionalList.Add(currentTranslateAnswer);
        }

        //answersProvisionalList = currentQuest.answers.ToList<string>();
        textTitle.text = LanguageManager.Instance.GetStringValue(LevelProgressManager.Instance.currentUnit[LevelProgressManager.Instance.currentLevel].levels[LevelProgressManager.Instance.currentSubLevel].levelName) + " " + LanguageManager.Instance.GetStringValue("QuestionnaireText");
        textController.StartNewDialogue(currentQuest.question);

        yield return new WaitUntil(() => !textController.isPlaying);
        textIndicator.gameObject.SetActive(true);
        UIManager.Instance.timer2DPanel.SetActive(true);
       
        timer.StartTimer(31f);
        RandomButton();
        indexQuestion++;
        
        yield return new WaitUntil(() => !timer.isCounting);

        if (!isAnswered)
        {
            ButtonEvent(false);
        }
    }

    List<Quest> ShuffleQuestions()
    {
        List<Quest> shuffledQuestions;
        shuffledQuestions = questionnaires[indexQuestionnaires].questions.OrderBy(x => Guid.NewGuid()).Take(3).ToList();
        return shuffledQuestions;
    }

    void RandomButton()
    {
        takeList = new List<int>(new int[buttons.Count]);
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].gameObject.SetActive(true);
            randomIndex = UnityEngine.Random.Range(1, buttons.Count + 1);
            while (takeList.Contains(randomIndex))
            {
                randomIndex = UnityEngine.Random.Range(1, buttons.Count + 1);
            }

            takeList[i] = randomIndex;
            string answerEnun = answersProvisionalList[takeList[i] - 1];
            int index = answersProvisionalList.IndexOf(answerEnun);


            SetButtonText(buttons[i], positionIndicator[i], answerEnun);
            AddButtonClickEvent(buttons[i], index);
            SetupButtonColors(buttons[i], index, currentQuest.trueAnswer);
        }
    }

    void SetupButtonColors(Button button, int index, int trueAnswerIndex)
    {
        colors = button.colors;
        colors.highlightedColor = new Color32(36, 96, 214, 255);

        if (index.Equals(trueAnswerIndex))
        {
            colors.pressedColor = new Color32(0, 255, 0, 255);
        }
        else
        {
            colors.pressedColor = new Color32(255, 0, 0, 255);
        }

        button.colors = colors;
    }

    void AddButtonClickEvent(Button button, int index)
    {
        button.onClick.AddListener(() => EventClickButton(index));
    }

    void SetButtonText(Button button, string enumText, string answer)
    {
        button.transform.GetChild(0).transform.GetComponentInChildren<TMP_Text>().text = (enumText + " " + answer);
    }

    void EventClickButton(int indexButton)
    {
        timer.isCounting = false;
        isAnswered = true;
        
        if (indexButton.Equals(currentQuest.trueAnswer))
        {
            ButtonEvent(true);        
        }
        else
        {
            ButtonEvent(false);
        }
    }

    void ButtonEvent(bool value)
    {
        if (value)
        {
            correctSFX.Play();
            currentQuest.isAnswerCorrect = true;
            bodyPanel.SetActive(false);
            observationsPanel.SetActive(true);
            observationsPanel.GetComponentInChildren<TMP_Text>().color = new Color32(0, 255, 0, 255);
            observationsPanel.GetComponentInChildren<TMP_Text>().text = LanguageManager.Instance.GetStringValue("TrueAnswerText");
        }
        else
        {
            wrongSFX.Play();
            bodyPanel.SetActive(false);
            observationsPanel.SetActive(true);
            observationsPanel.GetComponentInChildren<TMP_Text>().color = new Color32(255, 0, 0, 255);
            observationsPanel.GetComponentInChildren<TMP_Text>().text = LanguageManager.Instance.GetStringValue("WrongAnswerText") + answersProvisionalList[currentQuest.trueAnswer];
        }
    }

    void ResetButtons()
    {
        UIManager.Instance.timer2DPanel.SetActive(false);
        textIndicator.gameObject.SetActive(false);
        textTitle.text = "";
        isAnswered = false;
        answersProvisionalList.Clear();

        observationsPanel.GetComponentInChildren<TMP_Text>().color = Color.black;

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].transform.GetChild(0).transform.GetComponentInChildren<TMP_Text>().text = "";
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].gameObject.SetActive(false);
        }   
    }
    
    public void Validate()
    {
        int position = indexQuestion + 1;
        textIndicator.text = LanguageManager.Instance.GetStringValue("QuestIndicator") + " " + position + "/3";

        if (indexQuestion == provisionalQuestionsList.Count)
        {
            bodyPanel.SetActive(false);
            QualityTest();
        }
        else
        {
            bodyPanel.SetActive(true);
            StartCoroutine(GetQuestion());
        }
    }

    void QualityTest()
    {
        ChangeStateGame();
        newQualification = QualificationModule();
        bodyPanel.SetActive(false);
        resultsPanel.SetActive(true);
        
       // int currentLessonListPosition = ModuleUIHelper.GetLessonTypeIndex(LevelProgressManager.Instance.currentUnit[LevelProgressManager.Instance.currentLevel].levels[LevelProgressManager.Instance.currentSubLevel].lessons, LessonType.Quiz);
        currentQualification = LevelProgressManager.Instance.currentUnit[LevelProgressManager.Instance.currentLevel].levels[LevelProgressManager.Instance.currentSubLevel].lessons[LevelProgressManager.Instance.currentLesson].score;

        if (newQualification > currentQualification)
        {
            LevelProgressManager.Instance.currentUnit[LevelProgressManager.Instance.currentLevel].levels[LevelProgressManager.Instance.currentSubLevel].lessons[LevelProgressManager.Instance.currentLesson].score = newQualification;
        }

        if (newQualification > (provisionalQuestionsList.Count/ 2))
        {
            if (!LevelProgressManager.Instance.currentUnit[LevelProgressManager.Instance.currentLevel].levels[LevelProgressManager.Instance.currentSubLevel].lessons[LevelProgressManager.Instance.currentLesson].isDone)
            {
                LevelProgressManager.Instance.currentUnit[LevelProgressManager.Instance.currentLevel].levels[LevelProgressManager.Instance.currentSubLevel].lessons[LevelProgressManager.Instance.currentLesson].isDone = true;
            }

            resultsPanel.GetComponentInChildren<TMP_Text>().text = LanguageManager.Instance.GetStringValue("PassedText") + " " + newQualification + " " +  LanguageManager.Instance.GetStringValue("PointsText") + " " + provisionalQuestionsList.Count;
        }
        else
        {
            resultsPanel.GetComponentInChildren<TMP_Text>().text = LanguageManager.Instance.GetStringValue("FailedText") + " " + newQualification + " " + LanguageManager.Instance.GetStringValue("PointsText") + " "+ provisionalQuestionsList.Count;
        }
    }

    private int QualificationModule()
    {
        int indexCalificationModule = 0;

        for (int i = 0; i < provisionalQuestionsList.Count; i++)
        {
            if (provisionalQuestionsList[i].isAnswerCorrect)
            {
                indexCalificationModule++;
            }
        }

        return indexCalificationModule;
    }

    public void End()
    {
        Debug.Log("End Quiz");
        textController.EndDialogue();
        UIManager.Instance.ChanceMusicBackGround(3);
        UIManager.Instance.ChanceMusicBackGround(0);

        ResetButtons();

        answersProvisionalList.Clear();
        provisionalQuestionsList.Clear();
        indexQuestion = 0;
        randomIndex = 0;
        currentQuest = null;
        observationsPanel.SetActive(false);
        bodyPanel.SetActive(false);
        resultsPanel.SetActive(false);
    }
}
