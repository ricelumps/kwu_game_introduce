using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour
{
    [SerializeField] private GameObject[] gamePanels;

    void Start() {
        foreach (var gamePanel in gamePanels)
        {
            gamePanel.SetActive(false);
        }
    }

    // MainScene 이동 버튼
    public void OnMainSceneBtn()
    {
        SceneManager.LoadScene("MainScene");
    }

    // GameScene 이동 버튼
    public void OnGameSceneBtn()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnCloseBtn()
    {
        foreach (var gamePanel in gamePanels)
        {
            gamePanel.SetActive(false);
        }
    }

    public void OnGamePanel1Btn()
    {
        foreach (var gamePanel in gamePanels)
        {
            gamePanel.SetActive(false);
        }
        gamePanels[0].SetActive(true);
    }

    public void OnGamePanel2Btn()
    {
        foreach (var gamePanel in gamePanels)
        {
            gamePanel.SetActive(false);
        }
        gamePanels[1].SetActive(true);
    }

    public void OnGamePanel3Btn()
    {
        foreach (var gamePanel in gamePanels)
        {
            gamePanel.SetActive(false);
        }
        gamePanels[2].SetActive(true);
    }

    public void OnGamePanel4Btn()
    {
        foreach (var gamePanel in gamePanels)
        {
            gamePanel.SetActive(false);
        }
        gamePanels[3].SetActive(true);
    }
}
