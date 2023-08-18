using SaveSystem;

using UnityEngine;

public class UIEngine : MonoBehaviour {

    [SerializeField] GameObject Stats;
    [SerializeField] GameObject StatPrefab;
    [SerializeField] Transform StatHost;
    [SerializeField] GameObject Credits;

    public void HideCredits() { Credits.SetActive(false); }
    public void HideStats() { 
        foreach (LoadHistoryUI go in StatHost.GetComponentsInChildren<LoadHistoryUI>()) Destroy(go.gameObject);
        Stats.SetActive(false); 
    }
    public void ShowCredits() { Credits.SetActive(true); }
    public void ShowStats() {
        Stats.SetActive(true);
        foreach (History h in Engine.Instance.Settings.PlayHistory) {
            if (h == Engine.Instance.Settings.AutoSave) continue;
            GameObject go = Instantiate(StatPrefab, StatHost);
            go.GetComponent<LoadHistoryUI>().Populate(h);
        }
    }
    public void OnStart() { UnityEngine.SceneManagement.SceneManager.LoadScene(1); }

}
