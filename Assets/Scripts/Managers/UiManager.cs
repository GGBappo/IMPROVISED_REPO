using UnityEngine;
using System;
using System.Collections.Generic;



public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup _loadingScreen;
    [SerializeField] private float _globalTransitionSpeed = 1f;

    private readonly Dictionary<TransitionType, ITransition> _transitions = new Dictionary<TransitionType, ITransition>();

    private void Awake()
    {
        _transitions.Add(TransitionType.Fade, new Fade());
    }

    private void OnEnable() => GameEvents.OnTransitionRequested += HandleTransition;
    private void OnDisable() => GameEvents.OnTransitionRequested -= HandleTransition;

    private void HandleTransition(TransitionType type, Action onComplete)
    {
        if (type == TransitionType.None)
        {
            onComplete?.Invoke();
            return;
        }

        if (_transitions.TryGetValue(type, out ITransition activeTransition))
        {
            // Just trigger it! DOTween handles all the animation perfectly in the background.
            activeTransition.TransitionIN(_loadingScreen, _globalTransitionSpeed, onComplete); 
        }
        else
        {
            Debug.LogWarning($"Transition {type} not found!");
        }
    }
}