using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] Button mainMenuBtn;


    private void Start()
    {
        Hide();
        GameManager.OnGameOver += GameManager_OnGameOver;
        mainMenuBtn.onClick.AddListener(() =>
        {
            SceneLoader.LoadScene(SceneLoader.Scene.Lobby);
        });
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
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
