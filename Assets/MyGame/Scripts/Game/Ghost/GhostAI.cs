using Unity.Netcode;
using UnityEngine;

public class GhostAI : NetworkBehaviour
{
    private GhostData data;

    private Transform[] keyPoints;
    private int currentKeyIndex;

    private Transform targetPlayer;
    private Transform targetKey;

    private enum State { Patrol, Chase }
    private State state;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        FindKeys();
        state = State.Patrol;
    }

    void Update()
    {
        if (!IsServer || !GameManager.Instance.GetIsPlaying()) return;
        DetectPlayer();

        if (state == State.Patrol)
            Patrol();
        else
            Chase();
    }

    void Patrol()
    {
        if (keyPoints.Length == 0)
        {
            FindKeys();
            return;
        }

        if (targetKey == null)
            MoveToNextKey();

        MoveTowards(targetKey.position, data.patrolSpeed);

        if (Vector3.Distance(transform.position, targetKey.position) < data.reachDistance)
        {
            //Debug.Log(Vector3.Distance(transform.position, target.position));
            Debug.Log("Ghost reached key point " + currentKeyIndex);
            MoveToNextKey();
        }
    }

    void Chase()
    {
        if (targetPlayer == null)
        {
            ReturnToPatrol();
            return;
        }

        float dist = Vector3.Distance(transform.position, targetPlayer.position);

        if (dist > data.loseRadius)
        {
            ReturnToPatrol();
            return;
        }

        if (dist < data.jumpScareRadius)
        {
            //jumpscare
            TriggerJumpscare(targetPlayer.GetComponent<NetworkObject>().OwnerClientId);
            //go to somewhere else
            transform.position = GhostSpawner.Instance.GetRandomSpawnPosition();
        }

        MoveTowards(targetPlayer.position, data.chaseSpeed);
    }

    void TriggerJumpscare(ulong clientId)
    {
        ShowJumpscareClientRpc(clientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void ShowJumpscareClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId)
            return;

        if (JumpscareUI.Instance == null)
        {
            Debug.LogError("Client chưa có JumpscareUI");
            return;
        }

        Debug.Log("Hiện Jumpscare cho client " + clientId);

        JumpscareUI.Instance.ShowJumpscare(data.jumpscareSprite, data.jumpscareTime);
    }

    void DetectPlayer()
    {
        if (state == State.Chase) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var player = client.PlayerObject;
            if (player == null) continue;

            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist <= data.detectRadius)
            {
                targetPlayer = player.transform;
                state = State.Chase;
                return;
            }
        }
    }

    void ReturnToPatrol()
    {
        targetPlayer = null;
        state = State.Patrol;
        MoveToNextKey();
    }

    void MoveToNextKey()
    {
        FindKeys();
        currentKeyIndex = Random.Range(0, keyPoints.Length);
        targetKey = keyPoints[currentKeyIndex];
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 dir = (target - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (dir != Vector3.zero)
            transform.forward = dir;
    }

    void FindKeys()
    {
        var keys = GameManager.Instance.GetKeys();
        keyPoints = new Transform[keys.Count];
        for (int i = 0; i < keys.Count; i++)
            keyPoints[i] = keys[i].transform;

        currentKeyIndex = 0;
    }

    public void SetData(GhostData ghostData)
    {
        data = ghostData;
    }
}
