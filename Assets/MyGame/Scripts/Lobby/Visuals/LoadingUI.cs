using DG.Tweening;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    public static LoadingUI Instance;



    [SerializeField] RectTransform ghostRunning;

    public float floatDistance = 20f;
    public float floatDuration = 1f;

    private Vector2 startPos;

    private void Awake()
    {
        Instance = this;
        startPos = ghostRunning.anchoredPosition;
        Hide();
    }

    private void OnEnable()
    {
        ghostRunning.anchoredPosition = startPos;

        ghostRunning.DOAnchorPosY(startPos.y + floatDistance, floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        ghostRunning.DOKill();
        ghostRunning.anchoredPosition = startPos;
    }


    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
