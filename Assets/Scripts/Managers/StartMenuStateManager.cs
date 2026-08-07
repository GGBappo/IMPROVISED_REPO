using UnityEngine;

public class StartMenuStateManager : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _startButtonCanvasGroup;
    [SerializeField]
    private CameraMarkersHolder _cameraMarkerHolder;

    // readonly is taken off these in order to assign their respective constructors in the Awake function
    private START startState;
    private AWAIT awaitState;

    private readonly TASKHANDOUT taskHandoutState = new TASKHANDOUT();
    private readonly LEVELSELECT levelSelectState = new LEVELSELECT();
    private readonly LEVELCHOOSE levelChooseState = new LEVELCHOOSE();
    private readonly SETTINGS settingsState = new SETTINGS(); // this settings state is local to the start menu, if the settings menu is opened in any other place this should NOT be used.
    
    private IGameState currentState;
    [SerializeField]
    private StartMenuState currentStateType = StartMenuState.Start;
    private StartMenuState previousState = StartMenuState.Start;

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

    void Start()
    {
        SwitchState(StartMenuState.Start);
    }
    private void Awake()
    {
        startState = new START(_startButtonCanvasGroup, _cameraMarkerHolder);
        awaitState = new AWAIT(_cameraMarkerHolder.cameraMarkers[0]);
    }
    void Update()
    {
        // update the current state every frame
        currentState?.UpdateState();
    }

    /// <summary>
    /// Switches the current game state to the specified target state.
    /// </summary>
    /// <param name="targetState"></param>
    private void SwitchState(StartMenuState targetState)
    {
        if (currentState != null && currentStateType == targetState)
        {
            return;
        }

        if (targetState == StartMenuState.LevelSelect && currentStateType != StartMenuState.LevelSelect)
        {
            previousState = currentStateType;
        }

        currentState?.ExitState(); 

        switch (targetState)
        {
            case StartMenuState.Start: currentState = startState; break;
            case StartMenuState.TaskHandout: currentState = taskHandoutState; break;
            case StartMenuState.LevelSelect: currentState = levelSelectState; break;
            case StartMenuState.LevelChoose: currentState = levelChooseState; break;
            case StartMenuState.Settings: currentState = settingsState; break;
            case StartMenuState.Await: currentState = awaitState; break;
            default:
                Debug.LogWarning($"[StartMenuStateManager] Unknown start menu state: {targetState}");
                currentState = null;
                return;
        }

        currentStateType = targetState;
        currentState?.EnterState(); 
    }

    private void RestorePreviousState()
    {
        if (previousState == StartMenuState.LevelSelect)
        {
            previousState = StartMenuState.Await;
        }

        SwitchState(previousState);
    }
}
