using UnityEngine;

public class ACTIVE : IGameState
{
    public void EnterState()
    {
        Debug.Log("[GAME STATE] entered ACTIVE state.");
    }

    public void UpdateState()
    {
        
    }

    public void ExitState()
    {
        Debug.Log("[GAME STATE] exiting ACTIVE state.");
    }
}