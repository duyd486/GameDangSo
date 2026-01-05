using Unity.Netcode;
using UnityEngine;

public class PlayerJumpscare : NetworkBehaviour
{
    [SerializeField] private GhostList ghostList;

    public void TriggerJumpscare(string ghostName)
    {
        GhostData ghost = ghostList.GetGhostByName(ghostName);

        if (ghost == null)
        {
            Debug.Log("Ghost not on list");
        }

        GetComponent<PlayerLocomotion>().ApplySlow(ghost.jumpscareCooldown, ghost.jumpscareEffect);
        JumpscareUI.Instance.ShowJumpscare(ghost.jumpscareSprite, ghost.jumpscareDuration);
    }
}
