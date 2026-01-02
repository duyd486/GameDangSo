using Unity.Netcode;

public class Key : NetworkBehaviour, IInteractable
{
    public void Interact()
    {
        TryPickedUpServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void TryPickedUpServerRpc()
    {
        if (!IsServer) return;
        GameManager.Instance.OnKeyPicked();
        NetworkObject.Despawn();
    }
}
