using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    private void OnEnable() => GameEvents.OnTimerExpired += HandleGameOver;
    private void OnDisable() => GameEvents.OnTimerExpired -= HandleGameOver;
    
    private void Awake()
    {
        GameEvents.RequestSceneLoad("StartMenu");
    }

    private void HandleGameOver()
    {
        GameEvents.StateChanged(GlobalStateType.Outcome);
    }
}
