using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberForFinal : MonoBehaviour {
    static NumberForFinal numberSelected;

    [SerializeField] Image ballImage;
    [SerializeField] Color selectedBallColor;
    [SerializeField] TextMeshProUGUI numberText;
    [SerializeField] Color selectedTextColor;
    Color textOriginalColor;

    bool isColored = false;

    void Start() {
        textOriginalColor = numberText.color;
    }

    public Tween AnimateOut() {
        return this.GetComponent<RectTransform>().DOMoveY(1200, 0.5f).SetEase(Ease.InBack);
    }

    public Tween Select() {
        if(numberSelected != null) {
            numberSelected.Deselect();
        }

        numberSelected = this;

        return DOTween.Sequence()
            .Append(ballImage.DOColor(selectedBallColor, 0.05f))
            .Join(numberText.DOColor(selectedTextColor, 0.05f));

    }

    public void Deselect() {
        if (isColored) { return; }
        ballImage.DOColor(Color.white, 0.05f);
        numberText.DOColor(textOriginalColor, 0.05f);
    }

    public void SetRed() {
        ballImage.DOColor(Color.red, 0.5f);
        numberText.DOColor(Color.white, 0.5f);
        
        isColored = true;
    }

    public void SetGreen() {
        ballImage.DOColor(Color.green, 0.5f);
        numberText.DOColor(Color.white, 0.5f);
    }
}
