using UnityEngine;

public class ACTIVE : IGameState
{
    public void EnterState(GameStateManager manager)
    {
        Debug.Log("[GAME STATE] entered ACTIVE state.");
    }

    public void UpdateState(GameStateManager manager)
    {
        
    }

    public void ExitState(GameStateManager manager)
    {
        Debug.Log("[GAME STATE] exiting ACTIVE state.");
    }
}