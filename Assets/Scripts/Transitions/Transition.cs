using UnityEngine;

public class Transition : MonoBehaviour
{
    public float transitionSpeed = 1f;
    public CanvasGroup loadingScreen;

    public enum TransitionState { transitioning, transitioned }

    public bool isComplete { get; protected set; }
    public virtual void TransitionIN() {}
    public virtual void TransitionOUT() {}
}