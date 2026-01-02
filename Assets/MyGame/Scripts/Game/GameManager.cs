using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject keyPrefab;
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

        SpawnKey();

        isPlaying = true;
    }


    void SpawnKey()
    {
        for (int i = 0; i < totalKeys; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-70f, 70f), 0.3f, Random.Range(-70f, 70f));
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
            WinGame();
        }
        NotifyKeyPickedClientRpc(totalKeys - collectedKeys.Value);
    }

    [ClientRpc]
    void NotifyKeyPickedClientRpc(int keysleft)
    {
        Debug.Log("A key has been picked up!, " + keysleft.ToString() + " key left");
    }



    private void WinGame()
    {
        Debug.Log("Congratulations! You won the game!");
        isPlaying = false;
    }

    private void LoseGame()
    {
        Debug.Log("Game Over! You lost.");
        isPlaying = false;
    }
}
