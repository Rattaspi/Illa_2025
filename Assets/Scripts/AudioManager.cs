using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;

    [SerializeField] AudioSource audioPlayer;
    [SerializeField] AudioClip selectNumber;
    [SerializeField] AudioClip invalidSelection;
    [SerializeField] AudioClip loseAudio;
    [SerializeField] AudioClip winAudio;
    [SerializeField] AudioClip applauseAudio;
    [SerializeField] AudioClip buttonPressed;

    void Awake() {
        instance = this;
    }

    public void PlayInvalidSelectionSound() {
        audioPlayer.PlayOneShot(invalidSelection);
    }

    public void PlaySelectNumber() {
        audioPlayer.PlayOneShot(selectNumber);
    }

    public void PlayLose() {
        audioPlayer.PlayOneShot(loseAudio);
    }

    public void PlayWin() {
        audioPlayer.PlayOneShot(winAudio);
        audioPlayer.PlayOneShot(applauseAudio);
    }

    public void PlayButtonPressed() {
        audioPlayer.PlayOneShot(buttonPressed);
    }
}
