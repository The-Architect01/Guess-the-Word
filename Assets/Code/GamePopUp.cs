using UnityEngine;
using TMPro;
using DictionaryAPI;
using UnityEngine.SceneManagement;

public class GamePopUp : MonoBehaviour {

    [Header("Win/Lose Hosts")]
    [SerializeField] GameObject Win;
    [SerializeField] GameObject Lose;

    [Header("Win/Lose Text")]
    [SerializeField] TextMeshProUGUI WinGUI;
    [SerializeField] TextMeshProUGUI LoseGUI;

    DictionaryEntry Entry;

    [Header("Error Messages")]
    [SerializeField] Canvas ErrorCanvas;
    [SerializeField] TextMeshProUGUI ErrorMessage;

    public void ShowWin() {
        Entry = DictionaryFetchService.FetchedEntry;
        WinGUI.transform.parent.parent.gameObject.SetActive(true);
        WinGUI.transform.parent.gameObject.SetActive(true);
        WinGUI.text = $"<align=center><size=60><b>Yay!</b></size></align>\n" +
            $"You were able to guess the word!\n\n" +
            $"The word was <b>{Entry.word}</b>.\n" +
            $"<b>{Entry.word[0].ToString().ToUpper() + Entry.word.Substring(1, Entry.word.Length - 1)}</b> is a {Entry.meanings[0].partOfSpeech} meaning {Entry.meanings[0].definitions[0].definition.ToLower()}";
    }

    public void ShowLose() {
        Entry = DictionaryFetchService.FetchedEntry;
        Lose.SetActive(true);
        Lose.transform.parent.gameObject.SetActive(true);
        LoseGUI.text = $"<align=center><size=60><b>Oh no!</b></size></align>\n" +
            $"You were unable to guess the word!\n\n" +
            $"The word was <b>{Entry.word}</b>.\n" +
            $"<b>{Entry.word[0].ToString().ToUpper() + Entry.word.Substring(1, Entry.word.Length - 1)}</b> is a {Entry.meanings[0].partOfSpeech} meaning {Entry.meanings[0].definitions[0].definition.ToLower()}";
    }

    public void ShowError(string Word) {
        ErrorMessage.text = $"{Word} is not in my dictionary!\nPlease enter a different word!";
        ErrorCanvas.gameObject.SetActive(true);
    }

    public void ShowError(int WordLength) {
        ErrorMessage.text = $"You entered a word with {WordLength} letters!\nPlease enter a word with 5 letters!";
        ErrorCanvas.gameObject.SetActive(true);
    }

    public void HideError() {
        ErrorCanvas.gameObject.SetActive(false);
        GameObject.Find("Main Screen").GetComponent<GameUIHost>().EnableButtons();
    }

    public void GoToHome() { SceneManager.LoadScene(0); }

    public void Reload() { SceneManager.LoadScene(1); }

    private void Update() {
        if (Engine.Instance.EventConsumed) return;
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)) 
            HideError();
    }

}