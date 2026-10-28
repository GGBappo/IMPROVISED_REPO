using UnityEngine;

public class StartMenuStateManager : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _startButtonCanvasGroup;
    [SerializeField]
    private CameraMarkersHolder _cameraMarkerHolder;

    private StateMachine<StartMenuState> stateMachine;
    private StartMenuState previousState = StartMenuState.Start;
    private StartMenuState pendingState;
    private bool hasPendingState;

    public StartMenuState CurrentState => stateMachine.CurrentState;

    // subscribe to events
    private void OnEnable()
    {
        GameEvents.OnStartMenuStateChanged += SwitchState;
        GameEvents.OnRequestRestorePreviousStartMenuState += RestorePreviousState;
    }

    private void OnDisable()
    {
        GameEvents.OnStartMenuStateChanged -= SwitchState;
        GameEvents.OnRequestRestorePreviousStartMenuState -= RestorePreviousState;
    }

    private void Awake()
    {
        stateMachine = new StateMachine<StartMenuState>();
        stateMachine.RegisterState(StartMenuState.Start, new START(_startButtonCanvasGroup, _cameraMarkerHolder));
        stateMachine.RegisterState(StartMenuState.TaskHandout, new TASKHANDOUT());
        stateMachine.RegisterState(StartMenuState.Await, new AWAIT(_cameraMarkerHolder.cameraMarkers[0]));
        stateMachine.RegisterState(StartMenuState.Wardrobe, new WARDROBE(_cameraMarkerHolder.cameraMarkers[0]));
        stateMachine.RegisterState(StartMenuState.Hallway, new HALLWAY(_cameraMarkerHolder.cameraMarkers[0]));
        stateMachine.RegisterState(StartMenuState.FileDetails, new FILEDETAILS());
        stateMachine.RegisterState(
            StartMenuState.Drawer,
            new DRAWER(_cameraMarkerHolder.cameraMarkers[2], _cameraMarkerHolder.cameraMarkers[0]));
        stateMachine.RegisterState(StartMenuState.Settings, new SETTINGS());

        stateMachine.AllowTransition(StartMenuState.Start, StartMenuState.TaskHandout);
        stateMachine.AllowTransition(StartMenuState.TaskHandout, StartMenuState.Await);
        stateMachine.AllowTransition(StartMenuState.Await, StartMenuState.Wardrobe);
        stateMachine.AllowTransition(StartMenuState.Await, StartMenuState.Hallway);
        stateMachine.AllowTransition(StartMenuState.Wardrobe, StartMenuState.Await);
        stateMachine.AllowTransition(StartMenuState.Hallway, StartMenuState.Await);
        stateMachine.AllowTransition(StartMenuState.Await, StartMenuState.Drawer);
        stateMachine.AllowTransition(StartMenuState.Await, StartMenuState.FileDetails);
        stateMachine.AllowTransition(StartMenuState.Await, StartMenuState.Settings);
        stateMachine.AllowTransition(StartMenuState.Drawer, StartMenuState.Await);
        stateMachine.AllowTransition(StartMenuState.Drawer, StartMenuState.FileDetails);
        stateMachine.AllowTransition(StartMenuState.FileDetails, StartMenuState.Await);
        stateMachine.AllowTransition(StartMenuState.FileDetails, StartMenuState.Drawer);
        stateMachine.AllowTransition(StartMenuState.Settings, StartMenuState.Await);
    }

    private void Start()
    {
        Debug.Log("[StartMenuStateManager] Starting in Start state.");
        stateMachine.Start(StartMenuState.Start);

        if (hasPendingState)
        {
            StartMenuState requestedState = pendingState;
            hasPendingState = false;
            Debug.Log($"[StartMenuStateManager] Applying queued state request: {requestedState}.");
            SwitchState(requestedState);
        }
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void SwitchState(StartMenuState targetState)
    {
        if (stateMachine == null || !stateMachine.HasCurrentState)
        {
            pendingState = targetState;
            hasPendingState = true;
            Debug.Log($"[StartMenuStateManager] Queued state request before startup: {targetState}.");
            return;
        }

        Debug.Log($"[StartMenuStateManager] State change requested: {stateMachine.CurrentState} -> {targetState}.");

        if (targetState == StartMenuState.FileDetails && stateMachine.HasCurrentState)
        {
            previousState = stateMachine.CurrentState;
        }

        stateMachine.TryChangeState(targetState);
    }

    private void RestorePreviousState()
    {
        if (previousState == StartMenuState.FileDetails)
        {
            previousState = StartMenuState.Await;
        }

        SwitchState(previousState);
    }
}
