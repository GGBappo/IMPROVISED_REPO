using UnityEngine;

public class OUTCOME : IGameState
{
    public void EnterState(GameStateManager manager)
    {
        Debug.Log("[GAME STATE] entered OUTCOME state.");
    }

    public void UpdateState(GameStateManager manager)
    {
        
    }

    public void ExitState(GameStateManager manager)
    {
        Debug.Log("[GAME STATE] exiting OUTCOME state.");
    }
}