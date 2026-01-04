using Unity.Netcode;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private Transform hand;
    private bool canInteract = false;
    private bool isCarrying = false;
    private Key key;

    private void Update()
    {
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
                hit.transform.GetComponentInParent<IInteractable>().Interact(this);
            }
        }
        else
        {
            canInteract = false;
        }
    }

    public void CarryThisKey(Key key)
    {
        isCarrying = true;
        key.gameObject.transform.SetParent(GetComponent<NetworkObject>().transform, false);
        key.transform.localPosition = new Vector3(-0.9f, 0.2f, 1f);
        this.key = key;
    }

    public void DropKey()
    {
        isCarrying = false;
        key.DestroyKeyRpc();
        this.key = null;
    }

    public bool GetCanInteract()
    {
        return canInteract;
    }
    public bool GetIsCarrying()
    {
        return isCarrying;
    }
}