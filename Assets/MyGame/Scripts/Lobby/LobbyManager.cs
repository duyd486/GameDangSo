using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public event EventHandler OnLobbyCreated;
    public event EventHandler<OnLobbyDataChangedEventArgs> OnLobbyDataChanged;
    public class OnLobbyDataChangedEventArgs : EventArgs
    {
        public Lobby lobby;
    }

    private Lobby hostLobby;

    private Lobby joinedLobby;

    private List<Lobby> currentLobbies;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    private string playerName;

    private void Awake()
    {
        Instance = this;
        playerName = "duy" + UnityEngine.Random.Range(10, 99);
        Debug.Log("Player Name: " + playerName);
    }

    private async void Start()
    {

        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Sign In " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();


    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyUpdate();
    }


    public async void CreateLobby(string lobbyName, int maxPlayer, Action<Lobby> HostLobby)
    {
        try
        {
            Debug.Log("Start Create Lobby");

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer()
            };


            hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, createLobbyOptions);
            joinedLobby = hostLobby;

            OnLobbyCreated?.Invoke(this, EventArgs.Empty);
            HostLobby(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    public async void ListLobbies(Action ShowLobbies)
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            currentLobbies = queryResponse.Results;

            ShowLobbies();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    public async void JoinLobby(string id)
    {
        try
        {
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(id, joinLobbyByIdOptions);
            OnLobbyDataChanged?.Invoke(this, new OnLobbyDataChangedEventArgs
            {
                lobby = joinedLobby
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
            OnLobbyDataChanged?.Invoke(this, new OnLobbyDataChangedEventArgs
            {
                lobby = joinedLobby
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0)
            {
                heartbeatTimer = 15;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    private async void HandleLobbyUpdate()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0)
            {
                lobbyUpdateTimer = 1.5f;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                OnLobbyDataChanged?.Invoke(this, new OnLobbyDataChangedEventArgs
                {
                    lobby = joinedLobby
                });
            }
        }
    }

    public List<Lobby> GetCurrentLobbies()
    {
        return currentLobbies;
    }

    public Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
    }
}
