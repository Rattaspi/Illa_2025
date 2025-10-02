using UnityEngine;
using UnityEngine.UI;

public class KeypadNumberMirror : MonoBehaviour {
    [SerializeField] Button associatedButton;
    [SerializeField] KeypadNumberAnimation numberAnimator;


    void Start() {
        associatedButton.onClick.AddListener(() => numberAnimator.PlayPressedAnimation());
    }
}
