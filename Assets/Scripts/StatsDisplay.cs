using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class StatsDisplay : MonoBehaviour {
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI infoText;

    string statsDataRaw;
    string[] statsDataEntries;
    int showingTab = 0;

    [SerializeField]List<string> containedDates = new List<string>();


    
    void Start() {
        statsDataRaw = StatsCollection.instance.GetStatsRawFileData();
        DeserializeData();
        UpdateDisplayedInfo();

        void DeserializeData() {
            statsDataRaw = statsDataRaw.Trim();
            statsDataEntries = statsDataRaw.Split('\n');
            foreach (string line in statsDataEntries) {
                string date = line.Split(' ')[0];
                if (!containedDates.Contains(date)) {
                    containedDates.Add(date);
                }
            }
        }
    }

    void Update() {
        if (Keyboard.current.iKey.wasPressedThisFrame) {
            if(canvasGroup.alpha == 0) { UpdateDisplayedInfo(); }

            canvasGroup.DOFade(canvasGroup.alpha > 0 ? 0 : 1, 0.25f);
        }

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame) {
            showingTab++;
            showingTab = Mathf.Clamp(showingTab, 0, containedDates.Count - 1);
            UpdateDisplayedInfo();
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame) {
            showingTab--;
            showingTab = Mathf.Clamp(showingTab, 0, containedDates.Count - 1);
            UpdateDisplayedInfo();
        }
    }

    void UpdateDisplayedInfo() {
        int try1 = 0;
        int try2 = 0;
        int try3 = 0;
        int try4 = 0;
        int win1 = 0;
        int win2 = 0;
        int win3 = 0;
        int win4 = 0;

        foreach (string line in statsDataEntries) {
            if (line.Split(' ')[0] == containedDates[showingTab]) {
                int tries = int.Parse(line[line.Length - 4].ToString());
                bool hasWon = line[line.Length - 1] == 'W';
                switch (tries) {
                    case 1:
                        try1++;
                        if(hasWon) { win1++; }
                        break;
                    case 2:
                        try2++;
                        if (hasWon) { win2++; }
                        break;
                    case 3:
                        try3++;
                        if (hasWon) { win3++; }
                        break;
                    case 4:
                        try4++;
                        if (hasWon) { win4++; }
                        break;
                }
            }

            string updatedInfo = $"{containedDates[showingTab]}\n";
            updatedInfo += $"   1: {try1} ({win1})\n" +
                $"   2: {try2} ({win2})\n" +
                $"   3: {try3} ({win3})\n" +
                $"   4: {try4} ({win4})\n";

            infoText.text = updatedInfo;
        }
    }
}
