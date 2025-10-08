using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour {
    public static VideoManager instance;

    [SerializeField] VideoPlayer player;
    [SerializeField] VideoClip startClip;
    [SerializeField] VideoClip loopClip;
    [SerializeField] VideoClip endClip;

    bool playingStartClip = true;
    bool changeToFinalClip = false;

    void Awake() {
        instance = this;
    }

    void Update() {
        if (Time.timeSinceLevelLoad > 1f && playingStartClip && !player.isPlaying) {
            playingStartClip = false;
            player.clip = loopClip;
            player.isLooping = true;
            player.Play();
        }
        else if (changeToFinalClip && !player.isPlaying) {
            changeToFinalClip = false;
            player.clip = endClip;
            player.isLooping = false;
            player.Play();
        }
    }

    public void StopVideo() {
        changeToFinalClip = true;
        player.isLooping = false;
    }
}
