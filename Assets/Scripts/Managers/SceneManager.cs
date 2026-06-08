using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOperator : MonoBehaviour
{
    private Scene ActiveScene;
    private Scene CurrentScene; // different from actiive scene, as this is the scene we want to load, but it may not be active yet
    public void Setup()
    {
        Load("SESSION");
    }

    ///// load functions /////
    public void Load(string sceneName)
    {
        StartCoroutine(LoadWithTransition(sceneName));
    }
    public void Load(string sceneName, bool loadWithTransition)
    {
        if (loadWithTransition)
        {
            StartCoroutine(LoadWithTransition(sceneName));
        }
        else
        {
            StartCoroutine(TransitionlessLoad(sceneName));
        }
    }
    /////////////////////////
    
    ////// unload functions ////// 
    public void Unload(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }
    //////////////////////////////
    
    
    private IEnumerator TransitionlessLoad(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while(!operation.isDone)
        {
            Debug.Log("Loading progress: " + operation.progress);
            yield return null;
        }
        Debug.Log("Scene loaded: " + sceneName);
        Debug.Log("Current Active Scene: " + SceneManager.GetActiveScene().name);
    }
    private IEnumerator LoadWithTransition(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while(!operation.isDone)
        {
            Debug.Log("Loading progress: " + operation.progress);
            yield return null;
        }
        Debug.Log("Scene loaded: " + sceneName);
        Debug.Log("Current Active Scene: " + SceneManager.GetActiveScene().name);
        yield return null;
        
    }
}
