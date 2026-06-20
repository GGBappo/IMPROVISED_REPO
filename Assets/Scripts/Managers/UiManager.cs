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
        _transitions.Add(TransitionType.Fade, new Fade());
    }

    private void OnEnable() {GameEvents.OnTransitionINRequested += HandleTransitionIn; GameEvents.OnTransitionOUTRequested += HandleTransitionOut;}
    private void OnDisable() {GameEvents.OnTransitionINRequested -= HandleTransitionIn; GameEvents.OnTransitionOUTRequested -= HandleTransitionOut;}

    private void HandleTransitionIn(TransitionType type, Action onComplete)
    {
        if (_transitions.TryGetValue(type, out ITransition activeTransition))
            activeTransition.TransitionIN(_loadingScreen, _globalTransitionSpeed, onComplete);
    }

    private void HandleTransitionOut(TransitionType type, Action onComplete)
    {
        if (_transitions.TryGetValue(type, out ITransition activeTransition))
            activeTransition.TransitionOUT(_loadingScreen, _globalTransitionSpeed, onComplete);
    }
}