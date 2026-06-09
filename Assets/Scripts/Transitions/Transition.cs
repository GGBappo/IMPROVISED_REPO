using UnityEngine;
using System;

public class Transition : MonoBehaviour
{
    public float transitionSpeed = 1f;
    public CanvasGroup loadingScreen;

    public enum TransitionState { transitioning, transitioned }

    public bool isComplete { get; protected set; }
    public virtual void TransitionIN(Action onComplete = null) {}
    public virtual void TransitionOUT(Action onComplete = null) {}
}