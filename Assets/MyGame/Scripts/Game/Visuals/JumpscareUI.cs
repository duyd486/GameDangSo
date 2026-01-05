using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JumpscareUI : MonoBehaviour
{
    public static JumpscareUI Instance;

    [SerializeField] private Image jumpscareImage;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        jumpscareImage.enabled = false;
    }

    public void ShowJumpscare(Sprite sprite, float duration)
    {
        if (sprite == null)
        {
            Debug.LogError("JumpscareUI: sprite null");
            return;
        }

        if (jumpscareImage == null)
        {
            Debug.LogError("JumpscareUI: image null");
            return;
        }

        jumpscareImage.sprite = sprite;
        jumpscareImage.enabled = true;

        StartCoroutine(HideAfterDelay(duration));
    }

    private IEnumerator HideAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        jumpscareImage.enabled = false;
    }
}
