using Unity.Netcode;
using UnityEngine;

public class PlayerInteract : NetworkBehaviour
{
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private Transform hand;
    [SerializeField] private GameObject keyVisual;
    private bool canInteract = false;
    public NetworkVariable<bool> isCarrying = new NetworkVariable<bool>(false);

    private void Start()
    {
        keyVisual.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        isCarrying.OnValueChanged += OnCarryChanged;
    }

    void OnCarryChanged(bool oldValue, bool newValue)
    {
        keyVisual.SetActive(newValue);
    }

    private void Update()
    {
        if (!IsOwner) return;
        HandleInteract();
    }

    private void HandleInteract()
    {
        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance) && hit.transform.GetComponentInParent<IInteractable>() != null)
        {
            canInteract = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                hit.transform.GetComponentInParent<IInteractable>().Interact(OwnerClientId);
            }
        }
        else
        {
            canInteract = false;
        }
    }

    public void CarryKeyServer()
    {
        if (!IsServer) return;
        isCarrying.Value = true;
    }

    public void DropKey()
    {
        isCarrying.Value = false;
    }

    public bool GetCanInteract()
    {
        return canInteract;
    }
    public bool GetIsCarrying()
    {
        return isCarrying.Value;
    }
}