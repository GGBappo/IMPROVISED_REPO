using System;
using UnityEngine;

public static class GameEvents
{
    // timer/strike events
    public static event Action<int, float> OnStrikeOccurred; // (strikeCount, penaltyApplied)
    public static event Action OnTimerExpired;

    // state events
    public static event Action<GlobalStateType> OnGlobalStateChanged; // (newState)
    public static event Action<StartMenuState> OnStartMenuStateChanged;
    public static event Action OnRequestRestorePreviousStartMenuState;

    // scene/level events
    public static event Action<string, TransitionType, bool> OnRequestSceneLoad;
    public static event Action<string> OnRequestSceneUnLoad;
    public static event Action<TransitionType> OnRequestLevelReset;
    public static event Action OnRequestLevelEnd;
    public static event Action<int> OnRequestLevelStart;
    public static event Action<string> OnSceneFullyLoaded; // (sceneName)
    public static event Action OnRequestDrawerOpen;
    public static event Action OnRequestDrawerClose;
    public static event Action OnRequestLatestAssignmentFolderSpawn;
    public static event Action<Transform> OnDataPassLatestAssignmentFolderSpawn;
    public static event Action<RuntimeDialogueGraph, string> OnRequestDialogueStart;
    public static event Action OnRequestDialogueEnd;

    /// UI events
    public static event Action OnStartButtonPressed; // please note this is to be depricated soon until i refactor the code to use the state manager more
    public static event Action OnDialogueButtonPressed; // THIS EVENT IS HERE IF NEED BE, IT IS CURRENTLY NOT REFERENCED BY ANYTHING
    public static event Action OnRequestSettingsMenuOpen;
    public static event Action OnRequestSettingsMenuClose;
    public static event Action OnShowGameOverRequested;
    public static event Action OnHideGameOverRequested;
    public static event Action OnShowWinScreenRequested;
    public static event Action OnHideWinScreenRequested;
    public static event Action OnRequestShowDialogueUI;
    public static event Action OnRequestHideDialogueUI;
    public static event Action<TransitionType, Action> OnTransitionINRequested; 
    public static event Action<TransitionType, Action> OnTransitionOUTRequested; 
    public static event Action<float, CanvasGroup, Canvas> OnFadeOutUIElementRequested; // (duration, canvasGroup, canvas)
    public static event Action<float, CanvasGroup, Canvas> OnFadeInUIElementRequested; // (duration, canvasGroup, canvas)
    public static event Action<DialogueBoxPosition> OnDialogueBoxMove;
    public static event Action<string> OnPingObjectToHighlight;
    public static event Action<string> OnPingObjectToUnhighlight;
    public static event Action<string, string, string, int> OnRequestOpenFileScreen;
    public static event Action OnRequestCloseFileScreen;


    // camera events
    public static event Action<Vector3, Quaternion, float, Vector3?, float?> OnCameraMoveRequest; // (position, rotation, duration, lookAtMarker, FOV)
    public static event Action<Vector3, float, float> OnCameraLookAtRequest; // (targetPosition, duration, FOV)
    public static event Action<GameObject, float, float> OnCameraLookAtGameObjectRequest; // (targetGameObject, duration, FOV)
    public static event Action<float, bool, float> OnCameraFOVChangeRequest; // (newFOV, slowZoom, duration)
    
    // Start Screen events
    public static event Action OnRequestNPCInteractionSequence; // (no parameters)
    public static event Action OnRequestNPCInteractionSequenceExit; // (no parameters)

    #region Timer & Strike Calls
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
    /// <param name="newState">The new global state.</param>
    public static void GlobalStateChanged(GlobalStateType newState)
    {
        OnGlobalStateChanged?.Invoke(newState);
    }
    /// <summary>
    /// Invoke the OnStartMenuStateChanged event to notify subscribers that the start menu state has changed.
    /// </summary>
    /// <param name="newState">The new start menu state.</param>
    public static void StartMenuStateChanged(StartMenuState newState)
    {
        OnStartMenuStateChanged?.Invoke(newState);
    }

    /// <summary>
    /// Requests that the start menu restore its previous state after the file screen closes.
    /// </summary>
    public static void RestorePreviousStartMenuState()
    {
        OnRequestRestorePreviousStartMenuState?.Invoke();
    }
    #endregion

    #region Scene/Level Calls
    /// <summary>
    /// Requests a scene to be loaded. Idealy, this should be used alongside <see cref="RequestSceneUnLoad"/> in order to unload scenes.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    /// <param name="transition">The transition to play while loading.</param>
    /// <param name="setActive">Whether the loaded scene should be set active.</param>
    public static void RequestSceneLoad(string sceneName, TransitionType transition = TransitionType.None, bool setActive = true)
    {
        Debug.Log($"[GameEvents] Requesting load of scene: {sceneName} with transition: {transition}");
        OnRequestSceneLoad?.Invoke(sceneName, transition, setActive);
    }

    /// <summary>
    /// Requests a scene to be unloaded. Idealy, this should be used alongside <see cref="RequestSceneLoad"/> in order to load scenes.
    /// </summary>
    /// <param name="sceneName">The name of the scene to unload.</param>
    public static void RequestSceneUnLoad(string sceneName)
    {
        Debug.Log($"[GameEvents] Requesting unload of scene: {sceneName}");
        OnRequestSceneUnLoad?.Invoke(sceneName);
    }

    /// <summary>
    /// Requests the start of the current level. This should be used to notify subscribers that the current level has started and any necessary setup should be performed.
    /// </summary>
    public static void RequestLevelStart(int levelIndex)
    {
        Debug.Log($"[GameEvents] Requesting start of current level");
        OnRequestLevelStart?.Invoke(levelIndex);
    }

    /// <summary>
    /// Requests a level reset. This should be used to reset the current level.
    /// </summary>
    /// <param name="transition">The transition to play while resetting the level.</param>
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

    /// <summary>
    /// Invoke the OnSceneFullyLoaded event to notify subscribers that a scene has finished loading.
    /// </summary>
    /// <param name="sceneName">The name of the loaded scene.</param>
    public static void SceneFullyLoaded(string sceneName)
    {
        Debug.Log($"[GameEvents] Scene fully loaded: {sceneName}");
        OnSceneFullyLoaded?.Invoke(sceneName);
    }

    /// <summary>
    /// Invoke the OnRequestDrawerOpen event to request that the drawer be opened.
    /// </summary>
    public static void RequestDrawerOpen()
    {
        OnRequestDrawerOpen?.Invoke();
    }

    /// <summary>
    /// Invoke the OnRequestDrawerClose event to request that the drawer be closed.
    /// </summary>
    public static void RequestDrawerClose()
    {
        OnRequestDrawerClose?.Invoke();
    }

    /// <summary>
    /// Invoke the OnRequestLatestAssignmentFolderSpawn event to request spawning the latest assignment folder.
    /// </summary>
    /// <param name="spawnPosition">The position where the folder should be spawned.</param>
    public static void RequestLatestAssignmentFolderSpawn()
    {
        OnRequestLatestAssignmentFolderSpawn?.Invoke();
    }

    /// <summary>
    /// Invoke the OnDataPassLatestAssignmentFolderSpawn event to finalize spawning the latest assignment folder with a specific spawn position.
    /// This should be used in conjunction with <see cref="RequestLatestAssignmentFolderSpawn"/> in order to pass a request from an action node.
    /// </summary>
    /// <param name="spawnPosition"></param>
    public static void DataPassLatestAssignmentFolderSpawn(Transform spawnPosition)
    {
        OnDataPassLatestAssignmentFolderSpawn?.Invoke(spawnPosition);
    }
    #endregion

    #region UI Calls
    /// <summary>
    /// Invoke the OnStartButtonPressed event to notify subscribers that the start button was pressed.
    /// </summary>
    public static void StartButtonPressed()
    {
        OnStartButtonPressed?.Invoke();
        Debug.Log("[GameEvents] OnStartButtonPressed invoked");
    }

    /// <summary>
    /// Invoke the OnDialogueButtonPressed event to notify subscribers that the dialogue button was pressed.
    /// </summary>
    public static void DialougeButtonPressed() // temporarily placed here if need be.
    {
        OnDialogueButtonPressed?.Invoke();
        Debug.Log("[GameEvents] OnDialougeButtonPressed invoked");
    }

    /// <summary>
    /// Invoke the OnRequestDialogueStart event to request that dialogue begin at the specified node.
    /// </summary>
    /// <param name="dialogueGraph">The dialogue graph to start from.</param>
    /// <param name="nodeID">The node ID to start from, or null to use the entry node.</param>
    public static void RequestDialogueStart(RuntimeDialogueGraph dialogueGraph, string nodeID = null)
    {
        Debug.Log($"[GameEvents] Requesting dialogue start: {(dialogueGraph != null ? dialogueGraph.name : "<no graph>")} / {nodeID ?? "<entry>"}");
        OnRequestDialogueStart?.Invoke(dialogueGraph, nodeID);
    }

    /// <summary>
    /// Invoke the OnRequestDialogueEnd event to request that the current dialogue end.
    /// </summary>
    public static void RequestDialogueEnd()
    {
        Debug.Log("[GameEvents] Requesting dialogue end");
        OnRequestDialogueEnd?.Invoke();
    }
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
    /// Requests the win screen to be shown.
    /// </summary>
    public static void RequestShowWinScreen()
    {
        Debug.Log("[GameEvents] Requesting to show Win Screen");
        OnShowWinScreenRequested?.Invoke();
        Debug.Log("[GameEvents] OnShowWinScreenRequested invoked");
    }

    /// <summary>
    /// Requests the win screen to be hidden.
    /// </summary>
    public static void RequestHideWinScreen()
    {
        Debug.Log("[GameEvents] Requesting to hide Win Screen");
        OnHideWinScreenRequested?.Invoke();
        Debug.Log("[GameEvents] OnHideWinScreenRequested invoked");
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

    /// <summary>
    /// Invokes the OnTransitionOUTRequested event to notify subscribers that a scene transition should complete.
    /// </summary>
    /// <param name="transition">The visual style of the transition.</param>
    /// <param name="onComplete">Optional callback executed the exact frame the transition animation finishes.</param>
    public static void RequestTransitionOUT(TransitionType transition, Action onComplete = null)
    {
        Debug.Log($"[GameEvents] Requesting transition OUT of type: {transition}");
        OnTransitionOUTRequested?.Invoke(transition, onComplete);
    }

    /// <summary>
    /// Invoke the OnFadeOutUIElementRequested event to request that a UI element fade out.
    /// </summary>
    /// <param name="duration">The fade duration in seconds.</param>
    /// <param name="canvasGroup">The CanvasGroup to fade out, if any.</param>
    /// <param name="canvas">The Canvas to fade out, if any.</param>
    public static void RequestFadeOutUIElement(float duration, CanvasGroup canvasGroup = null, Canvas canvas = null)
    {
        Debug.Log($"[GameEvents] Requesting fade out of UI element: {canvasGroup?.name ?? canvas?.name} over duration: {duration}");
        OnFadeOutUIElementRequested?.Invoke(duration, canvasGroup, canvas);
    }

    /// <summary>
    /// Invoke the OnFadeInUIElementRequested event to request that a UI element fade in.
    /// </summary>
    /// <param name="duration">The fade duration in seconds.</param>
    /// <param name="canvasGroup">The CanvasGroup to fade in, if any.</param>
    /// <param name="canvas">The Canvas to fade in, if any.</param>
    public static void RequestFadeInUIElement(float duration, CanvasGroup canvasGroup = null, Canvas canvas = null)
    {
        Debug.Log($"[GameEvents] Requesting fade in of UI element: {canvasGroup?.name ?? canvas?.name} over duration: {duration}");
        OnFadeInUIElementRequested?.Invoke(duration, canvasGroup, canvas);
    }

    /// <summary>
    /// Invoke the OnRequestSettingsMenuOpen event to request that the settings menu open.
    /// </summary>
    public static void RequestSettingsMenuOpen()
    {
        Debug.Log("[GameEvents] Requesting to open Settings Menu");
        OnRequestSettingsMenuOpen?.Invoke();
    }

    /// <summary>
    /// Invoke the OnRequestSettingsMenuClose event to request that the settings menu close.
    /// </summary>
    public static void RequestSettingsMenuClose()
    {
        Debug.Log("[GameEvents] Requesting to close Settings Menu");
        OnRequestSettingsMenuClose?.Invoke();
    }

    /// <summary>
    /// Invoke the OnRequestShowDialogueUI event to request that the dialogue UI be shown.
    /// </summary>
    public static void RequestShowDialogueUI()
    {
        Debug.Log("[GameEvents] Requesting to show Dialogue UI");
        OnRequestShowDialogueUI?.Invoke();
    }

    /// <summary>
    /// Invoke the OnRequestHideDialogueUI event to request that the dialogue UI be hidden.
    /// </summary>
    public static void RequestHideDialogueUI()
    {
        Debug.Log("[GameEvents] Requesting to hide Dialogue UI");
        OnRequestHideDialogueUI?.Invoke();
    }
    
    /// <summary>
    /// Invoke the OnDialogueBoxMove event to request that the dialogue box move to a new position.
    /// </summary>
    /// <param name="dialogueBoxPosition">The target position for the dialogue box.</param>
    public static void RequestDialogueBoxMove(DialogueBoxPosition dialogueBoxPosition)
    {
        Debug.Log($"[GameEvents] Requesting to move Dialogue box to {dialogueBoxPosition} position");
        OnDialogueBoxMove?.Invoke(dialogueBoxPosition);
    }

    /// <summary>
    /// Invoke the OnPingObjectToHighlight event to request that an object be highlighted.
    /// </summary>
    /// <param name="objectID">The object ID to highlight.</param>
    public static void PingObjectToHightlight(string objectID)
    {
        Debug.Log($"[GameEvents] Pinging object with object ID '{objectID}' to highlight");
        OnPingObjectToHighlight?.Invoke(objectID);
    }

    /// <summary>
    /// Invoke the OnPingObjectToUnhighlight event to request that an object be unhighlighted.
    /// </summary>
    /// <param name="objectID">The object ID to unhighlight.</param>
    public static void PingObjectToUnightlight(string objectID)
    {
        Debug.Log($"[GameEvents] Pinging object with object ID '{objectID}' to unhighlight");
        OnPingObjectToHighlight?.Invoke(objectID);
    }

    public static void RequestOpenFileScreen(string levelName, string levelLocation, string levelDescription, int levelIndex)
    {
        Debug.Log($"[GameEvents] Opening file screen for level with the name {levelName}");
        OnRequestOpenFileScreen?.Invoke(levelName, levelLocation, levelDescription, levelIndex);
    }
    
    public static void RequestCloseFileScreen()
    {
        Debug.Log("[GameEvents] Closing file screen");
        OnRequestCloseFileScreen?.Invoke();
    }
    #endregion

    #region Camera Calls

    /// <summary>
    /// Invoke the OnCameraMoveRequest event to request that the camera move to a new position and rotation.
    /// </summary>
    /// <param name="position">The target position.</param>
    /// <param name="rotation">The target rotation.</param>
    /// <param name="duration">The movement duration in seconds.</param>
    /// <param name="lookAtMarker">Optional marker for the camera to look at.</param>
    /// <param name="FOV">Optional field of view to apply during the move.</param>
    public static void RequestCameraMove(Vector3 position, Quaternion rotation, float duration, Vector3? lookAtMarker = null, float? FOV = null)
    {
        Debug.Log($"[GameEvents] Requesting camera move to position: {position}, rotation: {rotation}, duration: {duration}");
        OnCameraMoveRequest?.Invoke(position, rotation, duration, lookAtMarker, FOV);
    }

    /// <summary>
    /// Invoke the OnCameraLookAtRequest event to request that the camera look at a world position.
    /// </summary>
    /// <param name="targetPosition">The position for the camera to look at.</param>
    /// <param name="duration">The look-at duration in seconds.</param>
    /// <param name="FOV">The field of view to use while looking at the target.</param>
    public static void RequestCameraLookAt(Vector3 targetPosition, float duration, float FOV = 50f)
    {
        Debug.Log($"[GameEvents] Requesting camera to look at position: {targetPosition}, duration: {duration}, FOV: {FOV}");
        OnCameraLookAtRequest?.Invoke(targetPosition, duration, FOV);
    }

    /// <summary>
    /// Invoke the OnCameraLookAtGameObjectRequest event to request that the camera look at a GameObject.
    /// </summary>
    /// <param name="target">The target GameObject.</param>
    /// <param name="duration">The look-at duration in seconds.</param>
    /// <param name="FOV">The field of view to use while looking at the target.</param>
    public static void RequestCameraLookAt(GameObject target, float duration, float FOV = 50f)
    {
        Debug.Log($"[GameEvents] Requesting camera to look at GameObject: {target.name}, duration: {duration}, FOV: {FOV}");
        OnCameraLookAtGameObjectRequest?.Invoke(target, duration, FOV);
    }

    /// <summary>
    /// Invoke the OnCameraFOVChangeRequest event to request a change to the camera field of view.
    /// </summary>
    /// <param name="FOV">The target field of view.</param>
    /// <param name="slowZoom">Whether the change should be treated as a slow zoom.</param>
    /// <param name="duration">The change duration in seconds.</param>
    public static void RequestCameraFOVChange(float FOV, bool slowZoom = false, float duration = 1f)
    {
        Debug.Log($"[GameEvents] Requesting camera FOV change to: {FOV}, slowZoom: {slowZoom}, duration: {duration}");
        OnCameraFOVChangeRequest?.Invoke(FOV, slowZoom, duration);
    }
    #endregion

    #region Start Screen Calls
    /// <summary>
    /// Invoke the OnRequestNPCInteractionSequence event to request the NPC interaction sequence.
    /// </summary>
    public static void RequestNPCInteractionSequence()
    {
        Debug.Log("[GameEvents] Requesting NPC interaction sequence");
        OnRequestNPCInteractionSequence?.Invoke();
    }

    /// <summary>
    /// Invoke the OnRequestNPCInteractionSequenceExit event to request the exit of the NPC interaction sequence.
    /// </summary>
    public static void RequestNPCInteractionSequenceExit()
    {
        Debug.Log("[GameEvents] Requesting exit of NPC interaction sequence");
        OnRequestNPCInteractionSequenceExit?.Invoke();
    }
    #endregion
}