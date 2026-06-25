using UnityEngine;
using System;
using DG.Tweening; 

public class Fade : ITransition
{
    public void TransitionIN(CanvasGroup canvas, float speed, Action onComplete)
    {
        canvas.alpha = 0f; 
        
        canvas.DOFade(1f, speed).OnComplete(() => onComplete?.Invoke());
    }

    public void TransitionOUT(CanvasGroup canvas, float speed, Action onComplete)
    {
        canvas.alpha = 1f;
        canvas.DOFade(0f, speed).OnComplete(() => onComplete?.Invoke());
    }
}