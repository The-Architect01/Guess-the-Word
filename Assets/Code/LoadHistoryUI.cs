using SaveSystem;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class LoadHistoryUI : MonoBehaviour {

    [SerializeField] Image Background;
    [SerializeField] TextMeshProUGUI CorrectWord;
    [SerializeField] TextMeshProUGUI TimeToWin;
    [SerializeField] HistoryDisplay[] Guesses;    

    public void Populate(History history) {
        CorrectWord.text = history.CorrectAnswer;
        TimeToWin.text = $"Time: {System.Math.Round(history.TimeToWin, 3).ToString("#.000")}";

        int i = 0;
        foreach (string s in history.GuessHistory) {
            Guesses[i].Populate(s, history.CorrectAnswer);
            i++;
        }
       
    }


}
