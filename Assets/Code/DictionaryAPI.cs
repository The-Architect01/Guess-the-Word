using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Networking;

namespace DictionaryAPI {

    [Serializable]
    public class DictionaryEntryWrapper {
        public DictionaryEntry[] Entries;
    }

    [Serializable]
    public class DictionaryEntry {

        public string word;
        public string phonetic;
        public Phonetic phonetics;
        public string origin;
        public Meaning[] meanings;
        public License license;
        public string[] sourceUrls;
    }

    [Serializable]
    public class Phonetic {
        public string text;
        public string audio;
        public string sourceUrl;
        public License license;
    }

    [Serializable]
    public class Meaning {
        public string partOfSpeech;
        public DefinitionEntry[] definitions;
    }

    [Serializable]
    public class License {
        public string name;
        public string url;
    }


    [Serializable]
    public class DefinitionEntry {
        public string definition;
        public string example;
        public string[] synonyms;
        public string[] antonyms;
    }

    public class DictionaryFetchService : MonoBehaviour {

        public static DictionaryEntry FetchedEntry;
        public static IEnumerator GetDefinition(string Word) {
            using (UnityWebRequest HTTPHandler = UnityWebRequest.Get($"https://api.dictionaryapi.dev/api/v2/entries/en/{Word}")) {
                yield return HTTPHandler.SendWebRequest();

                if (HTTPHandler.result == UnityWebRequest.Result.Success) {
                    string JSONFormat = $"{{\"Entries\": {HTTPHandler.downloadHandler.text}}}";
                    JsonUtility.FromJson<DictionaryEntryWrapper>(JSONFormat);
                    FetchedEntry = JsonUtility.FromJson<DictionaryEntryWrapper>(JSONFormat).Entries[0];
                } else {
                    throw new Exception("Can't retrieve definition!");
                }
            }
        }
    }
}