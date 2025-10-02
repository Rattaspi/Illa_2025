using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class KeypadNumberAnimation : MonoBehaviour {
    [SerializeField] Button button;
    [SerializeField] CanvasGroup glowCanvasGroup;

    Tween pressedAnimationTween;

    void Start() {
        button.onClick.AddListener(PlayPressedAnimation);
    }

    public void PlayPressedAnimation() {
        if (pressedAnimationTween != null) {
            pressedAnimationTween.Kill();
        }
        glowCanvasGroup.alpha = 1;
        pressedAnimationTween = glowCanvasGroup.DOFade(0f, 1f);
    }
}
