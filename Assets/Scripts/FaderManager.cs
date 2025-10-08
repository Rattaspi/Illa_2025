using DG.Tweening;
using UnityEngine;

public class FaderManager : MonoBehaviour {
    public static FaderManager instance;

    [SerializeField] CanvasGroup[] faders;

    void Awake() {
        instance = this;
    }

    void Start() {
        Sequence fadeInSequence = DOTween.Sequence();
        for (int i = 0; i < faders.Length; i++) {
            fadeInSequence.Join(faders[i].DOFade(1, 0.5f).From());
        }
    }

    Tween FadeTo(int fadeTo) {
        Sequence fadeInSequence = DOTween.Sequence();
        for (int i = 0; i < faders.Length; i++) {
            fadeInSequence.Join(faders[i].DOFade(fadeTo, 0.5f));
        }

        return fadeInSequence;
    }

    public Tween FadeIn() {
        return FadeTo(0);
    }
    public Tween FadeOut() {
        return FadeTo(1);
    }
}
