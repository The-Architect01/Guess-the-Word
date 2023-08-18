using System.Collections;
using System.Linq;

using TMPro;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.UI;

public class GameUIHost : MonoBehaviour {

    [Header("Engine Data")]
    [SerializeField] string Word;
    [SerializeField] float TTW;
    const string LETTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const char ESCAPE_BACK = '↩';
    const char ESCAPE_ENTER = '↪';
    bool HasWin;
    bool CanGuess = true;

    [Header("UI Data")]
    [SerializeField] int CurGuessCount = 0;
    [SerializeField] int CurCharIndex = 0;
    [SerializeField] TextMeshProUGUI Hint;
    [SerializeField] GameObject GuessHost;
    [SerializeField] Button[] AlphaButtons;

    [Header("Colors")]
    public Color Correct;
    public Color WrongPlace;
    public Color Incorrect;

    // Start is called before the first frame update
    IEnumerator Start() {
        if (Engine.Instance.Settings.HasUnfinishedGame) {
            yield return LoadSave();
        } else {
            Word = Engine.Instance.WordList[Random.Range(0, Engine.Instance.WordList.Length - 1)];
            yield return StartCoroutine(DictionaryAPI.DictionaryFetchService.GetDefinition(Word));
            Hint.text = $"{DictionaryAPI.DictionaryFetchService.FetchedEntry.meanings[0].partOfSpeech}\n\t{DictionaryAPI.DictionaryFetchService.FetchedEntry.meanings[0].definitions[0].definition}";
        }
        AlphaButtons = GetComponentsInChildren<Button>().Where(i => i.name.Length == 1).ToArray();
    }

    private void Update() {
        Engine.Instance.EventConsumed = false;

        if (CanGuess) TTW += Time.deltaTime;

        if (Engine.Instance.Settings.IsHardMode)
            foreach (Button b in AlphaButtons.Where(i => !i.enabled))
                if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), b.GetComponentInChildren<TextMeshProUGUI>().text))) return;

        if (!string.IsNullOrEmpty(Input.inputString))
            if (!Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.Backspace) && !Input.GetKeyDown(KeyCode.Delete))
                InvokeKey(Input.inputString);
            else if (Input.GetKeyDown(KeyCode.Return)) InvokeKey("↪");
            else InvokeKey("↩");

    }

    private void OnApplicationFocus(bool focus) {
        if (!focus) {
            
            string[] Guesses = new string[6];
            for (int i = 0; i < 6; i++) {
                string Guess = "";
                for (int j = 0; j < 5; j++) {
                    Guess += GuessHost.GetComponentsInChildren<HorizontalLayoutGroup>()[i].GetComponentsInChildren<Image>()[j].GetComponentInChildren<TextMeshProUGUI>().text;
                }
                Guesses[i] = Guess;
            }

            Engine.Instance.Settings.AutoSave = new SaveSystem.History() {
                TimeToWin = TTW,
                IsWin = HasWin,
                CorrectAnswer = Word,
                GuessHistory = Guesses.Where(word => word != "" && word.Length == 5).ToArray()
            };
        }
    }

    IEnumerator LoadSave() {
        Word = Engine.Instance.Settings.AutoSave.CorrectAnswer;
        TTW = Engine.Instance.Settings.AutoSave.TimeToWin;

        yield return StartCoroutine(DictionaryAPI.DictionaryFetchService.GetDefinition(Word));
        Hint.text = $"{DictionaryAPI.DictionaryFetchService.FetchedEntry.meanings[0].partOfSpeech}\n\t{DictionaryAPI.DictionaryFetchService.FetchedEntry.meanings[0].definitions[0].definition}";

        for (int i = 0; i < Engine.Instance.Settings.AutoSave.GuessHistory.Length; i++) {
            for (int j = 0; j < 5; j++) {
                InvokeKey(Engine.Instance.Settings.AutoSave.GuessHistory[i][j].ToString());
            }
            InvokeKey("↪");
        }
    }

    public void InvokeKey(string KeyPressed) {

        if (!CanGuess) return;
        GetComponent<AudioSource>().Play();


        char Key = KeyPressed.ToUpper()[0];
        if (LETTERS.Contains(Key)) {
            if (CurCharIndex >= 5) CurCharIndex = 4;
            GuessHost.GetComponentsInChildren<HorizontalLayoutGroup>()[CurGuessCount].GetComponentsInChildren<Image>()[CurCharIndex].GetComponentInChildren<TextMeshProUGUI>().text = Key.ToString();
            CurCharIndex++;
        } else if (ESCAPE_BACK == Key) {
            CurCharIndex--;
            CurCharIndex = CurCharIndex < 0 ? 0 : CurCharIndex;
            GuessHost.GetComponentsInChildren<HorizontalLayoutGroup>()[CurGuessCount].GetComponentsInChildren<Image>()[CurCharIndex].GetComponentInChildren<TextMeshProUGUI>().text = "";
        } else if (ESCAPE_ENTER == Key) {
            Engine.Instance.EventConsumed = true;
            if (CurCharIndex >= 4)
                CheckWord();
            else {
                foreach (Button b in AlphaButtons) 
                    b.enabled = false;
                CanGuess = false;
                int EffectiveIndex = CurCharIndex - 1;
                GameObject.Find("Pop Up").GetComponent<GamePopUp>().ShowError(EffectiveIndex < 0 ? 0 : EffectiveIndex + 1);
            }
        }
    }

    public void CheckWord() {

        string Guess = "";
        for (int i = 0; i < 5; i++) {
            Guess += GuessHost.GetComponentsInChildren<HorizontalLayoutGroup>()[CurGuessCount].GetComponentsInChildren<Image>()[i].GetComponentInChildren<TextMeshProUGUI>().text;
        }

        if (Guess.Length < 5) {
            foreach (Button b in AlphaButtons) { b.enabled = false; }
            CanGuess = false;
            GameObject.Find("Pop Up").GetComponent<GamePopUp>().ShowError(Guess.Length);
            return;
        }

        if (!Engine.Instance.WordList.Contains(Guess.ToLower())) {
            foreach (Button b in AlphaButtons) { b.enabled = false; }
            CanGuess = false;
            GameObject.Find("Pop Up").GetComponent<GamePopUp>().ShowError(Guess);
            return;
        }

        int[] LetterCounts = new int[5] {
            Word.Count(i => i == Word[0]),
             Word.Count(i => i == Word[1]),
             Word.Count(i => i == Word[2]),
             Word.Count(i => i == Word[3]),
             Word.Count(i => i == Word[4])
        };


        for (int i = 0; i < 5; i++) {
            Image CurValue = GuessHost.GetComponentsInChildren<HorizontalLayoutGroup>()[CurGuessCount].GetComponentsInChildren<Image>()[i];
            string CurChar = CurValue.GetComponentInChildren<TextMeshProUGUI>().text;

            if (CurChar[0] == Word.ToUpper()[i])
                CurValue.color = Correct;
            else if (Word.ToUpper().Contains(CurChar[0]) && LetterCounts[i] == Guess.Count(j => j == Guess[i]))
                CurValue.color = WrongPlace;
            else
                CurValue.color = Incorrect;

            Button target = GameObject.Find(CurChar.ToUpper()).GetComponent<Button>();
            target.GetComponent<Image>().color = target.GetComponent<Image>().color == Correct ? Correct : CurValue.color;
            if (Engine.Instance.Settings.IsHardMode) target.enabled = CurValue.color != Incorrect;
        }

        if (Word.ToUpper() == Guess) {
            HasWin = true;
            CanGuess = false;
            GameObject.Find("Pop Up").GetComponent<GamePopUp>().ShowWin();
            PopulateGameSave();
        } else {
            CurGuessCount++;
            CurCharIndex = 0;
            if (CurGuessCount == 6) {
                CanGuess = false;
                GameObject.Find("Pop Up").GetComponent<GamePopUp>().ShowLose();
                PopulateGameSave();
            }
        }

    }

    public void EnableButtons() {
        CanGuess = true;
        foreach(Button b in AlphaButtons) { 
            if (Engine.Instance.Settings.IsHardMode)
                b.enabled = b.GetComponent<Image>().color != Incorrect;
            else
                b.enabled = true; 
        }
    }

    void PopulateGameSave() {

        string[] Guesses = new string[6];
        for (int i = 0; i < 6; i++) {
            string Guess = "";
            for (int j = 0; j < 5; j++) {
                Guess += GuessHost.GetComponentsInChildren<HorizontalLayoutGroup>()[i].GetComponentsInChildren<Image>()[j].GetComponentInChildren<TextMeshProUGUI>().text;
            }
            Guesses[i]= Guess;
        }

        Engine.Instance.Settings.PlayHistory.Add(new SaveSystem.History() {
            TimeToWin = TTW,
            IsWin = HasWin,
            CorrectAnswer = Word,
            GuessHistory = Guesses.Where(word => word != "").ToArray()
        }) ;

        Engine.Instance.Settings.AutoSave = null;
    }
}