using UnityEngine;

public class MENU : IGameState
{
    public void EnterState()
    {
        Debug.Log("[GAME STATE] entered MENU state.");
    }

    public void UpdateState()
    {
        
    }

    public void ExitState()
    {
        Debug.Log("[GAME STATE] exiting MENU state.");
    }
}