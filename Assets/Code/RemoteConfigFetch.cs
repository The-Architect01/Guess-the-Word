using System.Collections;

using Secrets;

using UnityEngine;

public class RemoteConfigFetch : MonoBehaviour {

#if !UNITY_WEBGL
    async Task Start() {
        Player Player = await EngineSecrets.Authenticate();
        await EngineSecrets.RemoteConfig(Player);
    }
#else
    IEnumerator Start() {
        yield return StartCoroutine(EngineSecrets.Authenticate());
        StartCoroutine(EngineSecrets.RemoteConfig(EngineSecrets.LoggedInPlayer));
    }
#endif
}
