using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float maxDistance = 10f;
    private bool canInteract = false;

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
                hit.transform.GetComponentInParent<IInteractable>().Interact();
            }
        }
        else
        {
            canInteract = false;
        }
    }

    public bool GetCanInteract()
    {
        return canInteract;
    }
}