using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject keyPrefab;

    [SerializeField] float gameDuration = 60f;
    [SerializeField] int totalKeys = 10;
    [SerializeField] int totalGhosts = 3;

    public NetworkVariable<bool> isPlaying = new NetworkVariable<bool>(false);
    public NetworkVariable<double> endTime = new NetworkVariable<double>();
    public NetworkVariable<int> collectedKeys = new NetworkVariable<int>(0);

    public List<Key> keys = new List<Key>();

    public static event EventHandler<OnGameOverEventArgs> OnGameOver;
    public class OnGameOverEventArgs : EventArgs
    {
        public bool isLose;
    }

    public static event EventHandler<OnGameStartEventArgs> OnGameStart;
    public class OnGameStartEventArgs : EventArgs
    {
        public int totalKeys;
        public int totalGhosts;
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

        RestartGame();

        RuntimeUI.Instance.PushMessage("The game has started!", false);

        endTime.Value = NetworkManager.ServerTime.Time + gameDuration;

        OnGameStart?.Invoke(this, new OnGameStartEventArgs
        {
            totalKeys = totalKeys,
            totalGhosts = totalGhosts,
        });
        Cursor.lockState = CursorLockMode.Locked;
        isPlaying.Value = true;
    }

    public void RestartGame()
    {
        if (!IsServer) return;
        collectedKeys.Value = 0;
        isPlaying.Value = false;
        foreach (var key in keys)
        {
            Destroy(key.gameObject);
        }
        keys.Clear();
    }


    public void OnKeyPicked()
    {
        if (!IsServer) return;
        NotifyKeyPickedClientRpc();
    }

    public void OnKeyAdded()
    {
        if (!IsServer) return;
        collectedKeys.Value++;
        if (collectedKeys.Value >= totalKeys)
        {
            WinGameClientRpc();
            isPlaying.Value = false;
        }
        else
        {
            NotifyKeyAddedClientRpc(totalKeys - collectedKeys.Value);
        }
    }

    [ClientRpc]
    void NotifyKeyPickedClientRpc()
    {
        RuntimeUI.Instance.PushMessage("Someone picked up a key!", false);
    }

    [ClientRpc]
    void NotifyKeyAddedClientRpc(int keyLeft)
    {
        RuntimeUI.Instance.PushMessage("A key has been added to the altar! " + keyLeft.ToString() + " keys left!", false);
    }


    [ClientRpc]
    private void WinGameClientRpc()
    {
        Cursor.lockState = CursorLockMode.None;
        OnGameOver?.Invoke(this, new OnGameOverEventArgs
        {
            isLose = false,
        });
    }

    [ClientRpc]
    private void LoseGameClientRpc()
    {
        Cursor.lockState = CursorLockMode.None;
        OnGameOver?.Invoke(this, new OnGameOverEventArgs
        {
            isLose = true,
        });
    }

    public void ReduceTime(float time)
    {
        if (!IsServer) return;
        endTime.Value -= time;
    }

    public bool GetIsPlaying()
    {
        return isPlaying.Value;
    }
    public double GetEndtime()
    {
        return endTime.Value;
    }
    public List<Key> GetKeys()
    {
        return keys;
    }
    public int GetTotalKeys()
    {
        return totalKeys;
    }
    public void RegisterKey(Key key)
    {
        if (!keys.Contains(key))
            keys.Add(key);
    }
    public void UnregisterKey(Key key)
    {
        keys.Remove(key);
    }
}
