using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CatchGameState
{
    Ready,
    Playing,
    GameOver
}

public class CatchGameManager : MonoBehaviour
{
    public static CatchGameManager Instance { get; private set; }

    [Header("Spawner")]
    [SerializeField] private CatchObjectSpawner objectSpawner;

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Countdown")]
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int countdownSeconds = 3;
    [SerializeField] private float initialDelay = 0.4f;
    [SerializeField] private float countdownStepDuration = 0.9f;
    [SerializeField] private float startTextDuration = 0.7f;

    [Header("Countdown Animation")]
    [SerializeField] private float countdownPopScale = 1.6f;
    [SerializeField] private Color numberColor = Color.yellow;
    [SerializeField] private Color startColor = Color.green;

    [Header("Difficulty")]
    [SerializeField] private float difficultyUpTime = 15f;
    [SerializeField] private float baseFallSpeed = 2.5f;
    [SerializeField] private float fallSpeedIncrease = 0.4f;
    [SerializeField] private float baseSpawnInterval = 1.2f;
    [SerializeField] private float spawnIntervalDecrease = 0.08f;
    [SerializeField] private float minSpawnInterval = 0.45f;

    public bool IsGameOver =>
        currentState == CatchGameState.GameOver;

    private CatchGameState currentState = CatchGameState.Ready;

    private int score;
    private int level = 1;
    private float elapsedTime;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        StartCoroutine(CountdownRoutine());
    }

    private void Update()
    {
        if (!IsPlaying())
        {
            return;
        }

        elapsedTime += Time.deltaTime;

        int newLevel =
            1 + Mathf.FloorToInt(elapsedTime / difficultyUpTime);

        if (newLevel != level)
        {
            level = newLevel;
            UpdateUI();
        }
    }

    private IEnumerator CountdownRoutine()
    {
        PrepareGame();

        // 첫 화면이 그려질 때까지 숫자를 숨깁니다.
        if (countdownText != null)
        {
            countdownText.text = string.Empty;
            countdownText.color = Color.clear;
        }

        yield return null;

        Canvas.ForceUpdateCanvases();

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForSecondsRealtime(initialDelay);

        for (int count = countdownSeconds; count > 0; count--)
        {
            yield return AnimateCountdownText(
                count.ToString(),
                numberColor,
                countdownStepDuration
            );
        }

        // START가 나타날 때 실제 게임을 시작합니다.
        StartGame();

        yield return AnimateCountdownText(
            "START!",
            startColor,
            startTextDuration
        );

        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }
    }

    private void PrepareGame()
    {
        currentState = CatchGameState.Ready;

        score = 0;
        level = 1;
        elapsedTime = 0f;

        if (objectSpawner != null)
        {
            objectSpawner.StopSpawning();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (countdownPanel != null)
        {
            countdownPanel.SetActive(true);
        }

        UpdateUI();
    }

    private void StartGame()
    {
        currentState = CatchGameState.Playing;

        if (objectSpawner != null)
        {
            objectSpawner.StartSpawning();
        }
    }

    private IEnumerator AnimateCountdownText(
        string message,
        Color color,
        float duration)
    {
        if (countdownText == null)
        {
            yield break;
        }

        RectTransform textTransform = countdownText.rectTransform;

        countdownText.text = message;

        Color textColor = color;
        textColor.a = 1f;
        countdownText.color = textColor;

        textTransform.localScale =
            Vector3.one * countdownPopScale;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Scene 로딩 직후 큰 시간 차이가 생기는 것을 방지합니다.
            float deltaTime = Mathf.Min(
                Time.unscaledDeltaTime,
                0.05f
            );

            elapsed += deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            float easedProgress =
                1f - Mathf.Pow(1f - progress, 3f);

            float scale = Mathf.Lerp(
                countdownPopScale,
                1f,
                easedProgress
            );

            textTransform.localScale = Vector3.one * scale;

            float alpha = 1f;

            if (progress > 0.7f)
            {
                alpha = Mathf.InverseLerp(
                    1f,
                    0.7f,
                    progress
                );
            }

            textColor.a = alpha;
            countdownText.color = textColor;

            yield return null;
        }

        textTransform.localScale = Vector3.one;

        textColor.a = 0f;
        countdownText.color = textColor;
    }

    public bool IsPlaying()
    {
        return currentState == CatchGameState.Playing;
    }

    public void AddScore(int amount)
    {
        if (!IsPlaying())
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

        currentState = CatchGameState.GameOver;

        if (objectSpawner != null)
        {
            objectSpawner.StopSpawning();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "최종 점수 : " + score;
        }
    }

    public void OnClickRestart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }

    public float GetFallSpeed()
    {
        return baseFallSpeed +
               ((level - 1) * fallSpeedIncrease);
    }

    public float GetSpawnInterval()
    {
        float interval =
            baseSpawnInterval -
            ((level - 1) * spawnIntervalDecrease);

        return Mathf.Max(interval, minSpawnInterval);
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "점수 : " + score;
        }

        if (levelText != null)
        {
            levelText.text = "레벨 : " + level;
        }
    }
}