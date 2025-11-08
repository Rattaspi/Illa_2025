using DG.Tweening;
using Sortify;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AppManager_Final : MonoBehaviour {
    int maxNumbers = 0;
    [SerializeField, ReadOnly] int winningNumber = 0;

    bool gameStarted = false;
    bool gameFinished = false;

    [BetterHeader("TOUCH SCREEN")]
    [SerializeField] GameObject blocker;
    [SerializeField] UnityEngine.Object selectableNumberPrefab;
    [SerializeField] RectTransform selectableNumbersParent;
    [SerializeField] CanvasGroup videoPubliTouchScreenCanvasGroup;

    List<RectTransform> selectableNumbers = new List<RectTransform>();
    List<NumberForFinal> orderedSelectedNumbers = new List<NumberForFinal>();

    [Space(), BetterHeader("LED SCREEN")]
    [SerializeField] RectTransform selectedNumbersParent;
    [SerializeField] CanvasGroup videoPubliLedScreenCanvasGroup;
    [SerializeField] GameObject[] confettiObjects;

    [Space(), BetterHeader("GIFT SEQUENCE")]
    [SerializeField] RectTransform giftLasso;
    [SerializeField] RectTransform giftHorizontalStripe;
    [SerializeField] RectTransform ledScreenCanvas;
    [SerializeField] RectTransform giftVerticalStripe;
    [SerializeField] RectTransform winnerSphere;

    List<RectTransform> selectedNumbers = new List<RectTransform>();
    const float ledScreenNumbersOffset = 1000f;

    bool override1 = false;
    bool override2 = false;

    void Awake() {
#if !UNITY_EDITOR
        Display.displays[0].Activate();
        Display.displays[1].Activate();
#endif
    }

    void Start() {

        SetupNumbers();

        winningNumber = Random.Range(1, maxNumbers + 1);
    }

    void SetupNumbers() {
        // Get the max numbers from config file
        string configFilePath = Path.Combine(Application.streamingAssetsPath, "config.txt");
        if (File.Exists(configFilePath)) {
            string[] configLines = File.ReadAllLines(configFilePath);
            for (int i = 0; i < configLines.Length; i++) {
                string[] lineItems = configLines[i].Split(':');
                if (lineItems[0] == "final-maxnumbers") { maxNumbers = int.Parse(lineItems[1]); }
            }
        }

        // Create and configure all the numbers
        for (int i = 0; i < maxNumbers; i++) {
            // SELECTABLE NUMBERS - TOUCH SCREEN
            GameObject instantiatedTouchScreenNumber = (GameObject)Instantiate(selectableNumberPrefab, selectableNumbersParent);
            instantiatedTouchScreenNumber.name = (i + 1).ToString();
            instantiatedTouchScreenNumber.GetComponentInChildren<TextMeshProUGUI>().text = instantiatedTouchScreenNumber.name;
            instantiatedTouchScreenNumber.GetComponent<Button>().onClick.AddListener(() => SelectNumber(int.Parse(instantiatedTouchScreenNumber.name)));
            selectableNumbers.Add(instantiatedTouchScreenNumber.GetComponent<RectTransform>());

            // SELECTED NUMBERS - LED SCREEN
            GameObject instantiatedLedScreenNumber = (GameObject)Instantiate(selectableNumberPrefab, selectedNumbersParent);
            instantiatedLedScreenNumber.name = (i + 1).ToString();
            instantiatedLedScreenNumber.GetComponentInChildren<TextMeshProUGUI>().text = instantiatedTouchScreenNumber.name;
            selectedNumbers.Add(instantiatedLedScreenNumber.GetComponent<RectTransform>());

        }
    }

    void Update() {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) {
            if (!gameStarted && !gameFinished) {
                gameStarted = true;

                StartGame();
            }
            else if (gameStarted && !gameFinished) {
                blocker.SetActive(false);
            }
            else if (gameStarted && gameFinished) {
                FinishGame();
            }
        }
    }

    void StartGame() {
        // Remove grid layout to have free movement to numbers
        Destroy(selectableNumbersParent.GetComponent<GridLayoutGroup>());
        Destroy(selectedNumbersParent.GetComponent<GridLayoutGroup>());

        // Offset led screen numbers outside the viewport
        for (int i = 0; i < selectedNumbers.Count; i++) {
            selectedNumbers[i].position += -Vector3.up * ledScreenNumbersOffset;
        }

        // START GEME SEQUENCE
        Sequence startSequence = DOTween.Sequence();

        // Fade the logo from the touch screen
        startSequence.Append(videoPubliTouchScreenCanvasGroup.DOFade(0, 0.5f))
            .Join(videoPubliLedScreenCanvasGroup.DOFade(0, 0.5f));

        startSequence.OnComplete(() => blocker.SetActive(false));

        // Spawn number sequenced animation
        AnimateNumbers();
    }

    void AnimateNumbers() {
        for (int i = 0; i < selectableNumbers.Count; i++) {
            DOTween.Sequence()
                .PrependInterval(i * 0.1f)
                .Append(selectableNumbers[i].DOMoveY(1250, 0.5f).From().SetEase(Ease.OutBack));
        }
    }

    void SelectNumber(int selectedNumber) {
        LightManager.instance.TriggerSelectNumber();

        orderedSelectedNumbers.Add(selectedNumbers[selectedNumber - 1].GetComponent<NumberForFinal>());

        selectedNumbers[orderedSelectedNumbers.Count - 1].GetComponentInChildren<TextMeshProUGUI>().text = selectedNumber.ToString("00");
        selectedNumbers[orderedSelectedNumbers.Count - 1].name = selectedNumber.ToString();
        DOTween.Sequence()
            .Append(selectableNumbersParent.GetChild(selectedNumber - 1).GetComponent<NumberForFinal>().AnimateOut())
            .Append(selectedNumbers[orderedSelectedNumbers.Count - 1].DOMoveY(ledScreenNumbersOffset, 1f).SetEase(Ease.OutBack).SetRelative(true));

        blocker.SetActive(true);

        if (orderedSelectedNumbers.Count >= maxNumbers) {
            gameFinished = true;
        }

        // Override
        if (override1 && override2) {
            override1 = false;
            override2 = false;
            winningNumber = selectedNumber;
        }
    }

    void FinishGame() {
        StartCoroutine(Reveal());
    }

    List<NumberForFinal> orderedNumbersForReveal;
    IEnumerator Reveal() {
        VideoManager.instance.StopVideo();

        const float timeForFastRoulette = 2.5f;
        const float fastRouletteSelectChangeTime = 0.1f;
        int numberOfIterationsForFastRoulette = Mathf.CeilToInt(timeForFastRoulette / fastRouletteSelectChangeTime);

        // List for revealing order
        orderedNumbersForReveal = Shuffle(orderedSelectedNumbers);
        NumberForFinal winningNumber = selectedNumbersParent.Find(this.winningNumber.ToString()).GetComponent<NumberForFinal>();
        orderedNumbersForReveal.Remove(winningNumber);
        orderedNumbersForReveal.Add(winningNumber);

        while (orderedNumbersForReveal.Count > 2) { // Special reveal for the last 2 numbers
            // ITERATION FOR 1 REVEAL
            List<NumberForFinal> randomlyOrderedNumbersForRoulette = Shuffle(orderedNumbersForReveal);

            LightManager.instance.TriggerRoulette();

            // Roulette
            for (int j = 0; j < numberOfIterationsForFastRoulette; j++) {
                DOTween.Sequence()
                    .Append(randomlyOrderedNumbersForRoulette[j % randomlyOrderedNumbersForRoulette.Count].Select())
                    .PrependCallback(() => AudioManager.instance.PlayButtonPressed());

                yield return new WaitForSeconds(fastRouletteSelectChangeTime);
            }

            // Slow roulette - 6 iterations
            for (int j = 0; j < 6; j++) {
                DOTween.Sequence()
                    .Append(randomlyOrderedNumbersForRoulette[j % randomlyOrderedNumbersForRoulette.Count].Select())
                    .PrependCallback(() => AudioManager.instance.PlayButtonPressed());
                yield return new WaitForSeconds(0.2f + 0.1f * j);
            }

            // Select the last number which is going to be revealed
            orderedNumbersForReveal[0].Select();

            yield return new WaitForSeconds(1f);

            // Animate the number to the center of the screen
            Vector3 originalNumberPosition = orderedNumbersForReveal[0].GetComponent<RectTransform>().position;
            Vector3 originalNumberScale = orderedNumbersForReveal[0].GetComponent<RectTransform>().localScale;

            orderedNumbersForReveal[0].Deselect();
            orderedNumbersForReveal[0].transform.parent = orderedNumbersForReveal[0].transform.parent.parent;


            yield return DOTween.Sequence()
                .Append(orderedNumbersForReveal[0].GetComponent<RectTransform>().DOMove(new Vector3(Screen.width / 2, Screen.height / 2, 0), 0.5f))
                .Join(orderedNumbersForReveal[0].GetComponent<RectTransform>().DOScale(3f, 1f).SetEase(Ease.OutBack));
                

            yield return new WaitForSeconds(3.5f);

            // Reveal
            if (int.Parse(orderedNumbersForReveal[0].name) == this.winningNumber) {
                // WIN
                orderedNumbersForReveal[0].SetGreen();
                AudioManager.instance.PlayWin();
                LightManager.instance.TriggerWin();
            }
            else {
                // LOSE
                orderedNumbersForReveal[0].SetRed();
                AudioManager.instance.PlayLose();
                LightManager.instance.TriggerLose();

                yield return new WaitForSeconds(3f);

                // Get revealed number back to its original posititon
                yield return DOTween.Sequence()
                    .Append(orderedNumbersForReveal[0].GetComponent<RectTransform>().DOMove(originalNumberPosition, 0.5f))
                    .Join(orderedNumbersForReveal[0].GetComponent<RectTransform>().DOScale(originalNumberScale, 0.5f))
                    .Join(orderedNumbersForReveal[0].GetComponent<CanvasGroup>().DOFade(0.4f, 0.5f))
                    .AppendCallback(() => orderedNumbersForReveal[0].transform.parent = selectedNumbersParent);
            }

            yield return new WaitForSeconds(3f);

            orderedNumbersForReveal.RemoveAt(0);
        }

        // LAST 2 NUMBERS
        // SPECIAL REVEAL STARTS HERE

        DOTween.Sequence()
            .Append(orderedNumbersForReveal[0].GetComponent<RectTransform>().DOMove(new Vector3(Screen.width / 2, (Screen.height / 4) * 1, 0), 1f))
            .Join(orderedNumbersForReveal[0].GetComponent<RectTransform>().DOScale(4f, 2f));
        DOTween.Sequence()
            .Append(orderedNumbersForReveal[1].GetComponent<RectTransform>().DOMove(new Vector3(Screen.width / 2, (Screen.height / 4) * 3, 0), 1f))
            .Join(orderedNumbersForReveal[1].GetComponent<RectTransform>().DOScale(4f, 2f));

        yield return new WaitForSeconds(5f);

        // Open gift
        DOTween.Sequence()
            .Append(giftLasso.GetComponent<CanvasGroup>().DOFade(0, 0.5f))
            .Append(giftHorizontalStripe.DOMoveX(ledScreenCanvas.rect.width / 3 + 100, 0.5f).SetRelative(true))
            .Append(giftVerticalStripe.DOMoveY(2000, 0.5f))
            .Append(winnerSphere.DOMoveY(ledScreenCanvas.rect.height / 2, 2.5f).SetEase(Ease.OutBack));

        LightManager.instance.TriggerRoulette();

        // Roulette
        for (int j = 0; j < numberOfIterationsForFastRoulette; j++) {
            DOTween.Sequence()
                .Append(orderedNumbersForReveal[j % orderedNumbersForReveal.Count].Select())
                .PrependCallback(() => AudioManager.instance.PlayButtonPressed());

            yield return new WaitForSeconds(fastRouletteSelectChangeTime);
        }

        // Slow roulette - 10 iterations
        for (int j = 0; j < 10; j++) {
            DOTween.Sequence()
                .Append(orderedNumbersForReveal[j % orderedNumbersForReveal.Count].Select())
                .PrependCallback(() => AudioManager.instance.PlayButtonPressed());
            yield return new WaitForSeconds(0.2f + 0.1f * j);
        }

        // Reveal number from gift screen sphere
        yield return DOTween.Sequence()
            .Append(winnerSphere.DOScaleX(0, 0.5f))
            .AppendCallback(() => {
                winnerSphere.GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
                winnerSphere.GetChild(0).GetComponent<TextMeshProUGUI>().text = this.winningNumber.ToString("00");
            })
            .Append(winnerSphere.DOScaleX(winnerSphere.localScale.y, 0.5f));

        yield return new WaitForSeconds(1f);

        // Reveal both remaining numbers
        if (int.Parse(orderedNumbersForReveal[0].name) == this.winningNumber) {
            // WIN
            orderedNumbersForReveal[0].SetGreen();
        }
        else {
            // LOSE
            orderedNumbersForReveal[0].SetRed();
        }
        if (int.Parse(orderedNumbersForReveal[1].name) == this.winningNumber) {
            // WIN
            orderedNumbersForReveal[1].SetGreen();
        }
        else {
            // LOSE
            orderedNumbersForReveal[1].SetRed();
        }

        // WIN SEQUENCE
        AudioManager.instance.PlayWin();
        LightManager.instance.TriggerWin();
        foreach (GameObject go in confettiObjects) {
            go.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);

        orderedNumbersForReveal[0].GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        orderedNumbersForReveal[1].GetComponent<RectTransform>().DOMove(new Vector3(Screen.width / 2, Screen.height / 2, 0), 0.5f);
        orderedNumbersForReveal[1].GetComponent<RectTransform>().DOScale(8.35f, 1f).SetEase(Ease.OutBack);
        orderedNumbersForReveal[1].transform.parent = selectedNumbersParent.parent;
    }

    public List<NumberForFinal> Shuffle(List<NumberForFinal> list) {
        List<NumberForFinal> shuffledList = new List<NumberForFinal>(list);

        System.Random rng = new System.Random();
        int n = shuffledList.Count;
        while (n > 1) {
            n--;
            int k = rng.Next(n + 1);
            NumberForFinal value = shuffledList[k];
            shuffledList[k] = shuffledList[n];
            shuffledList[n] = value;
        }

        return shuffledList;
    }

    public void Override1() {
        override1 = true;
    }
    public void Override2() {
        override2 = true;
    }
}
