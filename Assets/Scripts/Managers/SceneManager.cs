using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOperator : MonoBehaviour
{
    // i still need to use these pls dont abuse me for this :(
    private Scene _activeScene;
    private Scene _currentScene; // different from actiive scene, as this is the scene we want to load, but it may not be active yet
    
    public void Setup()
    {
        // unsure what to put here yet as initializing
    }
    
    public void Load(string sceneName, TransitionType transition = TransitionType.None, bool setActive = true)
    {
        StartCoroutine(LoadCoroutine(sceneName, transition, setActive));
    }
  
    
    public void Unload(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }
    
    private IEnumerator LoadCoroutine(string sceneName, TransitionType transition, bool setActive)
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
            if (setActive)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
                Debug.Log("Current Active Scene: " + SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            bool transitionComplete = false;
            GameManager.Instance.PlayTransitionIN(transition, () => { transitionComplete = true; });

            yield return new WaitUntil(() => transitionComplete);

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!operation.isDone)
            {
                Debug.Log("Loading progress: " + operation.progress + " for scene: " + sceneName);
                yield return null;
            }
        }
        
    }
    
}
