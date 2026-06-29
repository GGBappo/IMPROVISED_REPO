using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    private void OnEnable() => GameEvents.OnTimerExpired += HandleTimeExpiration;
    private void OnDisable() => GameEvents.OnTimerExpired -= HandleTimeExpiration;
    
    private void Awake()
    {
        GameEvents.RequestSceneLoad(sceneName:"StartMenu", setActive:true);
    }

    private void HandleTimeExpiration()
    {
        GameSessionData.lostOnTime = true;
        GameEvents.StateChanged(GlobalStateType.Outcome);
    }
}
