using UnityEngine;

public class BeerPongStateManager : MonoBehaviour
{
    private readonly PLAYERTURN playerTurnState = new PLAYERTURN();
    private readonly PLAYERWIN playerWinState = new PLAYERWIN();
    private readonly AITURN AITurnState = new AITURN();
    private readonly AIWIN AIWinState = new AIWIN();
    
    
    private IGameState currentState;

    // subscribe to events
    private void OnEnable()
    {
        GameEvents.OnRequestBeerPongMinigameStateChange += SwitchState;
    }

    private void OnDisable()
    {
        GameEvents.OnRequestBeerPongMinigameStateChange -= SwitchState;
    }

    void Start()
    {
        SwitchState(BeerPongMinigameStates.PlayerTurn);
    }
    private void Awake()
    {
        
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
    private void SwitchState(BeerPongMinigameStates targetState)
    {
        currentState?.ExitState(); 

        switch (targetState)
        {
            case BeerPongMinigameStates.PlayerTurn: currentState = playerTurnState; break;
            case BeerPongMinigameStates.AITurn: currentState = AITurnState; break;
            case BeerPongMinigameStates.PlayerWin: currentState = playerWinState; break;
            case BeerPongMinigameStates.AIWin: currentState = AIWinState; break;
            default:
                Debug.LogWarning($"[StartMenuStateManager] Unknown start menu state: {targetState}");
                currentState = null;
                return;
        }

        currentState?.EnterState(); 
    }

}