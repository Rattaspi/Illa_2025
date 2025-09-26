using DG.Tweening;
using Sortify;
using TMPro;
using UnityEngine;

public class Gift : MonoBehaviour {
    [BetterHeader("Image references")]
    [SerializeField] CanvasGroup giftCanvasGroup;
    [SerializeField] RectTransform gift;
    [SerializeField] RectTransform head;
    [SerializeField] RectTransform body;

    [Space(), BetterHeader("Number")]
    [SerializeField] CanvasGroup numberCanvasGroup;
    [SerializeField] RectTransform numberRectTransform;
    [SerializeField] TextMeshProUGUI numberText;

    void Start() {
        numberText.text = AppManager.instance.GetWinnerNumberValue().ToString("000");
    }

    public Tween AppearAnimation() {
        return gift.DOMoveY(Screen.height / 2, 0.75f).SetEase(Ease.OutBack);
    }

    [ContextMenu("Open animation")]
    public Tween OpenAnimation() {
        return DOTween.Sequence()
            .Append(gift.DOScale(new Vector3(0, -0.3f, 0), 0.5f).SetRelative(true))
            .Append(gift.DOScale(new Vector3(0, 0.3f, 0), 0.15f).SetRelative(true))
            .Append(head.DOMove(new Vector3(20f, 50f), 0.5f).SetRelative(true))
            .Join(head.DORotate(new Vector3(0, 0, -20f), 0.5f).SetRelative(true))
            .Join(body.DOMoveY(-75f, 0.5f).SetRelative(true))
            .Join(numberCanvasGroup.DOFade(1, 0.25f))
            .Join(numberRectTransform.DOScale(0, 1f).From());
    }
}
