using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour {
    const float resetTime = 2f;
    float resetCounter;

    void Update() {
        if (Keyboard.current.rKey.isPressed) {
            resetCounter += Time.deltaTime;
        }
        else {
            resetCounter = 0;
        }

        if (resetCounter >= resetTime) {
            this.enabled = false;
            FaderManager.instance.FadeOut().OnComplete(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        }
    }
}
