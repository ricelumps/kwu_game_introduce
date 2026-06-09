using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score Settings")]
    [SerializeField] private float baseScorePerSecond = 0.5f;
    [SerializeField] private float speedScoreMultiplier = 0.75f;

    private float currentScore;
    private int highScore;

    private const string HighScoreKey = "HighScore";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadHighScore();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPlaying())
        {
            AddSurvivalScore();
        }
    }

    public void AddSurvivalScore()
    {
        float speedBonus = SpeedManager.Instance.GetCurrentSpeed() * speedScoreMultiplier;
        currentScore += (baseScorePerSecond + speedBonus) * Time.deltaTime;
    }

    public void AddBonusScore(int amount)
    {
        currentScore += amount;


    }

    public void SaveHighScore()
    {
        int score = GetCurrentScore();

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }
    }

    public void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public void ResetScore()
    {
        currentScore = 0f;
    }

    public int GetCurrentScore()
    {
        return Mathf.FloorToInt(currentScore);
    }

    public int GetHighScore()
    {
        return highScore;
    }
}