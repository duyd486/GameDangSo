using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject keyPrefab;

    [SerializeField] float gameDuration = 60f;
    [SerializeField] int totalKeys = 10;

    public NetworkVariable<bool> isPlaying = new NetworkVariable<bool>(false);
    public NetworkVariable<double> endTime = new NetworkVariable<double>();
    public NetworkVariable<int> collectedKeys = new NetworkVariable<int>(0);




    public static event EventHandler<OnGameOverEventArgs> OnGameOver;
    public class OnGameOverEventArgs : EventArgs
    {
        public bool isLose;
    }


    private void Awake()
    {
        Instance = this;
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) return;

        // Server làm việc ở đây

    }


    private void Update()
    {
        if (!IsServer) return;

        if (NetworkManager.ServerTime.Time >= endTime.Value && isPlaying.Value)
        {
            LoseGameClientRpc();
            isPlaying.Value = false;
        }
    }


    public void StartGame()
    {
        if (!IsServer) return;

        Debug.Log("StartGame called on SERVER");

        endTime.Value = NetworkManager.ServerTime.Time + gameDuration;

        SpawnKey();

        isPlaying.Value = true;
    }


    void SpawnKey()
    {
        for (int i = 0; i < totalKeys; i++)
        {
            Vector3 pos = new Vector3(UnityEngine.Random.Range(-70f, 70f), 0.3f, UnityEngine.Random.Range(-70f, 70f));
            GameObject keyObject = Instantiate(keyPrefab, pos, Quaternion.identity);
            keyObject.GetComponent<NetworkObject>().Spawn();
        }
    }


    public void OnKeyPicked()
    {
        if (!IsServer) return;
        collectedKeys.Value += 1;
        if (collectedKeys.Value >= totalKeys)
        {
            WinGameClientRpc();
            isPlaying.Value = false;
        }
        NotifyKeyPickedClientRpc(totalKeys - collectedKeys.Value);
    }

    [ClientRpc]
    void NotifyKeyPickedClientRpc(int keysleft)
    {
        Debug.Log("A key has been picked up!, " + keysleft.ToString() + " key left");
    }


    [ClientRpc]
    private void WinGameClientRpc()
    {
        Debug.Log("Congratulations! You won the game!");
        OnGameOver?.Invoke(this, new OnGameOverEventArgs
        {
            isLose = false,
        });
    }

    [ClientRpc]
    private void LoseGameClientRpc()
    {
        Debug.Log("Game Over! You lost.");
        OnGameOver?.Invoke(this, new OnGameOverEventArgs
        {
            isLose = true,
        });
    }

    public bool GetIsPlaying()
    {
        return isPlaying.Value;
    }
    public double GetEndtime()
    {
        return endTime.Value;
    }
}
