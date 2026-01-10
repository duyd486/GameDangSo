using TMPro;
using Unity.Netcode;
using UnityEngine;

public class RuntimeUI : MonoBehaviour
{
    public static RuntimeUI Instance;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI keyLeftText;

    [SerializeField] private GameObject messageContainer;
    [SerializeField] private MessageSingleUI messageSingleUI;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.collectedKeys.OnValueChanged += UpdateKeyLeft;
        keyLeftText.text = "Keys: " + "0/" + GameManager.Instance.GetTotalKeys();
    }

    private void UpdateKeyLeft(int previousValue, int newValue)
    {
        keyLeftText.text = "Keys: " + newValue.ToString() + "/" + GameManager.Instance.GetTotalKeys();
    }

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

    public void PushMessage(string message, bool isError)
    {
        MessageSingleUI messageSingleUITmp = Instantiate(messageSingleUI, messageContainer.transform);
        messageSingleUITmp.SetMessage(message, isError);
    }
}
