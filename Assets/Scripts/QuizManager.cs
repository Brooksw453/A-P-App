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
    public bool HasBeenAsked => hasBeenAsked;
    public bool UserGotItRight => userGotItRight;
 

}

public class QuizManager : MonoBehaviour
{
    [SerializeField] private List<Question> questions = new List<Question>();
    private List<Question> selectedQuestions = new List<Question>();

    [SerializeField] private TextMeshProUGUI scoreText;
    private bool quizFinished = false;
    private int score = 0;
    private int currentQuestionIndex = 0;
    public GameObject correctScreen;
    public GameObject incorrectScreen;
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    private AudioSource audioSource; 
    public GameObject correctAnswerParticleEffect;
    public static QuizManager ActiveQuiz { get; private set; }
    public GameObject reloadQuizPanel; // Assign the "Reload Quiz" panel in the inspector


    private void Start()
    {
        if (ActiveQuiz != null && ActiveQuiz != this)
    {
        // Some other quiz is already running
        Debug.LogWarning("Another quiz is active. Disabling this one.");
        this.enabled = false;
        return;
    }

    ActiveQuiz = this;    
    audioSource = GetComponent<AudioSource>(); // Ensure you have an AudioSource component attached
        if (audioSource == null)
        {
            gameObject.AddComponent<AudioSource>();
            audioSource = GetComponent<AudioSource>();
        }
        InitializeQuiz();
        DisplayNextQuestion();
    }
    private void Awake()
{
    if(ActiveQuiz == null)
    {
        ActiveQuiz = this;
    }
    else
    {
        Debug.LogWarning("Multiple quizzes trying to become active. Overriding with latest.");
        ActiveQuiz = this;
    }
}
 private void InitializeQuiz()
{
    Shuffle(questions);
    Debug.Log("Initializing quiz. Total questions: " + questions.Count);

    for (int i = 0; i < 5; i++)
    {
        Debug.Log("Adding question #" + i);
        selectedQuestions.Add(questions[i]);
    }
}


    public void DisplayNextQuestion()
    {
        if (currentQuestionIndex >= selectedQuestions.Count)
    {
        quizFinished = true;  // Update the flag here
        FinishQuiz();
        return;
    }    
        if (currentQuestionIndex >= selectedQuestions.Count)
        {
            FinishQuiz();
            return;
        }

        foreach (var q in selectedQuestions)
        {
            q.QuestionCanvas.SetActive(false);
        }

        selectedQuestions[currentQuestionIndex].QuestionCanvas.SetActive(true);
        currentQuestionIndex++;
    }

    public void AnswerQuestion(int userAnswer)
    {
        if (quizFinished) return;

        Question currentQuestion = selectedQuestions[currentQuestionIndex - 1];
        
        if (userAnswer == currentQuestion.CorrectAnswer)
        {
            score += 20;
            UpdateScoreDisplay();
            ShowCorrectAnswerFeedback();
        }
        else
        {
            ShowIncorrectAnswerFeedback();
        }

        // Wait for 1 second (via coroutine) before displaying the next question
        StartCoroutine(WaitAndDisplayNext());
    }

    IEnumerator WaitAndDisplayNext()
    {
        yield return new WaitForSeconds(1);
        DisplayNextQuestion();
    }

    void ShowCorrectAnswerFeedback()
    {
    correctScreen.SetActive(true);
    audioSource.PlayOneShot(correctSound);

    // Instantiate the particle effect at the correctScreen's position
    GameObject particleInstance = Instantiate(correctAnswerParticleEffect, correctScreen.transform.position, Quaternion.identity);
    Destroy(particleInstance, 5f); // Destroy the instance after 5 seconds. Adjust time as needed.

    // Optionally disable the screen after some time
    StartCoroutine(DisableAfterDelay(correctScreen, 1f));
    }

    void ShowIncorrectAnswerFeedback()
    {
        incorrectScreen.SetActive(true);
        audioSource.PlayOneShot(incorrectSound);
        // Optionally disable the screen after some time
        StartCoroutine(DisableAfterDelay(incorrectScreen, 1f));
    }

    IEnumerator DisableAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    public void ReloadQuiz()
    {
    // 1. Reset score
    score = 0;
    UpdateScoreDisplay();
     reloadQuizPanel.SetActive(false);

    // 2. Turn off all currently active questions
    foreach (var q in selectedQuestions)
    {
        q.QuestionCanvas.SetActive(false);
    }

    // 3. Clear selected questions list
    selectedQuestions.Clear();

    // 4. Shuffle and select new set of 5 random questions
    Shuffle(questions);
    for (int i = 0; i < 5; i++)
    {
        selectedQuestions.Add(questions[i]);
    }
    ActiveQuiz = this;

    // 5. Reset question index
    currentQuestionIndex = 0;

    // 6. Display the first question
    DisplayNextQuestion();

    // Also reset the quizFinished flag
    quizFinished = false;
    }

    private void UpdateScoreDisplay()
    {
        scoreText.text = "Score: " + score;
    }


    private void FinishQuiz()
    {
        ActiveQuiz = null;    
    Debug.Log("Quiz Finished!");
        // You can expand this later to handle the end of the quiz
            // Activate the "Reload Quiz" panel
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
