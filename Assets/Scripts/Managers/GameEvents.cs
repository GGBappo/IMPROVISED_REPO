using System;
using UnityEngine;

public static class GameEvents
{
    // timer/strike events
    public static event Action<int, float> OnStrikeOccurred; // (strikeCount, penaltyApplied)
    public static event Action OnTimerExpired;

    // state events
    public static event Action<GlobalStateType> OnGlobalStateChanged; // (newState)

    // scene/level events
    public static event Action<string, TransitionType, bool> OnRequestSceneLoad;
    public static event Action<string> OnRequestSceneUnLoad;
    public static event Action<TransitionType> OnRequestLevelReset;
    public static event Action OnRequestLevelEnd;
    public static event Action OnRequestLevelStart;
    public static event Action<string> OnSceneFullyLoaded; // (sceneName)

    /// UI events
    public static event Action OnShowGameOverRequested;
    public static event Action OnHideGameOverRequested;
    public static event Action<TransitionType, Action> OnTransitionINRequested; 
    public static event Action<TransitionType, Action> OnTransitionOUTRequested; 

    // camera events
    public static event Action<Vector3, Quaternion, float> OnCameraMoveRequest; // (position, rotation, duration)
    public static event Action<Vector3, float> OnCameraLookAtRequest; // (targetPosition, duration)
    public static event Action<GameObject, float> OnCameraLookAtGameObjectRequest; // (targetGameObject, duration)
    public static event Action<float> OnCameraFOVChangeRequest; // (newFOV)

    #region Timer/Strike Calls
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
    #endregion

    #region State Calls
    /// <summary>
    /// Invoke the OnGlobalStateChanged event to notify the state manager that the global state has changed
    /// </summary>
    /// <param name="newState"></param>
    public static void StateChanged(GlobalStateType newState)
    {
        OnGlobalStateChanged?.Invoke(newState);
    }
    #endregion

    #region Scene/Level Calls
    /// <summary>
    /// Requests a scene to be loaded. Idealy, this should be used alongside <see cref="RequestSceneUnLoad"/> in order to unload scenes.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="transition"></param>
    /// <param name="setActive"></param>
    public static void RequestSceneLoad(string sceneName, TransitionType transition = TransitionType.None, bool setActive = true)
    {
        Debug.Log($"[GameEvents] Requesting load of scene: {sceneName} with transition: {transition}");
        OnRequestSceneLoad?.Invoke(sceneName, transition, setActive);
    }

    /// <summary>
    /// Requests a scene to be unloaded. Idealy, this should be used alongside <see cref="RequestSceneLoad"/> in order to load scenes.
    /// </summary>
    /// <param name="sceneName"></param>
    public static void RequestSceneUnLoad(string sceneName)
    {
        Debug.Log($"[GameEvents] Requesting unload of scene: {sceneName}");
        OnRequestSceneUnLoad?.Invoke(sceneName);
    }

    /// <summary>
    /// Requests the start of the current level. This should be used to notify subscribers that the current level has started and any necessary setup should be performed.
    /// </summary>
    public static void RequestLevelStart()
    {
        Debug.Log($"[GameEvents] Requesting start of current level");
        OnRequestLevelStart?.Invoke();
    }

    /// <summary>
    /// Requests a level reset. This should be used to reset the current level.
    /// </summary>
    /// <param name="transition"></param>
    public static void RequestLevelReset(TransitionType transition)
    {
        Debug.Log($"[GameEvents] Requesting reset of current level with transition: {transition}");
        OnRequestLevelReset?.Invoke(transition);
    }

    /// <summary>
    /// Requests the end of the current level. This should be used to notify subscribers that the current level has ended and any necessary cleanup should be performed.
    /// <br>Note:</br> "End" here means after the level has been completed no matter the outcome. It is to quite literally END the level, meaning unload.
    /// </summary>
    public static void RequestEndLevel()
    {
        Debug.Log($"[GameEvents] Requesting end of current level");
        OnRequestLevelEnd?.Invoke();
    }

    public static void SceneFullyLoaded(string sceneName)
    {
        Debug.Log($"[GameEvents] Scene fully loaded: {sceneName}");
        OnSceneFullyLoaded?.Invoke(sceneName);
    }
    #endregion

    #region UI Calls
    /// <summary>
    /// Requests the game over screen to be shown.
    /// </summary>
    public static void RequestShowGameOverScreen()
    {
        Debug.Log("[GameEvents] Requesting to show Game Over Screen");
        OnShowGameOverRequested?.Invoke();
        Debug.Log("[GameEvents] OnShowGameOverRequested invoked");
    }
    
    /// <summary>
    /// Requests the game over screen to be hidden.
    /// </summary>
    public static void RequestHideGameOverScreen()
    {
        Debug.Log("[GameEvents] Requesting to hide Game Over Screen");
        OnHideGameOverRequested?.Invoke();
        Debug.Log("[GameEvents] OnHideGameOverRequested invoked");
    }

    /// <summary>
    /// Invokes the <see cref="OnTransitionINRequested"/> event to notify subscribers that a scene transition has been requested. Should be used in conjunction with the <see cref="RequestSceneLoad"/> and <see cref="RequestSceneUnLoad"/> events to trigger visual transitions when loading/unloading scenes.
    /// In addition, it should ALWAYS be followed by <see cref="RequestTransitionOUT"/> in order to complete the transition.
    /// For a full list of available transitions, see: <see cref="TransitionType"/>.
    /// </summary>
    /// <param name="transition">The visual style of the transition.</param>
    /// <param name="onComplete">Optional callback executed the exact frame the transition animation finishes.</param>
    public static void RequestTransitionIN(TransitionType transition, Action onComplete = null)
    {
        Debug.Log($"[GameEvents] Requesting transition IN of type: {transition}");
        OnTransitionINRequested?.Invoke(transition, onComplete);
    }

    public static void RequestTransitionOUT(TransitionType transition, Action onComplete = null)
    {
        Debug.Log($"[GameEvents] Requesting transition OUT of type: {transition}");
        OnTransitionOUTRequested?.Invoke(transition, onComplete);
    }
    #endregion

    #region Camera Calls

    public static void RequestCameraMove(Vector3 position, Quaternion rotation, float duration)
    {
        Debug.Log($"[GameEvents] Requesting camera move to position: {position}, rotation: {rotation}, duration: {duration}");
        OnCameraMoveRequest?.Invoke(position, rotation, duration);
    }

    public static void RequestCameraLookAt(Vector3 targetPosition, float duration)
    {
        Debug.Log($"[GameEvents] Requesting camera to look at position: {targetPosition}, duration: {duration}");
        OnCameraLookAtRequest?.Invoke(targetPosition, duration);
    }

    public static void RequestCameraLookAt(GameObject target, float duration)
    {
        Debug.Log($"[GameEvents] Requesting camera to look at GameObject: {target.name}, duration: {duration}");
        OnCameraLookAtGameObjectRequest?.Invoke(target, duration);
    }

    public static void RequestCameraFOVChange(float FOV)
    {
        Debug.Log($"[GameEvents] Requesting camera FOV change to: {FOV}");
        OnCameraFOVChangeRequest?.Invoke(FOV);
    }
    #endregion
}