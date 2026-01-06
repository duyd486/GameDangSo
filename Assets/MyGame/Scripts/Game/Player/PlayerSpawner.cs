using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
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

    private void GameManager_OnGameStart(object sender, GameManager.OnGameStartEventArgs e)
    {
        SpawnPlayers();
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
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
        networkObject.SpawnAsPlayerObject(clientId);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-10f, 10f);
        float z = Random.Range(1f, 6f);
        return new Vector3(x, 3f, z);
    }
}
