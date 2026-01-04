using Unity.Netcode;

public class Altar : NetworkBehaviour, IInteractable
{
    public void Interact(ulong clientId)
    {
        //if (!player.GetIsCarrying()) return;
        TryAddKeyServerRpc(clientId);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void TryAddKeyServerRpc(ulong clientId)
    {
        if (!IsServer) return;

        if (!NetworkManager.Singleton.ConnectedClients
            .TryGetValue(clientId, out var client)) return;

        var player = client.PlayerObject.GetComponent<PlayerInteract>();

        if (player == null) return;

        player.DropKey();
        GameManager.Instance.OnKeyAdded();
    }
}
