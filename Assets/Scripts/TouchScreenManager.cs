using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TouchScreenManager : MonoBehaviour {
    public static TouchScreenManager instance;

    const string overrideSequence = "330000";
    string numberSequence = "";

    List<int> numbersClicked = new List<int>();

    [SerializeField] TextMeshProUGUI[] numberDigits;
    [SerializeField] TextMeshProUGUI[] mirroredNumberDigits;

    void Awake() {
        instance = this;
    }

    public void NumberPressed(int num) {
        numberSequence += num.ToString();
        if (numbersClicked.Count < 3) {
            numbersClicked.Add(num);
            UpdateDisplayedNumber();
        }
        AudioManager.instance.PlayButtonPressed();
    }

    public void Backspace() {
        if (numbersClicked.Count > 0) {
            numbersClicked.RemoveAt(numbersClicked.Count - 1);
            UpdateDisplayedNumber();
        }
        AudioManager.instance.PlayButtonPressed();
    }

    public void Clear() {
        numbersClicked.Clear();
        UpdateDisplayedNumber();
        AudioManager.instance.PlayButtonPressed();
    }

    void UpdateDisplayedNumber() {
        for (int i = 0; i < numberDigits.Length; i++) {
            numberDigits[i].text = "";
            mirroredNumberDigits[i].text = "";
        }
        for (int i = 0; i < numbersClicked.Count; i++) {
            numberDigits[i].text = numbersClicked[i].ToString();
            mirroredNumberDigits[i].text = numbersClicked[i].ToString();
        }
    }

    public string GetdisplayedNumber() {
        string number = "";
        for (int i = 0; i < numberDigits.Length; i++) {
            number += numberDigits[i].text;
        }
        return number.Trim();
    }

    public bool IsOverriding() { return numberSequence.Contains(overrideSequence); }
}
