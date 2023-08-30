using System.Collections.Generic;
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
    private int score = 0;
    private int currentQuestionIndex = 0;

    private void Start()
    {
        InitializeQuiz();
        DisplayNextQuestion();
    }

    private void InitializeQuiz()
    {
        Shuffle(questions);

        for (int i = 0; i < 5; i++)
        {
            selectedQuestions.Add(questions[i]);
        }
    }

    public void DisplayNextQuestion()
    {
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

    public void ReloadQuiz()
    {
    // 1. Reset score
    score = 0;
    UpdateScoreDisplay();

    // 2. Clear selected questions list
    selectedQuestions.Clear();

    // 3. Shuffle and select new set of 5 random questions
    Shuffle(questions);
    for (int i = 0; i < 5; i++)
    {
        selectedQuestions.Add(questions[i]);
    }

    // 4. Reset question index
    currentQuestionIndex = 0;

    // 5. Display the first question
    DisplayNextQuestion();

    // Also reset the quizFinished flag
    quizFinished = false;
    }

    public void AnswerQuestion(int userAnswer)
    {
        Question currentQuestion = selectedQuestions[currentQuestionIndex - 1];

        if (userAnswer == currentQuestion.CorrectAnswer)
        {
            score += 20;
            UpdateScoreDisplay();
            // Assuming you'll implement the userGotItRight property later
            // currentQuestion.userGotItRight = true;  
        }

        DisplayNextQuestion();
    }

    private void UpdateScoreDisplay()
    {
        scoreText.text = "Score: " + score;
    }

    private void FinishQuiz()
    {
        Debug.Log("Quiz Finished!");
        // You can expand this later to handle the end of the quiz
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
