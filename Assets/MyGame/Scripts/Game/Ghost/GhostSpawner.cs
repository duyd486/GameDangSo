using Unity.Netcode;
using UnityEngine;

public class GhostSpawner : NetworkBehaviour
{
    public static GhostSpawner Instance;

    [SerializeField] private GhostList ghostList;

    [SerializeField] private Vector3 spawnAreaMin = new Vector3(-100f, 0f, -100f);
    [SerializeField] private Vector3 spawnAreaMax = new Vector3(100f, 0f, 100f);

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
        SpawnGhosts(e.totalGhosts);
    }

    void SpawnGhosts(int totalGhosts)
    {
        if (!IsServer) return;
        // Implement ghost spawning logic here
        GhostData[] ghostDatas = ghostList.ghosts;
        for (int i = 0; i < totalGhosts; i++)
        {
            var ghostData = ghostDatas[Random.Range(0, ghostDatas.Length)];
            var ghost = Instantiate(ghostData.ghostPrefab, GetRandomSpawnPosition(), Quaternion.identity);
            ghost.GetComponent<GhostAI>().SetData(ghostData);
            ghost.GetComponent<NetworkObject>().Spawn();
        }
    }
    public Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        float z = Random.Range(spawnAreaMin.z, spawnAreaMax.z);
        return new Vector3(x, y, z);
    }
}
