using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuiz", menuName = "A&P Lab/Quiz Data")]
public class QuizData : ScriptableObject
{
    public string quizName;
    public int questionsPerRound = 5;
    public int pointsPerCorrectAnswer = 20;
    public List<QuestionData> questions = new List<QuestionData>();
}

[System.Serializable]
public class QuestionData
{
    public string questionText;
    public List<string> answerOptions = new List<string>();
    public int correctAnswerIndex;
}
