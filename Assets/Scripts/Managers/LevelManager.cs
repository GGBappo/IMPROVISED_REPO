using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    public LevelData[] allLevels;
    public LevelData currentLevel;
    public int currentLevelIndex = 0;
    
    private void OnEnable() {
        GameEvents.OnRequestLevelStart += StartLevel; 
        GameEvents.OnRequestLevelReset += LevelReset; 
        GameEvents.OnRequestLevelEnd += EndLevel;
        GameEvents.OnSceneFullyLoaded += InitializeLevel;
    }
    private void OnDisable() {
        GameEvents.OnRequestLevelStart -= StartLevel; 
        GameEvents.OnRequestLevelReset -= LevelReset; 
        GameEvents.OnRequestLevelEnd -= EndLevel;
        GameEvents.OnSceneFullyLoaded -= InitializeLevel;
    }

    private void StartLevel()
    {
        currentLevel = allLevels[currentLevelIndex];
        GameEvents.RequestSceneLoad(currentLevel.sceneToLoad.SceneName, TransitionType.Fade);
    }

    private void InitializeLevel(string loadedSceneName)
    {
        if (currentLevel != null && loadedSceneName == currentLevel.sceneToLoad.SceneName)
        {
            GameObject cameraMarker = GameObject.FindGameObjectWithTag("CameraMarker");
            
            if (cameraMarker != null)
            {
                GameEvents.RequestCameraMove(cameraMarker.transform.position, cameraMarker.transform.rotation, 0f);
                GameEvents.RequestCameraFOVChange(5f);
            }
        }
    }

    private void LevelReset(TransitionType transition)
    {
        string sceneName = currentLevel.sceneToLoad.SceneName;
        Debug.Log($"[Level Manager] Resetting level: {sceneName}");
        GameEvents.RequestSceneUnLoad(sceneName);
        GameEvents.RequestSceneLoad(sceneName, TransitionType.Fade);
        GameEvents.StateChanged(GlobalStateType.Active);
    } 

    private void EndLevel()
    {
        currentLevelIndex = 0;
        GameEvents.RequestSceneUnLoad(currentLevel.sceneToLoad.SceneName);
        currentLevel = null;
        GameEvents.RequestSceneLoad("StartMenu", TransitionType.Fade);
        GameEvents.StateChanged(GlobalStateType.Active);
    }
}
