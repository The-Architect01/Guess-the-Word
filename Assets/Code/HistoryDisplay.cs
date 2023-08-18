using SaveSystem;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class HistoryDisplay : MonoBehaviour {

    [SerializeField] TextMeshProUGUI GuessedWord;
    [SerializeField] Image[] LettersInWord;

    public void Populate(string History, string CorrectWord) {
        string WORD = CorrectWord.ToUpper();
        GuessedWord.text = History;
        for (int i = 0; i < LettersInWord.Length; i++) {
            Image image = LettersInWord[i];
            image.color = WORD[i] == History[i] ? new Color32(29, 188, 72, 255) : WORD.Contains(History[i]) ? new Color32(243, 164, 0, 255) : new Color32(183, 44, 44, 255);
            image.GetComponentInChildren<TextMeshProUGUI>().text = History[i].ToString();
        }
    }

}
