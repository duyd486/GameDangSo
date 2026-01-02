using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject keyPrefab;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] float gameDuration = 300f; // 5 minutes
    [SerializeField] int totalKeys = 10;
    [SerializeField] bool isPlaying = false;


    public NetworkVariable<double> endTime = new NetworkVariable<double>();
    public NetworkVariable<int> collectedKeys = new NetworkVariable<int>(0);





    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Instance = this;



        if (!IsServer) return;

        // Server làm việc ở đây



        //SpawnKeys();
    }


    private void Update()
    {
        Debug.Log("Current Server Time: " + NetworkManager.ServerTime.Time);







        if (!IsServer) return;

        if (NetworkManager.ServerTime.Time >= endTime.Value && isPlaying)
        {
            LoseGame();
        }
    }


    public void StartGame()
    {






        if (!IsServer) return;

        Debug.Log("StartGame called on SERVER");

        endTime.Value = NetworkManager.ServerTime.Time + gameDuration;


        isPlaying = true;
    }

    private void LoseGame()
    {
        Debug.Log("Game Over! You lost.");
        isPlaying = false;
    }
}
