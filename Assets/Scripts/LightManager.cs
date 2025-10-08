using Sortify;
using UnityEngine;
using UnityEngine.UI;

public class LightManager : MonoBehaviour {
    public static LightManager instance;

    [SerializeField] RawImage screen1ColorImage;
    [SerializeField] RawImage screen2ColorImage;
    [SerializeField] RawImage screen3ColorImage;

    [Space(), BetterHeader("DMX scenes")]
    [SerializeField] Animator dmxSceneAnimator;

    const string SELECT_NUMBER_ANIMTRIGGER = "SelectNumber";
    const string OPEN_GIFT_ANIMTRIGGER = "OpenGift";
    const string WIN_ANIMTRIGGER = "Win";
    const string LOSE_ANIMTRIGGER = "Lose";


    readonly int[] adresses = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    void Awake() {
        instance = this;
    }

    void Update() {
        byte[] values = {(byte)(screen1ColorImage.color.r * 254f), (byte)(screen1ColorImage.color.g * 254f), (byte)(screen1ColorImage.color.b * 254f),
            (byte)(screen2ColorImage.color.r * 254f), (byte)(screen2ColorImage.color.g * 254f), (byte)(screen2ColorImage.color.b * 254f),
            (byte)(screen3ColorImage.color.r * 254f), (byte)(screen3ColorImage.color.g * 254f), (byte)(screen3ColorImage.color.b * 254f)};

        ArtNetSender.instance.UpdatePacketInfo(adresses, values);
    }

    void OnDisable() {
        byte[] values = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        ArtNetSender.instance.UpdatePacketInfo(adresses, values);
        ArtNetSender.instance.ForcePacketSend();
    }

    public void TriggerSelectNumber() { dmxSceneAnimator.SetTrigger(SELECT_NUMBER_ANIMTRIGGER); }
    public void TriggerOpenGift() { dmxSceneAnimator.SetTrigger(OPEN_GIFT_ANIMTRIGGER); }
    public void TriggerWin() { dmxSceneAnimator.SetTrigger(WIN_ANIMTRIGGER); }
    public void TriggerLose() { dmxSceneAnimator.SetTrigger(LOSE_ANIMTRIGGER); }
}
