using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    public LevelDatabase levelDatabase;
    public LevelData currentLevel;
    private int currentLevelIndex = 0;
    
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
        GameSessionData.won = false;
        GameSessionData.lostOnTime = false;
        GameSessionData.lostOnStrikes = false;
        currentLevel = levelDatabase.allLevels[currentLevelIndex];
        GameEvents.RequestSceneLoad(currentLevel.sceneToLoad.SceneName, TransitionType.Fade);
    }

    private void InitializeLevel(string loadedSceneName)
    {
        if (currentLevel != null && loadedSceneName == currentLevel.sceneToLoad.SceneName)
        {
            GameObject cameraMarker = GameObject.FindGameObjectWithTag("CameraMarker");
            
            if (cameraMarker != null)
            {
                GameEvents.RequestCameraMove(cameraMarker.transform.position, cameraMarker.transform.rotation, 0f, FOV: 50f);
            }
        }
    }

    private void LevelReset(TransitionType transition)
    {
        string sceneName = currentLevel.sceneToLoad.SceneName;
        Debug.Log($"[Level Manager] Resetting level: {sceneName}");
        GameEvents.RequestSceneUnLoad(sceneName);
        GameEvents.RequestSceneLoad(sceneName, TransitionType.Fade);
        GameEvents.GlobalStateChanged(GlobalStateType.Active);
    } 

    private void EndLevel()
    {
        GameEvents.RequestSceneUnLoad(currentLevel.sceneToLoad.SceneName);
        currentLevelIndex++;
        currentLevel = null;
        GameEvents.RequestSceneLoad("StartMenu", TransitionType.Fade);
        GameEvents.GlobalStateChanged(GlobalStateType.Active);
    }
}
