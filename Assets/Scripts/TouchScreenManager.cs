using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TouchScreenManager : MonoBehaviour {
    List<int> numbersClicked = new List<int>();

    [SerializeField] TextMeshProUGUI[] numberDigits;
    [SerializeField] TextMeshProUGUI[] mirroredNumberDigits;

    public void NumberPressed(int num) {
        if (numbersClicked.Count < 3) {
            numbersClicked.Add(num);
            UpdateDisplayedNumber();
        }
    }

    public void Backspace() {
        if (numbersClicked.Count > 0) {
            numbersClicked.RemoveAt(numbersClicked.Count - 1);
            UpdateDisplayedNumber();
        }
    }

    public void Clear() {
        numbersClicked.Clear();
        UpdateDisplayedNumber();
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

}
