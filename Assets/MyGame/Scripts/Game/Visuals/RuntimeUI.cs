using TMPro;
using Unity.Netcode;
using UnityEngine;

public class RuntimeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private void Update()
    {
        if (GameManager.Instance == null || !NetworkManager.Singleton) { return; }
        if (!GameManager.Instance.GetIsPlaying() || !NetworkManager.Singleton.IsClient) { return; }
        double timeLeft = GameManager.Instance.GetEndtime() - NetworkManager.Singleton.ServerTime.Time;

        timeLeft = Mathf.Max(0, (float)timeLeft);

        int minutes = Mathf.FloorToInt((float)timeLeft / 60);
        int seconds = Mathf.FloorToInt((float)timeLeft % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
