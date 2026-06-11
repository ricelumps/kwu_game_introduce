using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,
    Playing,
    Dying,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Spawner")]
    [SerializeField] private ObstacleSpawner obstacleSpawner;

    [Header("Countdown")]
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int countdownSeconds = 3;
    [SerializeField] private float initialDelay = 0.4f;
    [SerializeField] private float startTextDuration = 0.7f;

    [Header("Countdown Animation")]
    [SerializeField] private float countdownPopScale = 1.6f;
    [SerializeField] private float countdownStepDuration = 0.9f;
    [SerializeField] private Color numberColor = Color.yellow;
    [SerializeField] private Color startColor = Color.green;

    private GameState currentState = GameState.Ready;

    private void Awake()
    {
        Instance = this;

        // 이전 Scene에서 시간이 멈췄을 경우를 대비합니다.
        Time.timeScale = 1f;
    }

    private void Start()
    {
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        PrepareGame();

        // Scene이 처음 화면에 표시될 때까지 숫자를 숨깁니다.
        if (countdownText != null)
        {
            countdownText.text = string.Empty;
            countdownText.color = Color.clear;
        }

        // Scene 오브젝트와 UI가 초기화될 시간을 줍니다.
        yield return null;

        Canvas.ForceUpdateCanvases();

        // 첫 화면 렌더링 완료까지 기다립니다.
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // 게임 화면을 잠깐 확인한 뒤 3을 표시합니다.
        yield return new WaitForSecondsRealtime(initialDelay);

        for (int count = countdownSeconds; count > 0; count--)
        {
            yield return AnimateCountdownText(
                count.ToString(),
                numberColor,
                countdownStepDuration
            );
        }

        // START가 나타나는 순간 실제 게임을 시작합니다.
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
        currentState = GameState.Ready;

        if (countdownPanel != null)
        {
            countdownPanel.SetActive(true);
        }

        if (obstacleSpawner != null)
        {
            obstacleSpawner.StopSpawning();
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }

        if (SpeedManager.Instance != null)
        {
            SpeedManager.Instance.ResetSpeed();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideGameOver();
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

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Scene 로딩 직후 프레임 지연으로 숫자를 건너뛰지 않게 합니다.
            float deltaTime = Mathf.Min(
                Time.unscaledDeltaTime,
                0.05f
            );

            elapsedTime += deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / duration
            );

            // 크게 등장한 숫자가 부드럽게 작아집니다.
            float easedProgress =
                1f - Mathf.Pow(1f - progress, 3f);

            float scale = Mathf.Lerp(
                countdownPopScale,
                1f,
                easedProgress
            );

            textTransform.localScale =
                Vector3.one * scale;

            // 마지막 30% 동안 사라집니다.
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

    public void StartGame()
    {
        if (currentState != GameState.Ready)
        {
            return;
        }

        currentState = GameState.Playing;

        if (obstacleSpawner != null)
        {
            obstacleSpawner.StartSpawning();
        }

        Debug.Log("러닝 게임 시작");
    }

    public void BeginDeathSequence()
    {
        if (currentState != GameState.Playing)
        {
            return;
        }

        currentState = GameState.Dying;

        if (obstacleSpawner != null)
        {
            obstacleSpawner.StopSpawning();
        }
    }

    public void GameOver()
    {
        if (currentState == GameState.GameOver)
        {
            return;
        }

        currentState = GameState.GameOver;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SaveHighScore();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("RunningGameScene");
    }

    public bool IsPlaying()
    {
        return currentState == GameState.Playing;
    }
}