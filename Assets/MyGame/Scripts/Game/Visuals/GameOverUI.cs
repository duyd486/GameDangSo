using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] Button mainMenuBtn;
    [SerializeField] Button restartBtn;


    private void Start()
    {
        Hide();
        GameManager.OnGameStart += GameManager_OnGameStart;
        GameManager.OnGameOver += GameManager_OnGameOver;
        mainMenuBtn.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.IsHost)
            {
                SceneLoader.LoadSceneByNetwork(SceneLoader.Scene.Lobby);
                NetworkManager.Singleton.Shutdown();
                return;
            }
            SceneLoader.LoadScene(SceneLoader.Scene.Lobby);
            NetworkManager.Singleton.Shutdown();
        });

        if (NetworkManager.Singleton.IsHost)
        {
            restartBtn.gameObject.SetActive(true);
            restartBtn.onClick.AddListener(() =>
            {
                GameManager.Instance.StartGame();
            });
        }
        else
        {
            restartBtn.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStart -= GameManager_OnGameStart;
        GameManager.OnGameOver -= GameManager_OnGameOver;
    }

    private void GameManager_OnGameStart(object sender, GameManager.OnGameStartEventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGameOver(object sender, GameManager.OnGameOverEventArgs e)
    {
        Show(e.isLose);
    }


    public void Show(bool isLose)
    {
        if (isLose)
        {
            resultText.text = "YOU LOSS";
        }
        else
        {
            resultText.text = "YOU WIN";
        }
        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
