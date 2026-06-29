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
    /// Loads a scene additively with an optional transition. By default, the loaded scene will be set as the active scene.
    /// </summary>
    /// <param name="sceneName"> The name of the scene to load. </param>
    /// <param name="transition"> The type of transition to use. </param>
    /// <param name="setActive"> Whether to set the loaded scene as the active scene. </param>
    private void Load(string sceneName, TransitionType transition = TransitionType.None, bool setActive = true)
    {
        StartCoroutine(LoadCoroutine(sceneName, transition, setActive));
    }
    
    /// <summary>
    /// Unloads a scene asynchronously.
    /// </summary>
    /// <param name="sceneName"> The name of the scene to unload. </param>
    public void Unload(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }
    
    private IEnumerator LoadCoroutine(string sceneName, TransitionType transition, bool setActive = true)
    {
        if (transition == TransitionType.None)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while(!operation.isDone)
            {
                Debug.Log("Loading progress: " + operation.progress);
                yield return null;
            }

            if (setActive)
            {
                _currentScene = SceneManager.GetSceneByName(sceneName);
                SceneManager.SetActiveScene(_currentScene);
                _activeScene = SceneManager.GetActiveScene();
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

            if (setActive)
            {
                _currentScene = SceneManager.GetSceneByName(sceneName);
                SceneManager.SetActiveScene(_currentScene);
                _activeScene = SceneManager.GetActiveScene();
            }

            Debug.Log("Scene loaded: " + sceneName);

            GameEvents.RequestTransitionOUT(transition);
        }
    }
}
