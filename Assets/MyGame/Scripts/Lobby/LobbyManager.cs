using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public event EventHandler<OnLobbyCreatedEventArgs> OnLobbyCreated;
    public class OnLobbyCreatedEventArgs : EventArgs
    {
        public Lobby hostLobby;
    }
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
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        if (AuthenticationService.Instance.IsSignedIn)
        {
            playerName = SceneLoader.playerName;
            return;
        }
        playerName = "duy" + UnityEngine.Random.Range(10, 99);
        Debug.Log("Player Name: " + playerName);

        SceneLoader.playerName = playerName;

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


    public async void CreateLobby(string lobbyName, int maxPlayer)
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

            NetworkManager.Singleton.StartHost();

            OnLobbyCreated?.Invoke(this, new OnLobbyCreatedEventArgs
            {
                hostLobby = hostLobby
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    public async Task<Lobby> CreateLobbyWithRelay(string lobbyName, int maxPlayers)
    {
        try
        {
            string region = await GetNearestRegion();
            Debug.Log($"Sử dụng region: {region}");

            // Tạo Relay allocation cho host
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers, region);
            Debug.Log(allocation.GetType());

            // Lấy join code để người chơi khác có thể join
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // Tạo lobby với relay join code
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
            {
                { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
            }
            };

            hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            joinedLobby = hostLobby;

            // Cấu hình Unity Transport với Relay
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            // Start host
            NetworkManager.Singleton.StartHost();

            OnLobbyCreated?.Invoke(this, new OnLobbyCreatedEventArgs
            {
                hostLobby = hostLobby
            });

            return hostLobby;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error creating lobby with relay: {e}");
            return null;
        }
    }

    public async void ListLobbies(Action ShowLobbies)
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
            {
                new QueryFilter(
                    QueryFilter.FieldOptions.IsLocked,
                    "0",
                    QueryFilter.OpOptions.EQ
                )
            }
            };

            QueryResponse queryResponse =
                await LobbyService.Instance.QueryLobbiesAsync(options);

            currentLobbies = queryResponse.Results;
            ShowLobbies();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    public async void JoinLobbyById(string id)
    {
        try
        {
            //QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            //JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            //{
            //    Player = GetPlayer()
            //};

            joinedLobby = await JoinLobbyByIdRelay(id);
            //joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(id, joinLobbyByIdOptions);
            //OnLobbyDataChanged?.Invoke(this, new OnLobbyDataChangedEventArgs
            //{
            //    lobby = joinedLobby
            //});
            //NetworkManager.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }


    public async Task<Lobby> JoinLobbyByIdRelay(string id)
    {
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(id, joinLobbyByIdOptions);
            await JoinRelayFromLobby();
            OnLobbyDataChanged?.Invoke(this, new OnLobbyDataChangedEventArgs
            {
                lobby = joinedLobby
            });
            return joinedLobby;
        }
        catch (Exception e)
        {
            Debug.LogError($"Lỗi join lobby: {e}");
            return null;
        }
    }


    private async Task JoinRelayFromLobby()
    {
        try
        {
            // Lấy relay join code từ lobby
            string joinCode = joinedLobby.Data["RelayJoinCode"].Value;
            Debug.Log($"Đang join Relay với code: {joinCode}");

            // Join relay allocation với WebSocket
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            // Cấu hình Unity Transport
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetClientRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );

            // Start client
            NetworkManager.Singleton.StartClient();
            Debug.Log("Đã kết nối client qua Relay");
        }
        catch (Exception e)
        {
            Debug.LogError($"Lỗi join Relay: {e}");
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
            hostLobby = null;
            OnLobbyDataChanged?.Invoke(this, new OnLobbyDataChangedEventArgs
            {
                lobby = joinedLobby
            });

            NetworkManager.Singleton.Shutdown();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    public async void LockLobby()
    {
        if (hostLobby != null)
        {
            await LobbyService.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                IsLocked = true
            });
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

    private async Task<string> GetNearestRegion()
    {
        try
        {
            var regions = await RelayService.Instance.ListRegionsAsync();

            if (regions == null || regions.Count == 0)
            {
                Debug.LogWarning("Không tìm thấy regions, sử dụng region mặc định");
                return null; // Relay sẽ tự chọn region
            }

            // Lấy region đầu tiên (thường là gần nhất)
            string selectedRegion = regions[0].Id;
            Debug.Log($"Regions có sẵn: {string.Join(", ", regions.Select(r => r.Id))}");
            return selectedRegion;
        }
        catch (Exception e)
        {
            Debug.LogError($"Lỗi lấy regions: {e}");
            return null;
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

    public bool IsHost()
    {
        return hostLobby != null;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
    }
}
