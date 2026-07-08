using UnityEngine;
using System;
using System.Collections.Generic;



public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup _loadingScreen;
    [SerializeField] private CanvasGroup _gameOverScreen;
    [SerializeField] private CanvasGroup _pauseScreen;
    [SerializeField] private float _globalTransitionSpeed = 1f;

    private readonly Dictionary<TransitionType, ITransition> _transitions = new Dictionary<TransitionType, ITransition>();

    private void Awake()
    {
        HideGameOverScreen();
        _transitions.Add(TransitionType.Fade, new Fade());
    }

    private void OnEnable() {
        GameEvents.OnTransitionINRequested += HandleTransitionIn; 
        GameEvents.OnTransitionOUTRequested += HandleTransitionOut;
        GameEvents.OnShowGameOverRequested += ShowGameOverScreen;
        GameEvents.OnHideGameOverRequested += HideGameOverScreen;
    }

    private void OnDisable() {
        GameEvents.OnTransitionINRequested -= HandleTransitionIn; 
        GameEvents.OnTransitionOUTRequested -= HandleTransitionOut;
        GameEvents.OnShowGameOverRequested -= ShowGameOverScreen;
        GameEvents.OnHideGameOverRequested -= HideGameOverScreen;
    }

    private void HandleTransitionIn(TransitionType type, Action onComplete)
    {
        if (_transitions.TryGetValue(type, out ITransition activeTransition))
            activeTransition.TransitionIN(canvas: _loadingScreen, speed: _globalTransitionSpeed, onComplete: onComplete);
    }

    private void HandleTransitionOut(TransitionType type, Action onComplete)
    {
        if (_transitions.TryGetValue(type, out ITransition activeTransition))
            activeTransition.TransitionOUT(canvas: _loadingScreen, speed: _globalTransitionSpeed, onComplete: onComplete);
    }

    private void ShowGameOverScreen()
    {
        _gameOverScreen.alpha = 1f;
        _gameOverScreen.interactable = true;
        _gameOverScreen.blocksRaycasts = true;
        Debug.Log("[UIManager] Showing Game Over Screen");
    }

    private void HideGameOverScreen()
    {
        _gameOverScreen.alpha = 0f;
        _gameOverScreen.interactable = false;
        _gameOverScreen.blocksRaycasts = false;
        Debug.Log("[UIManager] Hiding Game Over Screen");
    }

    private void ShowPauseScreen()
    {
        _pauseScreen.alpha = 1f;
        _pauseScreen.interactable = true;
        _pauseScreen.blocksRaycasts = true;
        Debug.Log("[UIManager] Showing Pause Screen");
    }
    private void HidePauseScreen()
    {
        _pauseScreen.alpha = 0f;
        _pauseScreen.interactable = false;
        _pauseScreen.blocksRaycasts = false;
        Debug.Log("[UIManager] Hiding Pause Screen");
    }
}