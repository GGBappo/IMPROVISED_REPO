using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    public LevelData[] allLevels;
    public LevelData currentLevel;
    public int currentLevelIndex = 0;
    private bool isReady = false; // for wakeup when getting called by game manager
    
    private void OnEnable() {GameEvents.OnRequestLevelReset += LevelReset;}
    private void OnDisable() {GameEvents.OnRequestLevelReset -= LevelReset;}

    // okay some of my thought process is cooking something up by splitting strings
    private void LevelReset(TransitionType transition)
    {
        Debug.Log($"[LevelManager] Resetting level: {currentLevel.levelName} with transition: {transition}");
        GameEvents.RequestSceneUnLoad(currentLevel.levelLocation);
        GameEvents.RequestSceneLoad(currentLevel.levelLocation, transition, true);
    } 
}
