using UnityEngine;

public class Engine : Singleton<Engine> {
    public string[] WordList;
    public SaveSystem.Settings Settings;

    public bool EventConsumed = false;

    private new void Awake() {
        base.Awake();
        Settings = SaveSystem.SaveSystem.Load();
    }

    private void OnApplicationFocus(bool focus) {
        if (!focus) SaveSystem.SaveSystem.Save(Settings);
    }

    private void OnApplicationPause(bool pause) {
        if (pause) SaveSystem.SaveSystem.Save(Settings);
    }

    private void OnApplicationQuit() {
        SaveSystem.SaveSystem.Save(Settings);
    }
}

public class Singleton<T> :MonoBehaviour where T : MonoBehaviour {

    public static T Instance { get; private set; }

    protected void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        } else {
            Instance = this as T;
            DontDestroyOnLoad(this);
        }
    }

}