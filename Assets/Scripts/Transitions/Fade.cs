using UnityEngine;
using System.Collections;
using System;

public class Fade : Transition
{
    public TransitionState currentState { get; private set; }

    public override void TransitionIN(Action onComplete = null)
    {
        StartCoroutine(FadeIn(onComplete));
    }
    public override void TransitionOUT(Action onComplete = null)
    {
        StartCoroutine(FadeOut(onComplete));
    }

    private IEnumerator FadeIn(Action onComplete = null)
    {
        loadingScreen.alpha = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < transitionSpeed)
        {
            loadingScreen.alpha = 0f + (elapsedTime / transitionSpeed); // increase alpha over time
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        loadingScreen.alpha = 1f; 
        onComplete?.Invoke();
        yield return null;
    }

    private IEnumerator FadeOut(Action onComplete = null)
    {
        loadingScreen.alpha = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < transitionSpeed)
        {
            loadingScreen.alpha = 1f - (elapsedTime / transitionSpeed); // decrease alpha over time
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        loadingScreen.alpha = 0f; 
        onComplete?.Invoke();
        yield return null;
    }
}