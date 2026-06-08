using UnityEngine;
using System.Collections;

public class Fade : Transition
{
    public TransitionState currentState { get; private set; }

    public override void TransitionIN()
    {
        StartCoroutine(FadeIn());
    }
    public override void TransitionOUT()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
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
        yield return null;
    }

    private IEnumerator FadeOut()
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
        yield return null;
    }
}