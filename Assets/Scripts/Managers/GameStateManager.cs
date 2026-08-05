using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    // these are the four main global states of the game
    // these classes should only be accessed by the GameStateManager
    // changing states is done through changing the enum GlobalStateType
    private readonly MENU menuState = new MENU();
    private readonly ACTIVE activeState = new ACTIVE();
    private readonly HUB hubState = new HUB();
    private readonly DIALOGUE dialogueState = new DIALOGUE();
    private readonly OUTCOME outcomeState = new OUTCOME();

    private IGameState currentState;

    // subscribe to events
    private void OnEnable() => GameEvents.OnGlobalStateChanged += SwitchState;
    private void OnDisable() => GameEvents.OnGlobalStateChanged -= SwitchState;

    void Start()
    {
        // start the game in the menu state
        // this is assuming we're saying the title screen
        // also counts as a global state.
        SwitchState(GlobalStateType.Menu);
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
    private void SwitchState(GlobalStateType targetState)
    {
        currentState?.ExitState(); 

        switch (targetState)
        {
            case GlobalStateType.Menu: currentState = menuState; break;
            case GlobalStateType.Active: currentState = activeState; break;
            case GlobalStateType.Hub: currentState = hubState; break;
            case GlobalStateType.Dialogue: currentState = dialogueState; break;
            case GlobalStateType.Outcome: currentState = outcomeState; break;
        }

        currentState?.EnterState(); 
    }
}
