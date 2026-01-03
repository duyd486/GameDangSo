using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;



    private void Start()
    {
        Hide();
        GameManager.OnGameOver += GameManager_OnGameOver;
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
