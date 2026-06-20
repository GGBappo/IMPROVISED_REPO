using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<TransitionType, Action> OnTransitionINRequested; 
    public static event Action<TransitionType, Action> OnTransitionOUTRequested;    

    // timer events
    public static event Action<int, float> OnStrikeOccurred; // (strikeCount, penaltyApplied)
    public static event Action OnTimerExpired;

    // state events
    public static event Action<GlobalStateType> OnGlobalStateChanged; // (newState)

    // scene events
    public static event Action<string, TransitionType> OnRequestSceneLoad;
    public static event Action<string> OnRequestSceneUnLoad;

    /// <summary>
    /// Invokes the <see cref="OnTransitionINRequested"/> event to notify subscribers that a scene transition has been requested. Should be used in conjunction with the <see cref="RequestSceneLoad"/> and <see cref="RequestSceneUnLoad"/> events to trigger visual transitions when loading/unloading scenes.
    /// In addition, it should ALWAYS be followed by <see cref="RequestTransitionOUT"/> in order to complete the transition.
    /// For a full list of available transitions, see: <see cref="TransitionType"/>.
    /// </summary>
    /// <param name="transition">The visual style of the transition.</param>
    /// <param name="onComplete">Optional callback executed the exact frame the transition animation finishes.</param>
    public static void RequestTransitionIN(TransitionType transition, Action onComplete = null)
    {
        Debug.Log($"Requesting transition IN of type: {transition}");
        OnTransitionINRequested?.Invoke(transition, onComplete);
    }

    public static void RequestTransitionOUT(TransitionType transition, Action onComplete = null)
    {
        Debug.Log($"Requesting transition OUT of type: {transition}");
        OnTransitionOUTRequested?.Invoke(transition, onComplete);
    }

    /// <summary>
    /// Invoke the OnStrikeOccurred event to notify subscribers that a strike has occurred, along with the current strike count and penalty applied.
    /// </summary>
    /// <param name="strikeCount"></param>
    /// <param name="penalty"></param>
    public static void StrikeOccurred(int strikeCount, float penalty)
    {
        OnStrikeOccurred?.Invoke(strikeCount, penalty);
    }

    /// <summary>
    /// Invoke the OnTimerExpired event to notify subscribers that the timer has expired.
    /// </summary>
    public static void TimerExpired()
    {
        OnTimerExpired?.Invoke();
    }

    /// <summary>
    /// Invoke the OnGlobalStateChanged event to notify the state manager that the global state has changed
    /// </summary>
    /// <param name="newState"></param>
    public static void StateChanged(GlobalStateType newState)
    {
        OnGlobalStateChanged?.Invoke(newState);
    }

    /// <summary>
    /// Requests a scene to be loaded. Idealy, this should be used alongside <see cref="RequestSceneUnLoad"/> in order to unload scenes.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="transition"></param>
    public static void RequestSceneLoad(string sceneName, TransitionType transition = TransitionType.None)
    {
        Debug.Log($"Requesting load of scene: {sceneName} with transition: {transition}");
        OnRequestSceneLoad?.Invoke(sceneName, transition);
    }

    /// <summary>
    /// Requests a scene to be unloaded. Idealy, this should be used alongside <see cref="RequestSceneLoad"/> in order to load scenes.
    /// </summary>
    /// <param name="sceneName"></param>
    public static void RequestSceneUnLoad(string sceneName)
    {
        Debug.Log($"Requesting unload of scene: {sceneName}");
        OnRequestSceneUnLoad?.Invoke(sceneName);
    }
}