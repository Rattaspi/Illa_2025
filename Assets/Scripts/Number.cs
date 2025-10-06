using DG.Tweening;
using Sortify;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Number : MonoBehaviour {
    [SerializeField] TextMeshProUGUI numberText;
    [SerializeField, ReadOnly] int number = -1;
    bool selected = false;

    [Space(), BetterHeader("Selection")]
    [SerializeField] Image image;
    [SerializeField] Color selectedColor = Color.orange;

    public void SetNumber(int number) {
        this.number = number;
        numberText.text = number.ToString("000");
    }

    public void NofitySelectedNumber() {
        if (!selected) {
            //AppManager.instance.NofitySelectedNumber(this);
            selected = true;
            TweenColor(selectedColor, 0.25f);
        }
    }

    public Tween TweenColor(Color color, float time) {
        return DOTween.Sequence()
            .Append(image.DOColor(color, time))
            .Join(numberText.DOColor(color, time));
    }

    public int GetNumberValue() { return number; }
}
