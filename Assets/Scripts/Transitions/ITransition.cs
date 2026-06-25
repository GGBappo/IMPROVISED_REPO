using System;
using UnityEngine;

public interface ITransition
{
    void TransitionIN(CanvasGroup canvas, float speed, Action onComplete);
    void TransitionOUT(CanvasGroup canvas, float speed, Action onComplete);
}