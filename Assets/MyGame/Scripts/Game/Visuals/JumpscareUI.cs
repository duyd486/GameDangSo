using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JumpscareUI : MonoBehaviour
{
    public static JumpscareUI Instance;

    [SerializeField] private Image jumpscareImage;
    [SerializeField] private Image jumpscareViggnete;

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
            RuntimeUI.Instance.PushMessage("JumpscareUI: sprite null", true);
            return;
        }

        if (jumpscareImage == null)
        {
            RuntimeUI.Instance.PushMessage("JumpscareUI: image null", true);
            return;
        }

        jumpscareImage.sprite = sprite;
        jumpscareImage.enabled = true;
        jumpscareViggnete.enabled = true;

        StartCoroutine(HideAfterDelay(duration));
    }

    private IEnumerator HideAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        jumpscareImage.enabled = false;
        jumpscareViggnete.enabled = false;
    }
}
