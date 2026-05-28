using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI speedText;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HideGameOver();
    }

    private void Update()
    {
        if (ScoreManager.Instance != null)
        {
            UpdateScoreText();
            UpdateHighScoreText();
        }

        if (SpeedManager.Instance != null)
        {
            UpdateSpeedText();
        }
    }

    public void UpdateScoreText()
    {
        scoreText.text = "Score : " + ScoreManager.Instance.GetCurrentScore();
    }

    public void UpdateHighScoreText()
    {
        highScoreText.text = "High Score : " + ScoreManager.Instance.GetHighScore();
    }

    public void UpdateSpeedText()
    {
        speedText.text = "Speed : " + SpeedManager.Instance.GetCurrentSpeed().ToString("F1");
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);

        if (ScoreManager.Instance != null)
        {
            finalScoreText.text = "Final Score : " + ScoreManager.Instance.GetCurrentScore();
        }
    }

    public void HideGameOver()
    {
        gameOverPanel.SetActive(false);
    }

    public void OnClickRestart()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    public void OnClickTitle()
    {
        SceneManager.LoadScene("MainScene");
    }
}