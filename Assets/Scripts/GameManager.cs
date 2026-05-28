using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] private BonusItemSpawner bonusItemSpawner;

    private GameState currentState = GameState.Ready;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        currentState = GameState.Playing;

        ScoreManager.Instance.ResetScore();
        SpeedManager.Instance.ResetSpeed();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideGameOver();
        }

        obstacleSpawner.StartSpawning();
        bonusItemSpawner.StartSpawning();
    }

    public void GameOver()
    {
        Debug.Log("GameManager.GameOver 호출됨");

        if (currentState == GameState.GameOver)
        {
            return;
        }

        currentState = GameState.GameOver;

        if (obstacleSpawner != null)
        {
            obstacleSpawner.StopSpawning();
        }

        if (bonusItemSpawner != null)
        {
            bonusItemSpawner.StopSpawning();
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SaveHighScore();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver();
        }
        else
        {
            Debug.LogWarning("UIManager.Instance가 없습니다.");
        }

        Debug.Log("게임 오버");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("RunningGameScene");
    }


    public bool IsPlaying()
    {
        return currentState == GameState.Playing;
    }
}