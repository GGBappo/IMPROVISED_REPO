using UnityEngine;

public class HUB : IGameState
{
    public void EnterState()
    {
        Debug.Log("[GAME STATE] entered HUB state.");
    }

    public void UpdateState()
    {
        
    }

    public void ExitState()
    {
        Debug.Log("[GAME STATE] exiting HUB state.");
    }
}