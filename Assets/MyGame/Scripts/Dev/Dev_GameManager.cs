using Unity.Netcode;
using UnityEngine;

public class Dev_GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.StartHost();
                RuntimeUI.Instance.PushMessage("Host started", false);
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.StartClient();
                RuntimeUI.Instance.PushMessage("Client started", false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.StartGame();
        }
    }
}
