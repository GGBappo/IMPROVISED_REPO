using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void OnEnable() => GameEvents.OnTimerExpired += HandleGameOver;
    private void OnDisable() => GameEvents.OnTimerExpired -= HandleGameOver;
    
    private void HandleGameOver()
    {
        Debug.Log("timer expired, game over");
    }
}
