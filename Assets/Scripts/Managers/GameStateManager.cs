using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public MENU menuState = new MENU();
    public ACTIVE activeState = new ACTIVE();
    public HUB hubState = new HUB();
    public OUTCOME outcomeState = new OUTCOME();

    private IGameState currentState;
    private bool isReady = false;
    
    void Setup()
    {
        ChangeState(menuState);
        isReady = true;
    }

    void Update()
    {
        if (!isReady) return;
        currentState?.UpdateState(this); 
    }

    public void ChangeState(IGameState newState)
    {
        currentState?.ExitState(this); 
        currentState = newState;
        currentState?.EnterState(this); 
    }
}
