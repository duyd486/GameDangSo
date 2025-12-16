using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private Transform cameraContainer;


    private CinemachineCamera cinemachineCamera;



    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;

        Camera.main.transform.SetParent(cameraContainer);
        Camera.main.transform.position = new Vector3(0,1,0);
        cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
        cinemachineCamera.Follow = transform;
        cinemachineCamera.LookAt = transform;
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) return;
        Camera.main.transform.SetParent(null);
    }
}
