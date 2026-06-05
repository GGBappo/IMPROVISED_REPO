using UnityEngine;

public class HUB : IGameState
{
    public void EnterState(GameStateManager manager)
    {
        Debug.Log("[GAME STATE] entered HUB state.");
    }

    public void UpdateState(GameStateManager manager)
    {
        
    }

    public void ExitState(GameStateManager manager)
    {
        Debug.Log("[GAME STATE] exiting HUB state.");
    }
}