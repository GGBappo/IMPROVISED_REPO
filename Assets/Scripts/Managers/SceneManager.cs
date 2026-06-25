using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOperator : MonoBehaviour
{
    // i still need to use these pls dont abuse me for this :(
    private Scene _activeScene;
    private Scene _currentScene; // different from actiive scene, as this is the scene we want to load, but it may not be active yet

    private void OnEnable() {GameEvents.OnRequestSceneLoad += Load; GameEvents.OnRequestSceneUnLoad += Unload;}
    private void OnDisable() {GameEvents.OnRequestSceneLoad -= Load; GameEvents.OnRequestSceneUnLoad -= Unload;}


    /// <summary>
    /// Loads a scene additively with an optional transition and sets it as the active scene if specified.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="transition"></param>
    /// <param name="setActive"></param>
    private void Load(string sceneName, TransitionType transition = TransitionType.None, bool setActive = true)
    {
        StartCoroutine(LoadCoroutine(sceneName, transition));
    }
    
    /// <summary>
    /// Unloads a scene asynchronously.
    /// </summary>
    /// <param name="sceneName"></param>
    public void Unload(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }
    
    private IEnumerator LoadCoroutine(string sceneName, TransitionType transition)
    {
        if (transition == TransitionType.None)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while(!operation.isDone)
            {
                Debug.Log("Loading progress: " + operation.progress);
                yield return null;
            }
            Debug.Log("Scene loaded: " + sceneName);
        }
        else
        {
            GameEvents.RequestTransitionIN(transition);


            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!operation.isDone)
            {
                Debug.Log("Loading progress: " + operation.progress + " for scene: " + sceneName);
                yield return null;
            }
            Debug.Log("Scene loaded: " + sceneName);
            GameEvents.RequestTransitionOUT(transition);
        }
        
    }
    
}
