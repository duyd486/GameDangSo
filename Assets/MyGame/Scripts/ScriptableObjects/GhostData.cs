using UnityEngine;

[CreateAssetMenu(fileName = "GhostData", menuName = "Horror/GhostData")]
public class GhostData : ScriptableObject
{
    public string ghostName;

    [Header("Visual")]
    public GameObject ghostPrefab;
    public Sprite jumpscareSprite;

    [Header("AI")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    public float detectRadius = 8f;
    public float jumpScareRadius = 1.5f;
    public float loseRadius = 12f;
    public float reachDistance = 0.5f;

    [Header("Jumpscare")]
    public float jumpscareDistance = 1.2f;
    public float jumpscareDuration = 2f;
    public float jumpscareCooldown = 3f;
    public float jumpscareEffect = 0.5f;
    public float timerReduce = 10f;
}
