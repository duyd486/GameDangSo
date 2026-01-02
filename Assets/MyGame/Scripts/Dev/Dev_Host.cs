using Unity.Netcode;
using UnityEngine;

public class Dev_Host : MonoBehaviour
{
    [SerializeField] NetworkManager netManager;

    private void Awake()
    {
        netManager = GetComponent<NetworkManager>();
        netManager.StartHost();
    }
}
