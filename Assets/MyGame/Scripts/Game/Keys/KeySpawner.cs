using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KeySpawner : NetworkBehaviour
{
    public static KeySpawner Instance;

    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private Vector3 spawnAreaMin = new Vector3(-10f, 1f, -10f);
    [SerializeField] private Vector3 spawnAreaMax = new Vector3(10f, 1f, 10f);

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.OnGameStart += GameManager_OnGameStart;
    }

    private void GameManager_OnGameStart(object sender, GameManager.OnGameStartEventArgs e)
    {
        SpawnKeys(e.totalKeys);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        //SpawnKeys();
    }
    public void SpawnKeys(int totalKeys)
    {
        if (!IsServer) return;
        List<Key> keys = new List<Key>();
        for (int i = 0; i < totalKeys; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject keyInstance = Instantiate(keyPrefab, spawnPosition, Quaternion.identity);
            keyInstance.GetComponent<NetworkObject>().Spawn();
        }
    }
    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        float z = Random.Range(spawnAreaMin.z, spawnAreaMax.z);
        return new Vector3(x, y, z);
    }
}
