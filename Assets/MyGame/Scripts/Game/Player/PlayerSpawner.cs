using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private List<GameObject> activePlayers;
    //public override void OnNetworkSpawn()
    //{
    //    base.OnNetworkSpawn();

    //    StartCoroutine(StartSpawn());

    //    IEnumerator StartSpawn()
    //    {
    //        yield return new WaitForSeconds(1f);
    //        if (IsHost)
    //        {
    //            SpawnPlayers();
    //        }
    //    }

    //}

    private void Start()
    {
        GameManager.OnGameStart += GameManager_OnGameStart;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStart -= GameManager_OnGameStart;
    }

    private void GameManager_OnGameStart(object sender, GameManager.OnGameStartEventArgs e)
    {
        if (activePlayers.Count == 0)
        {
            SpawnPlayers();
        }
        else
        {
            ResetPlayerPos();
        }
    }

    private void SpawnPlayers()
    {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            RuntimeUI.Instance.PushMessage("Spawning player for clientId: " + clientId, false);
            SpawnPlayerForClient(clientId);
        }
    }

    private void SpawnPlayerForClient(ulong clientId)
    {
        GameObject playerInstance = Instantiate(playerPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        activePlayers.Add(playerInstance);
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
        networkObject.SpawnAsPlayerObject(clientId);
    }

    void ResetPlayerPos()
    {
        foreach (GameObject player in activePlayers)
        {
            player.transform.position = GetRandomSpawnPosition();
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-10f, 10f);
        float z = Random.Range(1f, 6f);
        return new Vector3(x, 3f, z);
    }
}
