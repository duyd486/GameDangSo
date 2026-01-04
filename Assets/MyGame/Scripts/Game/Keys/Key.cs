using Unity.Netcode;

public class Key : NetworkBehaviour, IInteractable
{
    public void Interact(ulong clientId)
    {
        TryPickedUpServerRpc(clientId);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void TryPickedUpServerRpc(ulong clientId)
    {
        if (!IsServer) return;

        if (!NetworkManager.Singleton.ConnectedClients
            .TryGetValue(clientId, out var client)) return;

        var player = client.PlayerObject.GetComponent<PlayerInteract>();
        if (player == null || player.GetIsCarrying()) return;

        player.CarryKeyServer();
        DestroyKeyRpc();
        GameManager.Instance.OnKeyPicked();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void DestroyKeyRpc()
    {
        if (IsServer)
        {
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }
}
