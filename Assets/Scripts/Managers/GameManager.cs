using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string _sceneToStartOn = "StartMenu";
    private void OnEnable() => GameEvents.OnTimerExpired += HandleTimeExpiration;
    private void OnDisable() => GameEvents.OnTimerExpired -= HandleTimeExpiration;
    
    private void Awake()
    {
        GameEvents.RequestSceneLoad(sceneName:_sceneToStartOn, setActive:true);
    }

    private void HandleTimeExpiration()
    {
        GameSessionData.lostOnTime = true;
        GameEvents.GlobalStateChanged(GlobalStateType.Outcome);
    }
}
