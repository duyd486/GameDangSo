using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private Transform cameraContainer;

    private CinemachineCamera cinemachineCamera;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        Cursor.lockState = CursorLockMode.Locked;
        cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();

        if (cinemachineCamera != null)
        {
            //cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
            cinemachineCamera.Follow = transform;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (!GameManager.Instance.GetIsPlaying())
        {
            cinemachineCamera.gameObject.SetActive(false);
        }
        else
        {
            cinemachineCamera.gameObject.SetActive(true);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        Camera.main.transform.SetParent(null);
    }
}
