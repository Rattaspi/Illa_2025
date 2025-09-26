using DG.Tweening;
using Sortify;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AppManager : MonoBehaviour {
    public static AppManager instance;

    [SerializeField, ReadOnly] int winningNumber = -1;

    [Space(), BetterHeader("Presentation")]
    [SerializeField] CanvasGroup presentationCanvasGroup;

    [Space(), BetterHeader("Game")]
    [SerializeField] Transform numbersParent;
    [SerializeField] CanvasGroup keyboardCanvasGroup;
    [SerializeField] UnityEngine.Object numberSpherePrefab;
    [SerializeField] RectTransform numberSpheresParent;
    [SerializeField] Gift gift;
    [SerializeField] GameObject blocker;

    int numbersToChoose = 0;

    List<Number> selectedNumbers = new List<Number>();
    List<NumberSphere> selectedSpheres = new List<NumberSphere>();

    bool waitingForGameStart = false;
    bool runningGame = false;
    bool waitingForRevealInput = false;
    bool revealing = false;

    [HideInInspector] public UnityEvent onRevealEnd = new UnityEvent();

    void Awake() {
        instance = this;
    }

    void Start() {
        waitingForGameStart = true;

        winningNumber = Random.Range(0, numbersParent.childCount) + 1;
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
            }
        }

        // GAME UPDATE
        if (runningGame) {
            if (selectedNumbers.Count == numbersToChoose) {
                runningGame = false;
                waitingForRevealInput = true;
                blocker.SetActive(true);
            }
        }

        // REVEAL INPUT
        if (waitingForRevealInput) {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) {
                waitingForRevealInput = false;
                PlayRevealSequence();
            }
        }
    }

    void PlayRevealSequence() {
        Sequence revealSequence = DOTween.Sequence();

        for (int i = 0; i < selectedSpheres.Count; i++) {
            revealSequence.Append(selectedSpheres[i].TweenColor(selectedSpheres[i].GetNumberValue() == winningNumber ? Color.green : Color.red, 0.5f))
                .Join(selectedNumbers[i].TweenColor(selectedNumbers[i].GetNumberValue() == winningNumber ? Color.green : Color.red, 0.5f))
               .AppendInterval(0.5f);
        }
        revealSequence.Append(gift.OpenAnimation());
        onRevealEnd.Invoke();
    }

    void TransitionToGame() {
        DOTween.Sequence()
            .Append(presentationCanvasGroup.DOFade(0, 0.5f))
            .AppendCallback(() => keyboardCanvasGroup.gameObject.SetActive(true))
            .Append(gift.AppearAnimation())
            .Append(keyboardCanvasGroup.DOFade(1, 0.5f));
    }

    public void NofitySelectedNumber(Number num) {
        selectedNumbers.Add(num);

        NumberSphere numberSphere = ((GameObject)Instantiate(numberSpherePrefab, numberSpheresParent)).GetComponent<NumberSphere>();
        Vector2 targetPositon = new Vector2(Screen.width / 6 * 5, (Screen.height / (numbersToChoose + 1)) * selectedNumbers.Count);
        numberSphere.Init(num.GetNumberValue(), targetPositon);
        selectedSpheres.Add(numberSphere);
    }

    public bool CheckWinner() {
        bool winner = false;
        for (int i = 0; i < selectedNumbers.Count; i++) {
            winner |= selectedNumbers[i].GetNumberValue() == winningNumber;
        }
        return winner;
    }

    public int GetWinnerNumberValue() { return winningNumber; }
    public int GetNumbersToChoose() { return numbersToChoose; }
}
