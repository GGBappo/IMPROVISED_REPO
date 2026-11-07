using UnityEngine;

public class BeerPongStateManager : MonoBehaviour
{
    private readonly StateMachine<BeerPongMinigameStates> stateMachine = new StateMachine<BeerPongMinigameStates>();
    public BeerPongMinigameStates CurrentState => stateMachine.CurrentState;

    // subscribe to events
    private void OnEnable()
    {
        GameEvents.OnRequestBeerPongMinigameStateChange += SwitchState;
    }

    private void OnDisable()
    {
        GameEvents.OnRequestBeerPongMinigameStateChange -= SwitchState;
    }

    private void Awake()
    {
        stateMachine.RegisterState(BeerPongMinigameStates.PlayerTurn, new PLAYERTURN());
        stateMachine.RegisterState(BeerPongMinigameStates.AITurn, new AITURN());
        stateMachine.RegisterState(BeerPongMinigameStates.PlayerWin, new PLAYERWIN());
        stateMachine.RegisterState(BeerPongMinigameStates.AIWin, new AIWIN());

        stateMachine.AllowTransition(BeerPongMinigameStates.PlayerTurn, BeerPongMinigameStates.AITurn);
        stateMachine.AllowTransition(BeerPongMinigameStates.PlayerTurn, BeerPongMinigameStates.PlayerWin);
        stateMachine.AllowTransition(BeerPongMinigameStates.PlayerTurn, BeerPongMinigameStates.AIWin);
        stateMachine.AllowTransition(BeerPongMinigameStates.AITurn, BeerPongMinigameStates.PlayerTurn);
        stateMachine.AllowTransition(BeerPongMinigameStates.AITurn, BeerPongMinigameStates.PlayerWin);
        stateMachine.AllowTransition(BeerPongMinigameStates.AITurn, BeerPongMinigameStates.AIWin);
        stateMachine.AllowTransition(BeerPongMinigameStates.PlayerWin, BeerPongMinigameStates.PlayerTurn);
        stateMachine.AllowTransition(BeerPongMinigameStates.AIWin, BeerPongMinigameStates.PlayerTurn);
    }

    private void Start()
    {
        stateMachine.Start(BeerPongMinigameStates.PlayerTurn);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void SwitchState(BeerPongMinigameStates targetState)
    {
        stateMachine.TryChangeState(targetState);
    }
}