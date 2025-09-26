using DG.Tweening;
using TMPro;
using UnityEngine;

public class NumberSphere : MonoBehaviour {
    [SerializeField] RectTransform sphereRectTransform;
    [SerializeField] CanvasGroup sphereCanvasGroup;

    [SerializeField] CanvasGroup upperSphereCanvasGroup;
    [SerializeField] RectTransform upperSphereRectTransform;

    [SerializeField] CanvasGroup lowerSphereCanvasGroup;
    [SerializeField] RectTransform lowerSphereRectTransform;

    [SerializeField] CanvasGroup numberCanvasGroup;
    [SerializeField] TextMeshProUGUI numberText;

    int number;

    public void Init(int number, Vector2 finalPosition) {
        this.number = number;
        numberText.text = number.ToString("000");
        numberCanvasGroup.transform.localScale = Vector3.one * 0.5f;
        PlayAnimation(finalPosition);
    }

    void PlayAnimation(Vector2 finalPosition) {
        DOTween.Sequence()
            .Append(sphereRectTransform.DOMoveY(Screen.height / 2, 1f).SetEase(Ease.OutBack))
            .AppendInterval(0.25f)
            .Append(sphereRectTransform.DOMove(finalPosition, 1f).SetEase(Ease.InOutBack))
            .AppendInterval(1)
            .Append(upperSphereRectTransform.DOMove(new Vector3(50, 75, 0), 1f).SetRelative(true).SetEase(Ease.OutExpo))
            .Join(upperSphereRectTransform.DORotate(new Vector3(0, 0, -30), 1f).SetRelative(true).SetEase(Ease.OutExpo))
            .Join(lowerSphereRectTransform.DOMove(new Vector3(50, -75, 0), 1f).SetRelative(true).SetEase(Ease.OutExpo))
            .Join(lowerSphereRectTransform.DORotate(new Vector3(0, 0, 30), 1f).SetRelative(true).SetEase(Ease.OutExpo))
            .Join(upperSphereCanvasGroup.DOFade(0, 1f).SetEase(Ease.OutExpo))
            .Join(lowerSphereCanvasGroup.DOFade(0, 1f).SetEase(Ease.OutExpo))
            .Join(numberCanvasGroup.DOFade(1, 0.5f))
            .Join(numberCanvasGroup.GetComponent<RectTransform>().DOScale(1.25f, 2f));
    }

    public Tween TweenColor(Color color, float time) {
        return numberText.DOColor(color, 0.5f);
    }

    public int GetNumberValue() { return number; }
}
