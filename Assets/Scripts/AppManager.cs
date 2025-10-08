using DG.Tweening;
using Sortify;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AppManager : MonoBehaviour {
    public static AppManager instance;

    [SerializeField, ReadOnly] int winningNumber = -1;

    [Space(), BetterHeader("Config")]
    [SerializeField] int maxNumbers = 350;

    [Space(), BetterHeader("Touch screen")]
    [SerializeField] RectTransform touchScreenCanvas;
    [SerializeField] RectTransform sphereToTypeNumber;
    [SerializeField] RectTransform[] triesSpheresDisplay;
    [SerializeField] RectTransform playButton;
    [SerializeField] GameObject blocker;

    [Space(), BetterHeader("LED screen")]
    [SerializeField] RectTransform ledScreenCanvas;
    [SerializeField] CanvasGroup chooseNumberHeaderCanvasGroup;
    [SerializeField] UnityEngine.Object sphereNumberForLedScreenPrefab;
    [SerializeField] CanvasGroup screen2Elements;
    [SerializeField] RectTransform dummyWinnerSphere;
    [SerializeField] RectTransform winnerSphere;
    [SerializeField] RectTransform giftLasso;
    [SerializeField] RectTransform giftVerticalStripe;
    [SerializeField] RectTransform giftHorizontalStripe;

    [Space(), BetterHeader("Video publi")]
    [SerializeField] CanvasGroup videoPubliCanvasGroup;
    [SerializeField] CanvasGroup logoIllaCanvasGroup;

    [Header("Win screen")]
    [SerializeField] GameObject[] confettiObjects;
    [SerializeField] CanvasGroup[] winMessages;

    [Header("Lose screen")]
    [SerializeField] CanvasGroup[] loseMessages;

    Vector3 originalSphereToTypeNumberPosition;
    float playButtonXPositon;

    int numbersToChoose = 0;

    List<int> selectedNumbers = new List<int>();
    List<GameObject> selectedNumbersInScreen2 = new List<GameObject>();

    bool waitingForGameStart = false;
    bool runningGame = false;
    bool waitingForRevealInput = false;

    [HideInInspector] public UnityEvent onRevealStart = new UnityEvent();

    void Awake() {
        instance = this;
    }

    void Start() {
        waitingForGameStart = true;

        winningNumber = Random.Range(0, maxNumbers) + 1;

        playButtonXPositon = playButton.position.x;
        playButton.position = new Vector3(0, playButton.position.y, playButton.position.z);

        originalSphereToTypeNumberPosition = sphereToTypeNumber.position;
    }

    void Update() {
        // Game start
        if (waitingForGameStart) {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) {
                numbersToChoose = 1;
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame) {
                numbersToChoose = 2;
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame) {
                numbersToChoose = 3;
            }
            else if (Keyboard.current.digit4Key.wasPressedThisFrame) {
                numbersToChoose = 4;
            }

            if (numbersToChoose > 0) {
                StatsCollection.instance.SetCurrentSceneSelectionAmount(numbersToChoose);
                waitingForGameStart = false;
                runningGame = true;

                TransitionToGame();
                blocker.SetActive(false);
                if(videoPubliCanvasGroup.alpha > 0) { 
                    videoPubliCanvasGroup.DOFade(0, 0.5f);
                    logoIllaCanvasGroup.DOFade(0, 0.5f);
                }
            }
        }

        // GAME UPDATE
        if (runningGame) {
            if (selectedNumbers.Count == numbersToChoose) {
                runningGame = false;
                waitingForRevealInput = true;
            }
        }

        // REVEAL INPUT
        if (waitingForRevealInput) {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) {
                blocker.SetActive(true);
                waitingForRevealInput = false;
                PlayRevealSequence();
            }
        }

        // PUBLI VIDEO MANAGEMENT
        if (Keyboard.current.vKey.wasPressedThisFrame) {
            if(videoPubliCanvasGroup.alpha < 0.5f) { 
                videoPubliCanvasGroup.DOFade(1, 0.5f); 
                logoIllaCanvasGroup.DOFade(1, 0.5f); 
            }
            else { 
                videoPubliCanvasGroup.DOFade(0, 0.5f); 
                logoIllaCanvasGroup.DOFade(0, 0.5f); 
            }
        }
    }

    void TransitionToGame() {
        Sequence gameStartSequence = DOTween.Sequence();
        for (int i = 0; i < numbersToChoose; i++) {
            triesSpheresDisplay[i].gameObject.SetActive(true);
            gameStartSequence.Append(triesSpheresDisplay[i].GetComponent<CanvasGroup>().DOFade(1, 0.5f));
        }
        gameStartSequence.AppendInterval(0.5f)
            .Append(AnimateNextSphere());
    }

    public void SelectNumber() {
        // Check if the selected number is valid
        string selectedNumberString = TouchScreenManager.instance.GetdisplayedNumber();
        int selectedNumberInt = -1;
        if (!string.IsNullOrEmpty(selectedNumberString)) { selectedNumberInt = int.Parse(selectedNumberString); }

        if (selectedNumberInt > 0 && selectedNumberInt <= maxNumbers && !selectedNumbers.Contains(selectedNumberInt)) {
            // NUMBER VALID
            LightManager.instance.TriggerSelectNumber();
            AudioManager.instance.PlaySelectNumber();
            Vector3 spawnSpherePosition = new Vector3(ledScreenCanvas.rect.width / 3 + 100 + 125 * selectedNumbers.Count, -150, 0);
            RectTransform sphereNumberForLedScreen = ((GameObject)Instantiate(sphereNumberForLedScreenPrefab, spawnSpherePosition, Quaternion.identity, chooseNumberHeaderCanvasGroup.transform.parent.parent)).GetComponent<RectTransform>();
            selectedNumbersInScreen2.Add(sphereNumberForLedScreen.gameObject);

            selectedNumbers.Add(selectedNumberInt);
            sphereNumberForLedScreen.GetComponentInChildren<TextMeshProUGUI>().text = selectedNumberString;

            if (chooseNumberHeaderCanvasGroup.alpha > 0) { chooseNumberHeaderCanvasGroup.DOFade(0, 0.5f); }

            blocker.SetActive(true);

            Sequence postSelectNumberSequence = DOTween.Sequence()
                .Append(sphereToTypeNumber.DOMoveY(1000, 1f).SetRelative().SetEase(Ease.InBack))
                .Join(sphereToTypeNumber.DOScale(0.5f, 1f))
                .Append(sphereNumberForLedScreen.DOMoveY(ledScreenCanvas.rect.height, 1f).SetRelative(true).SetEase(Ease.OutBack))
                .AppendCallback(delegate {
                    sphereToTypeNumber.GetComponent<CanvasGroup>().alpha = 0;
                    sphereToTypeNumber.position = originalSphereToTypeNumberPosition;
                    sphereToTypeNumber.localScale = Vector3.one;
                    TouchScreenManager.instance.Clear();
                });

            if (selectedNumbers.Count < numbersToChoose) { 
                postSelectNumberSequence.Append(AnimateNextSphere()); 
                blocker.SetActive(false);
            }
            else { 
                VideoManager.instance.StopVideo();
                postSelectNumberSequence.Append(playButton.GetComponent<CanvasGroup>().DOFade(0, 0.5f)); 
            }
        }
        else {
            // NUMBER INVALID
            TouchScreenManager.instance.Clear();
            DOTween.Kill(sphereToTypeNumber, true);
            sphereToTypeNumber.DOShakePosition(0.5f, new Vector3(20f, 0), 25);
            AudioManager.instance.PlayInvalidSelectionSound();
        }
    }

    Tween AnimateNextSphere() {
        int sphereToMove = selectedNumbers.Count;
        return DOTween.Sequence()
            .Append(triesSpheresDisplay[sphereToMove].DOScale(4.8f, 1f).SetEase(Ease.OutQuad))
            .Join(triesSpheresDisplay[sphereToMove].DOMove(sphereToTypeNumber.position, 0.5f))
            .Append(sphereToTypeNumber.GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
            .Append(triesSpheresDisplay[sphereToMove].DOScale(0, 0.5f))
            .AppendCallback(() => triesSpheresDisplay[sphereToMove].gameObject.SetActive(false))
            .Append(playButton.DOMoveX(playButtonXPositon, 0.5f).SetEase(Ease.OutBack))
            .Append(triesSpheresDisplay[sphereToMove].parent.DOMoveX(-150, 3f).SetRelative());
    }

    void PlayRevealSequence() {
        onRevealStart.Invoke();

        if (TouchScreenManager.instance.IsOverriding()) { winningNumber = selectedNumbers[0]; }
        winnerSphere.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetWinnerNumberValue().ToString();
        LightManager.instance.TriggerOpenGift();

        Sequence revealSequence = DOTween.Sequence();

        // LED SCREEN 1
        revealSequence.Append(dummyWinnerSphere.DOScale(0.1f, 0.25f).From())
            .Join(dummyWinnerSphere.DOMoveY(ledScreenCanvas.rect.height, 2f).SetRelative(true))
            .Join(dummyWinnerSphere.GetComponent<CanvasGroup>().DOFade(1, 0.5f));

        // LED SCREEN 3 - GIFT
        revealSequence.Append(giftLasso.GetComponent<CanvasGroup>().DOFade(0, 0.5f))
            .Append(giftHorizontalStripe.DOMoveX(ledScreenCanvas.rect.width / 3 + 100, 0.5f).SetRelative(true))
            .Append(giftVerticalStripe.DOMoveY(ledScreenCanvas.rect.height + 100, 0.5f).SetRelative(true))
            .Append(winnerSphere.DOMoveY(ledScreenCanvas.rect.height / 2, 2.5f).SetEase(Ease.OutBack))
            .AppendInterval(1f)
            .Append(winnerSphere.DOMoveX(ledScreenCanvas.rect.width / 2, 2f).SetEase(Ease.OutQuad))
            .Join(winnerSphere.DOScale(5.5f, 2f).SetEase(Ease.OutQuad))
            .Join(screen2Elements.DOFade(0, 0.5f))
            .Append(winnerSphere.GetChild(0).GetComponent<CanvasGroup>().DOFade(1, 1f));

        // LED SCREEN 2 - COLORS TO SPHERES
        revealSequence.AppendInterval(1f);
        bool alreadyWon = false;
        for (int i = 0; i < selectedNumbersInScreen2.Count; i++) {
            bool isNumberWinner = selectedNumbers[i] == GetWinnerNumberValue();
            alreadyWon |= isNumberWinner;
            Color colorToUse = isNumberWinner ? new Color(0.5f, 1f, 0.5f) : new Color(1f, 0.5f, 0.5f);
            revealSequence.Append(selectedNumbersInScreen2[i].GetComponent<Image>().DOColor(colorToUse, 0.5f));
            if (isNumberWinner) { revealSequence.AppendCallback(() => AudioManager.instance.PlayWin()); }
            if(!alreadyWon) { revealSequence.AppendCallback(()=>AudioManager.instance.PlayLose()); }
        }

        revealSequence.AppendCallback(delegate {
            if (alreadyWon) { 
                LightManager.instance.TriggerWin(); 
                ShowWinScreen();
            }
            else { 
                LightManager.instance.TriggerLose(); 
                ShowLoseScreen();
            }
        });
    }


    public bool CheckWinner() {
        bool winner = false;
        for (int i = 0; i < selectedNumbers.Count; i++) {
            winner |= selectedNumbers[i] == winningNumber;
        }
        return winner;
    }

    void ShowWinScreen() {
        foreach(GameObject go in confettiObjects) {
            go.SetActive(true);
        }
        foreach (CanvasGroup canvasGroup in winMessages) {
            canvasGroup.DOFade(1, 1f);
        }
    }

    void ShowLoseScreen() {
        foreach (CanvasGroup canvasGroup in loseMessages) {
            canvasGroup.DOFade(1f, 1f);
        }
    }

    public int GetWinnerNumberValue() { return winningNumber; }
    public int GetNumbersToChoose() { return numbersToChoose; }
}
