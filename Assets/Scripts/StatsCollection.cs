using UnityEngine;
using System.IO;
using System;

public class StatsCollection : MonoBehaviour {
    /*
     * TODO
     * Hacer un fichero para guardar la informacion por cada sesion y listarlos todos en raw
     * Hacer display para mostrar la info por dia
     */

    public static StatsCollection instance;

    int selectionAmount;

    string filepath;
    const string filename_stats = "2025Illa_Stats.txt";
    const string filename_statsraw = "2025Illa_StatsRAW.txt";

    void Awake() {
        instance = this;
        filepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    }

    void Start() {
        AppManager.instance.onRevealEnd.AddListener(UpdateInfo);
    }

    void UpdateInfo() {

        // STATS FILE
        string originalFileStatsData = ReadDataFromFile(filename_stats);
        string[] splittedFileStatsData = originalFileStatsData.Split("\n");

        int numbersToChoose = AppManager.instance.GetNumbersToChoose();
        string[] splittedLineToEdit = splittedFileStatsData[numbersToChoose].Split(": ");
        splittedLineToEdit[1] = (int.Parse(splittedLineToEdit[1]) + 1).ToString();
        splittedFileStatsData[numbersToChoose] = splittedLineToEdit[0] + ": " + splittedLineToEdit[1];

        WriteToFile(filename_stats, string.Join('\n', splittedFileStatsData));

        // STATS RAW FILE
        string originalStatsRawFileData = ReadDataFromFile(filename_statsraw);
        string updatedStatsRawFileData = $"{DateTime.Now.ToString()}--{AppManager.instance.GetNumbersToChoose()}--{(AppManager.instance.CheckWinner()?"W":"L")}\n";
        updatedStatsRawFileData += originalStatsRawFileData;
        WriteToFile(filename_statsraw, updatedStatsRawFileData);
    }

    string ReadDataFromFile(string filename) {
        string fileData = "";
        if(File.Exists(Path.Combine(filepath, filename))) {
            fileData = File.ReadAllText(Path.Combine(filepath, filename));
        }
        return fileData;
    }

    void WriteToFile(string filename, string text) {
        File.WriteAllText(Path.Combine(filepath, filename), text);
    }

    public void SetCurrentSceneSelectionAmount(int amount) {
        selectionAmount = amount;
    }

    public string GetStatsRawFileData() {
        return ReadDataFromFile(filename_statsraw);
    }
}
