using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        StartCoroutine(StartSpawn());

        IEnumerator StartSpawn()
        {
            yield return new WaitForSeconds(1f);
            if (IsHost)
            {
                SpawnPlayers();
            }
        }

    }

    private void SpawnPlayers()
    {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log("Spawning player for clientId: " + clientId);
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
        float z = Random.Range(-10f, 10f);
        return new Vector3(x, 3f, z);
    }
}
