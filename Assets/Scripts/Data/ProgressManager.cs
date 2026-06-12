using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    private const string BestScorePrefix = "BestScore_";
    private const string AttemptsPrefix = "Attempts_";

    public static ProgressManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveQuizResult(string quizName, int score, int maxScore)
    {
        int attempts = PlayerPrefs.GetInt(AttemptsPrefix + quizName, 0) + 1;
        PlayerPrefs.SetInt(AttemptsPrefix + quizName, attempts);

        int bestScore = PlayerPrefs.GetInt(BestScorePrefix + quizName, 0);
        if (score > bestScore)
        {
            PlayerPrefs.SetInt(BestScorePrefix + quizName, score);
        }

        PlayerPrefs.Save();
    }

    public int GetBestScore(string quizName)
    {
        return PlayerPrefs.GetInt(BestScorePrefix + quizName, 0);
    }

    public int GetAttemptCount(string quizName)
    {
        return PlayerPrefs.GetInt(AttemptsPrefix + quizName, 0);
    }

    public void ClearProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
