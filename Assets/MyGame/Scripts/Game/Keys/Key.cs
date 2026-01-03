using Unity.Netcode;

public class Key : NetworkBehaviour, IInteractable
{
    public void Interact()
    {
        TryPickedUpServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void TryPickedUpServerRpc()
    {
        if (!IsServer) return;
        GameManager.Instance.OnKeyPicked();
        NetworkObject.Despawn();
    }
}
