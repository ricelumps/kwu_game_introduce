using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CatchGameManager : MonoBehaviour
{
    public static CatchGameManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Difficulty")]
    [SerializeField] private float difficultyUpTime = 15f;
    [SerializeField] private float baseFallSpeed = 2.5f;
    [SerializeField] private float fallSpeedIncrease = 0.4f;
    [SerializeField] private float baseSpawnInterval = 1.2f;
    [SerializeField] private float spawnIntervalDecrease = 0.08f;
    [SerializeField] private float minSpawnInterval = 0.45f;

    public bool IsGameOver { get; private set; }

    private int score;
    private int level = 1;
    private float elapsedTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        IsGameOver = false;
        gameOverPanel.SetActive(false);
        UpdateUI();
    }

    private void Update()
    {
        if (IsGameOver)
        {
            return;
        }

        elapsedTime += Time.deltaTime;

        int newLevel = 1 + Mathf.FloorToInt(elapsedTime / difficultyUpTime);

        if (newLevel != level)
        {
            level = newLevel;
            UpdateUI();
        }
    }

    public void AddScore(int amount)
    {
        if (IsGameOver)
        {
            return;
        }

        score += amount;
        UpdateUI();
    }

    public void GameOver()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        gameOverPanel.SetActive(true);
        finalScoreText.text = "Final Score: " + score;
    }

    public void OnClickRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickTitle()
    {
        SceneManager.LoadScene("MainScene");
    }

    public float GetFallSpeed()
    {
        return baseFallSpeed + ((level - 1) * fallSpeedIncrease);
    }

    public float GetSpawnInterval()
    {
        float interval =
            baseSpawnInterval - ((level - 1) * spawnIntervalDecrease);

        return Mathf.Max(interval, minSpawnInterval);
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        levelText.text = "Level: " + level;
    }
}