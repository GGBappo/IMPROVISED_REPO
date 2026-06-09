using UnityEngine;
using System.Collections.Generic;
using System;

public enum TransitionType
{
    None,
    Fade
}

public class UIManager : MonoBehaviour
{
    private Dictionary<TransitionType, Transition> _transitions = new Dictionary<TransitionType, Transition>();
    [SerializeField] private Fade _fadeScript;

    public void Setup()
    {
        _transitions.Add(TransitionType.Fade, _fadeScript);
    }

    public void PlayTransitionIN(TransitionType transition, Action onComplete = null)
    {
        if (transition == TransitionType.None)
        {
            onComplete?.Invoke();
            return;
        }
        if (_transitions.TryGetValue(transition, out Transition transitionScript))
        {
            transitionScript.TransitionIN();
        }
        else
        {
            Debug.LogWarning("Transition not found: " + transition);
        }
    }
    public void PlayTransitionOUT(TransitionType transition, Action onComplete = null)
    {
        if (transition == TransitionType.None)
        {
            onComplete?.Invoke();
            return;
        }
        if (_transitions.TryGetValue(transition, out Transition transitionScript))
        {
            transitionScript.TransitionOUT();
        }
        else
        {
            Debug.LogWarning("Transition not found: " + transition);
        }
    } 
}
