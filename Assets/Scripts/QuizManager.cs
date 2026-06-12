using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Question
{
    [SerializeField] private GameObject questionCanvas;
    [SerializeField] private int correctAnswer;
    [SerializeField] private bool hasBeenAsked = false;
    [SerializeField] private bool userGotItRight = false;

    public GameObject QuestionCanvas => questionCanvas;
    public int CorrectAnswer => correctAnswer;
    public bool HasBeenAsked { get => hasBeenAsked; set => hasBeenAsked = value; }
    public bool UserGotItRight { get => userGotItRight; set => userGotItRight = value; }
}

public class QuizManager : MonoBehaviour
{
    [Header("Quiz Configuration")]
    [SerializeField] private List<Question> questions = new List<Question>();
    [SerializeField] private QuizData quizData;
    [SerializeField] private int questionsPerRound = 5;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI questionCountText;
    public GameObject correctScreen;
    public GameObject incorrectScreen;
    public GameObject reloadQuizPanel;

    [Header("Audio")]
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    private AudioSource audioSource;

    [Header("Effects")]
    public GameObject correctAnswerParticleEffect;

    public static QuizManager ActiveQuiz { get; private set; }

    private List<Question> selectedQuestions = new List<Question>();
    private bool quizFinished = false;
    private int score = 0;
    private int currentQuestionIndex = 0;
    private int pointsPerCorrect = 20;

    private void Awake()
    {
        ActiveQuiz = this;
    }

    private void Start()
    {
        if (ActiveQuiz != null && ActiveQuiz != this)
        {
            Debug.LogWarning("Another quiz is active. Disabling this one.");
            this.enabled = false;
            return;
        }

        ActiveQuiz = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (quizData != null)
        {
            questionsPerRound = quizData.questionsPerRound;
            pointsPerCorrect = quizData.pointsPerCorrectAnswer;
        }

        InitializeQuiz();
        DisplayNextQuestion();
    }

    private void InitializeQuiz()
    {
        selectedQuestions.Clear();
        Shuffle(questions);

        int count = Mathf.Min(questionsPerRound, questions.Count);
        for (int i = 0; i < count; i++)
        {
            questions[i].HasBeenAsked = false;
            questions[i].UserGotItRight = false;
            selectedQuestions.Add(questions[i]);
        }
    }

    public void DisplayNextQuestion()
    {
        if (currentQuestionIndex >= selectedQuestions.Count)
        {
            quizFinished = true;
            FinishQuiz();
            return;
        }

        foreach (var q in selectedQuestions)
        {
            q.QuestionCanvas.SetActive(false);
        }

        selectedQuestions[currentQuestionIndex].QuestionCanvas.SetActive(true);
        UpdateQuestionCountDisplay();
        currentQuestionIndex++;
    }

    public void AnswerQuestion(int userAnswer)
    {
        if (quizFinished) return;

        Question currentQuestion = selectedQuestions[currentQuestionIndex - 1];
        currentQuestion.HasBeenAsked = true;

        if (userAnswer == currentQuestion.CorrectAnswer)
        {
            currentQuestion.UserGotItRight = true;
            score += pointsPerCorrect;
            UpdateScoreDisplay();
            ShowCorrectAnswerFeedback();
        }
        else
        {
            currentQuestion.UserGotItRight = false;
            ShowIncorrectAnswerFeedback();
        }

        StartCoroutine(WaitAndDisplayNext());
    }

    private IEnumerator WaitAndDisplayNext()
    {
        yield return new WaitForSeconds(1);
        DisplayNextQuestion();
    }

    private void ShowCorrectAnswerFeedback()
    {
        correctScreen.SetActive(true);
        audioSource.PlayOneShot(correctSound);

        if (correctAnswerParticleEffect != null)
        {
            GameObject particleInstance = Instantiate(correctAnswerParticleEffect, correctScreen.transform.position, Quaternion.identity);
            Destroy(particleInstance, 5f);
        }

        StartCoroutine(DisableAfterDelay(correctScreen, 1f));
    }

    private void ShowIncorrectAnswerFeedback()
    {
        incorrectScreen.SetActive(true);
        audioSource.PlayOneShot(incorrectSound);
        StartCoroutine(DisableAfterDelay(incorrectScreen, 1f));
    }

    private IEnumerator DisableAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    public void ReloadQuiz()
    {
        score = 0;
        UpdateScoreDisplay();
        reloadQuizPanel.SetActive(false);

        foreach (var q in selectedQuestions)
        {
            q.QuestionCanvas.SetActive(false);
        }

        currentQuestionIndex = 0;
        quizFinished = false;
        ActiveQuiz = this;

        InitializeQuiz();
        DisplayNextQuestion();
    }

    private void UpdateScoreDisplay()
    {
        scoreText.text = "Score: " + score;
    }

    private void UpdateQuestionCountDisplay()
    {
        if (questionCountText != null)
            questionCountText.text = (currentQuestionIndex + 1) + " / " + selectedQuestions.Count;
    }

    private void FinishQuiz()
    {
        string quizName = quizData != null ? quizData.quizName : gameObject.name;
        int maxScore = selectedQuestions.Count * pointsPerCorrect;

        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.SaveQuizResult(quizName, score, maxScore);

            if (bestScoreText != null)
                bestScoreText.text = "Best: " + ProgressManager.Instance.GetBestScore(quizName);
        }

        ActiveQuiz = null;
        reloadQuizPanel.SetActive(true);
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        System.Random rng = new System.Random();
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
