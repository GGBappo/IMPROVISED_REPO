using UnityEngine;

public class StartMenuStateManager : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _startButtonCanvasGroup;
    [SerializeField]
    private GameObject _cameraMarker;

    // readonly is taken off these in order to assign their respective constructors in the Awake function
    private START startState;
    
    private readonly TASKHANDOUT taskHandoutState = new TASKHANDOUT();
    private readonly LEVELSELECT levelSelectState = new LEVELSELECT();
    private readonly LEVELCHOOSE levelChooseState = new LEVELCHOOSE();
    private readonly SETTINGS settingsState = new SETTINGS(); // this settings state is local to the start menu, if the settings menu is opened in any other place this should NOT be used.
    private readonly AWAIT awaitState = new AWAIT();
    
    private IGameState currentState;

    // subscribe to events
    private void OnEnable() => GameEvents.OnStartMenuStateChanged += SwitchState;
    private void OnDisable() => GameEvents.OnStartMenuStateChanged -= SwitchState;

    void Start()
    {
        SwitchState(StartMenuState.Start);
    }
    private void Awake()
    {
        startState = new START(_startButtonCanvasGroup);
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
        currentState?.ExitState(); 

        switch (targetState)
        {
            case StartMenuState.Start: currentState = startState; break;
            case StartMenuState.TaskHandout: currentState = taskHandoutState; break;
            case StartMenuState.LevelSelect: currentState = levelSelectState; break;
            case StartMenuState.LevelChoose: currentState = levelChooseState; break;
            case StartMenuState.Settings: currentState = settingsState; break;
            case StartMenuState.Await: currentState = awaitState; break;
        }

        currentState?.EnterState(); 
    }
}
