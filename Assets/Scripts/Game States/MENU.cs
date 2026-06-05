using UnityEngine;

public class MENU : IGameState
{
    public void EnterState(GameStateManager manager)
    {
        Debug.Log("[GAME STATE] entered MENU state.");
    }

    public void UpdateState(GameStateManager manager)
    {
        
    }

    public void ExitState(GameStateManager manager)
    {
        Debug.Log("[GAME STATE] exiting MENU state.");
    }
}