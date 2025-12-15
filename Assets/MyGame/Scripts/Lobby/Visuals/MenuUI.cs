using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class MenuUI : MonoBehaviour
{
    public static MenuUI Instance { get; private set; }

    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private Button listLobbyBtn;


    [SerializeField] private GameObject createLobbyModal;
    [SerializeField] private Button cancelCreateBtn;
    [SerializeField] private Button createBtn;
    [SerializeField] private TMP_InputField nameLobbyInput;

    [SerializeField] private TMP_InputField playerNameInput;


    public event EventHandler OnListLobbyClick;

    private void Awake()
    {
        Instance = this;
        createLobbyModal.SetActive(false);
    }

    private void Start()
    {
        playerNameInput.text = LobbyManager.Instance.GetPlayer().Data["PlayerName"].Value;

        playerNameInput.onValueChanged.AddListener((name) =>
        {
            LobbyManager.Instance.SetPlayerName(playerNameInput.text);
        });

        createLobbyBtn.onClick.AddListener(() => { createLobbyModal.SetActive(true); });

        cancelCreateBtn.onClick.AddListener(() => { createLobbyModal.SetActive(false); });

        createBtn.onClick.AddListener(CreateLobby);



        listLobbyBtn.onClick.AddListener(() =>
        {
            LobbyManager.Instance.ListLobbies(() =>
            {
                OnListLobbyClick?.Invoke(this, EventArgs.Empty);
            });
        });
    }

    private void CreateLobby()
    {
        if (nameLobbyInput.text.IsNullOrEmpty()) return;
        LobbyManager.Instance.CreateLobby(nameLobbyInput.text, 4, (lobby) =>
        {
            LobbyUI.Instance.UpdateLobby(lobby);
        });
        createLobbyModal.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
