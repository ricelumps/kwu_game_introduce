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

    [SerializeField] private ObstacleSpawner obstacleSpawner;

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

        Time.timeScale = 1.0f;

        ScoreManager.Instance.ResetScore();
        SpeedManager.Instance.ResetSpeed();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideGameOver();
        }

        obstacleSpawner.StartSpawning();
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
        SceneManager.LoadScene("RunningGameScene");
    }


    public bool IsPlaying()
    {
        return currentState == GameState.Playing;
    }
}