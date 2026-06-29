using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    public LevelData[] allLevels;
    public LevelData currentLevel;
    public int currentLevelIndex = 0;
    
    private void OnEnable() {GameEvents.OnRequestLevelStart += StartLevel; GameEvents.OnRequestLevelReset += LevelReset; GameEvents.OnRequestLevelEnd += EndLevel;}
    private void OnDisable() {GameEvents.OnRequestLevelStart -= StartLevel; GameEvents.OnRequestLevelReset -= LevelReset; GameEvents.OnRequestLevelEnd -= EndLevel;}

    private void StartLevel()
    {
        currentLevel = allLevels[currentLevelIndex];
    }

    private void LevelReset(TransitionType transition)
    {
        string sceneName = currentLevel.sceneToLoad.SceneName;
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
