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
        scoreText.text = "거리 : " + ScoreManager.Instance.GetCurrentScore() + "m";
    }

    public void UpdateHighScoreText()
    {
        highScoreText.text = "최장 거리 : " + ScoreManager.Instance.GetHighScore() + "m";
    }

    public void UpdateSpeedText()
    {
        speedText.text = "속도 : " + SpeedManager.Instance.GetCurrentSpeed().ToString("F1");
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);

        if (ScoreManager.Instance != null)
        {
            finalScoreText.text = "달린 거리 : " + ScoreManager.Instance.GetCurrentScore() + 'm';
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