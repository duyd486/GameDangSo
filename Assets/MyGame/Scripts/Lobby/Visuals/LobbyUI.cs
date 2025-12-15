using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private Button leaveLobbyBtn;
    [SerializeField] private Button startGameBtn;

    [SerializeField] private PlayerInfoSingleUI[] playerInfos;

    private Lobby lobby;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Hide();

        leaveLobbyBtn.onClick.AddListener(() =>
        {
            LobbyManager.Instance.LeaveLobby();
        });

        startGameBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
            NetworkManager.Singleton.StartHost();
        });

        LobbyManager.Instance.OnLobbyDataChanged += LobbyManager_OnLobbyDataChanged;
    }

    private void LobbyManager_OnLobbyDataChanged(object sender, LobbyManager.OnLobbyDataChangedEventArgs e)
    {
        UpdateLobby(e.lobby);
    }

    public void UpdateLobby(Lobby lobby)
    {
        if (lobby == null)
        {
            Hide();
            return;
        }

        this.lobby = lobby;
        lobbyName.text = lobby.Name;

        int playerIndex = 0;
        List<Player> players = lobby.Players;

        foreach (PlayerInfoSingleUI playerInfo in playerInfos)
        {
            if (playerIndex < players.Count)
            {
                playerInfo.UpdatePlayerInfo(players[playerIndex].Data["PlayerName"].Value);
                playerIndex++;
            }
            else
            {
                playerInfo.UpdatePlayerInfo();
            }
        }

        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if(LobbyManager.Instance.IsHost())
        {
            startGameBtn.gameObject.SetActive(true);
        }
        else
        {
            startGameBtn.gameObject.SetActive(false);
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
