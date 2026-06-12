using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("Lives")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private Image[] lifeImages;
    [SerializeField] private Sprite lifeImage;
    [SerializeField] private Sprite emptyLifeImage;

    [Header("Life Lost Animation")]
    [SerializeField] private float lifeAnimationDuration = 0.4f;
    [SerializeField] private float lifePunchScale = 1.5f;
    [SerializeField] private float lifeShakeAngle = 15f;
    [SerializeField] private Color lifeDamageColor = Color.red;

    [Header("Score Animation")]
    [SerializeField] private float scoreAnimationDuration = 0.25f;
    [SerializeField] private float scorePunchScale = 1.4f;
    [SerializeField] private Color scoreGainColor = Color.yellow;

    private Coroutine scoreAnimationRoutine;


    [Header("Score Popup")]
    [SerializeField] private ScorePopup scorePopupPrefab;
    [SerializeField] private RectTransform popupContainer;
    [SerializeField] private Canvas gameCanvas;


    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip plusCatchSound;
    [SerializeField] private AudioClip minusCatchSound;

    private int currentLives;





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
        currentLives = maxLives;

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

        if (scoreAnimationRoutine != null)
        {
            StopCoroutine(scoreAnimationRoutine);
        }

        scoreAnimationRoutine =
            StartCoroutine(AnimateScoreText(amount));
    }

    private IEnumerator AnimateScoreText(int amount)
    {
        if (scoreText == null)
        {
            yield break;
        }

        RectTransform textTransform = scoreText.rectTransform;

        Vector3 originalScale = textTransform.localScale;
        Vector2 originalPosition = textTransform.anchoredPosition;
        Color originalColor = scoreText.color;

        float elapsed = 0f;

        while (elapsed < scoreAnimationDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsed / scoreAnimationDuration
            );

            // 0 → 1 → 0 형태로 커졌다가 돌아옵니다.
            float punch = Mathf.Sin(progress * Mathf.PI);
            float scale = Mathf.Lerp(1f, scorePunchScale, punch);

            textTransform.localScale = originalScale * scale;

            // 위치를 고정하여 튕기거나 밀리지 않게 합니다.
            textTransform.anchoredPosition = originalPosition;

            scoreText.color = Color.Lerp(
                scoreGainColor,
                originalColor,
                progress
            );

            yield return null;
        }

        textTransform.localScale = originalScale;
        textTransform.anchoredPosition = originalPosition;
        scoreText.color = originalColor;

        scoreAnimationRoutine = null;
    }

    public void ShowScorePopup(int amount, Vector3 worldPosition)
    {
        if (scorePopupPrefab == null ||
            popupContainer == null ||
            gameCanvas == null)
        {
            return;
        }

        Vector2 screenPosition =
            Camera.main.WorldToScreenPoint(worldPosition);

        Camera uiCamera = gameCanvas.renderMode ==
            RenderMode.ScreenSpaceOverlay
                ? null
                : gameCanvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            popupContainer,
            screenPosition,
            uiCamera,
            out Vector2 localPosition
        );

        ScorePopup popup = Instantiate(
            scorePopupPrefab,
            popupContainer
        );

        popup.GetComponent<RectTransform>().anchoredPosition =
            localPosition;

        popup.Play(amount);
    }


    public void LoseLife()
    {
        if (!IsPlaying())
        {
            return;
        }

        currentLives = Mathf.Max(0, currentLives - 1);

        // 감소한 목숨 위치입니다.
        int lostLifeIndex = currentLives;

        UpdateUI();

        if (lostLifeIndex >= 0 &&
            lostLifeIndex < lifeImages.Length &&
            lifeImages[lostLifeIndex] != null)
        {
            StartCoroutine(
                AnimateLostLife(lifeImages[lostLifeIndex])
            );
        }

        if (currentLives <= 0)
        {
            GameOver();
        }
    }



    private IEnumerator AnimateLostLife(Image lifeImageUI)
    {
        RectTransform heartTransform = lifeImageUI.rectTransform;

        Vector3 originalScale = Vector3.one;
        Quaternion originalRotation = Quaternion.identity;
        Color originalColor = Color.white;

        float elapsed = 0f;

        lifeImageUI.color = lifeDamageColor;

        while (elapsed < lifeAnimationDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsed / lifeAnimationDuration
            );

            // 처음에는 커지고, 마지막에는 원래 크기로 돌아옵니다.
            float punch = Mathf.Sin(progress * Mathf.PI);

            float scale = Mathf.Lerp(
                1f,
                lifePunchScale,
                punch
            );

            // 좌우로 빠르게 흔듭니다.
            float angle =
                Mathf.Sin(progress * Mathf.PI * 6f) *
                lifeShakeAngle *
                (1f - progress);

            heartTransform.localScale =
                originalScale * scale;

            heartTransform.localRotation =
                Quaternion.Euler(0f, 0f, angle);

            // 빨간색에서 원래 색으로 돌아옵니다.
            lifeImageUI.color = Color.Lerp(
                lifeDamageColor,
                originalColor,
                progress
            );

            yield return null;
        }

        heartTransform.localScale = originalScale;
        heartTransform.localRotation = originalRotation;
        lifeImageUI.color = originalColor;
    }


    public void PlayPlusCatchSound()
    {
        if (audioSource != null && plusCatchSound != null)
        {
            audioSource.PlayOneShot(plusCatchSound);
        }
    }

    public void PlayMinusCatchSound()
    {
        if (audioSource != null && minusCatchSound != null)
        {
            audioSource.PlayOneShot(minusCatchSound);
        }
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
            ScoreManagerScript.Instance.planScore = score;
            ScoreManagerScript.Instance.catchGameClear = true;
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

        for (int i = 0; i < lifeImages.Length; i++)
        {
            if (lifeImages[i] == null)
            {
                continue;
            }

            // 남은 목숨은 채워진 하트,
            // 소진된 목숨은 빈 하트로 표시합니다.
            if (i < currentLives)
            {
                lifeImages[i].sprite = lifeImage;
            }
            else
            {
                lifeImages[i].sprite = emptyLifeImage;
            }

            lifeImages[i].enabled = true;
            lifeImages[i].preserveAspect = true;
        }
    }
}