using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // This line is for TextMeshPro

[System.Serializable]
public class QuestionDetails
{
    public GameObject questionUI; // Drag your UI GameObject here
    public Transform correctTouchPoint;
    public GameObject infoUIPanel; // Drag the UI Panel to show additional info here
}

public class BoneQuizManager : MonoBehaviour
{
    public List<QuestionDetails> questions = new List<QuestionDetails>();
    public TextMeshProUGUI scoreText; // This line is modified for TextMeshPro

    private int currentScore = 0;
    private int currentQuestionIndex = 0;

    private void Start()
    {
        ShowQuestion(currentQuestionIndex);
    }

    private void ShowQuestion(int index)
    {
        if (index < questions.Count)
        {
            foreach (var q in questions)
            {
                q.questionUI.SetActive(false); // Hide all questions
            }

            questions[index].questionUI.SetActive(true); // Show current question
        }
    }

    // This method can be called by the sphere touchpoints (using a collider and raycast for example)
    public void CheckAnswer(Transform selectedTouchPoint)
    {
        if (selectedTouchPoint == questions[currentQuestionIndex].correctTouchPoint)
        {
            // Answer is correct
            currentScore += 20;
            scoreText.text = "Score: " + currentScore.ToString();

            // Show the additional info UI Panel
            questions[currentQuestionIndex].infoUIPanel.SetActive(true);
            StartCoroutine(ShowNextQuestionAfterDelay());
        }
        else
        {
            // Answer is incorrect, directly show the next question
            ShowNextQuestion();
        }
    }

    private IEnumerator ShowNextQuestionAfterDelay()
    {
        yield return new WaitForSeconds(3); // Wait for 10 seconds, you can adjust this
        questions[currentQuestionIndex].infoUIPanel.SetActive(false); // Hide the info panel
        ShowNextQuestion();
    }

    private void ShowNextQuestion()
    {
        currentQuestionIndex++;
        if (currentQuestionIndex < questions.Count)
        {
            ShowQuestion(currentQuestionIndex);
        }
        else
        {
            // Quiz completed, you can show some final UI or restart the quiz
            RestartQuiz();
        }
    }

    public void RestartQuiz()
    {
        currentScore = 0;
        currentQuestionIndex = 0;
        scoreText.text = "Score: 0";
        ShowQuestion(currentQuestionIndex);
    }
}



