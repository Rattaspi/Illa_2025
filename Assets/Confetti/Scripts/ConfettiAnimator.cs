using UnityEngine;
using UnityEngine.UI;

public class ConfettiAnimator : MonoBehaviour {
    [SerializeField] Sprite[] animationSprites;
    [SerializeField] int fps;

    Image image;
    float frameTime;
    float counter = 0;
    int currentFrame = 0;

    void Start(){
        frameTime = 1f / fps;
        image = GetComponent<Image>();
        image.sprite = animationSprites[currentFrame];
    }

    void Update(){
        if (currentFrame < animationSprites.Length) {
            if (counter > frameTime) {
                image.sprite = animationSprites[currentFrame];
                currentFrame++;
                counter -= frameTime;
            }
            counter += Time.deltaTime;
        }
        else {
            this.enabled = false;
        }
    }
}
