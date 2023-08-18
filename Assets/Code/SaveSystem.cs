using System;
using System.Collections.Generic;

#if !UNITY_WEBGL
using System.IO;
#endif

using UnityEngine;

namespace SaveSystem {
    public static class SaveSystem {

#if !UNITY_WEBGL
        const string SAVELOCATION = "file0";

        public static void Save(Settings SettingsToSave) {
            File.WriteAllText(Path.Combine(Application.persistentDataPath, SAVELOCATION), Secrets.EngineSecrets.ResolveCrypto(JsonUtility.ToJson(SettingsToSave)));
            Debug.Log("Save written to disk.");
        }

        public static Settings Load() {
            if (File.Exists(Path.Combine(Application.persistentDataPath, SAVELOCATION))) {
                Debug.Log("Save found. Loading save...");
                return JsonUtility.FromJson<Settings>(Secrets.EngineSecrets.ResolveCrypto(File.ReadAllText(Path.Combine(Application.persistentDataPath, SAVELOCATION))));
            } else {
                Debug.Log("No save exists, making new save...");
                return new Settings(); 
            }
        }
#else
        public static void Save(Settings SettingsToSave) {
            PlayerPrefs.SetString("SaveData", Secrets.EngineSecrets.Encrypt(JsonUtility.ToJson(SettingsToSave)));
            PlayerPrefs.Save();
            Debug.Log("Save written to disk.");
        }

        public static Settings Load() {
            
            if (PlayerPrefs.HasKey("SaveData")) {
                Debug.Log("Save found. Loading save...");
                return JsonUtility.FromJson<Settings>(Secrets.EngineSecrets.Decrypt(PlayerPrefs.GetString("SaveData")));
            } else {
                Debug.Log("No save exists, making new save...");
                return new Settings();
            }
        }
#endif 

    }

    [Serializable]
    public class Settings {
        public bool IsDarkMode = false;
        public bool IsMobileMode = false;
        public bool IsHardMode = false;
        public List<History> PlayHistory = new List<History>();
        public History AutoSave;

        public bool HasUnfinishedGame { 
            get {
                if (AutoSave == null || AutoSave.NumMoves == 0) return false;
                return !AutoSave.IsWin && AutoSave.NumMoves != 6;
            }
        }
    }

    [Serializable]
    public class History {
        public int NumMoves { get { return GuessHistory.Length; } }
        public bool IsWin;
        public string CorrectAnswer = "";
        public string[] GuessHistory = new string[0];
        public float TimeToWin;
    }
}
