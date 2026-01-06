using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerLocomotion : NetworkBehaviour
{
    [SerializeField] private float walkSpeed = 5f;

    private float currentSpeedMultiplier = 1f;
    private Coroutine slowCoroutine;
    private float remainingSlowTime = 0f;

    private void Update()
    {
        if (!IsOwner) return;
        if (!GameManager.Instance.GetIsPlaying()) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 inputVector = new Vector2(h, v);
        Vector3 inputDir = new Vector3(inputVector.x, 0f, inputVector.y);

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        Vector3 moveDir = (cameraForward * inputDir.z + cameraRight * inputDir.x).normalized;

        transform.position += moveDir * walkSpeed * currentSpeedMultiplier * Time.deltaTime;

        Quaternion targetRot = Quaternion.LookRotation(cameraForward);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            10f * Time.deltaTime
        );
    }

    public void ApplySlow(float duration, float speedReduction)
    {
        // Clamp speedReduction trong khoảng 0-1
        speedReduction = Mathf.Clamp01(speedReduction);

        remainingSlowTime += duration;

        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }

        slowCoroutine = StartCoroutine(SlowCoroutine(speedReduction));
    }

    private IEnumerator SlowCoroutine(float speedReduction)
    {
        float targetMultiplier = 1f - speedReduction;
        currentSpeedMultiplier = targetMultiplier;

        while (remainingSlowTime > 0f)
        {
            remainingSlowTime -= Time.deltaTime;
            yield return null;
        }

        remainingSlowTime = 0f;
        currentSpeedMultiplier = 1f;
        slowCoroutine = null;
    }

    public void ClearSlow()
    {
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
            slowCoroutine = null;
        }

        remainingSlowTime = 0f;
        currentSpeedMultiplier = 1f;
    }

    public float GetRemainingSlowTime()
    {
        return remainingSlowTime;
    }

    public bool IsSlowed()
    {
        return currentSpeedMultiplier < 1f;
    }
}