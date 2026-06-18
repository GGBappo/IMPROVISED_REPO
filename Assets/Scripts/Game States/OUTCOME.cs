using UnityEngine;

public class OUTCOME : IGameState
{
    public void EnterState()
    {
        Debug.Log("[GAME STATE] entered OUTCOME state.");
    }

    public void UpdateState()
    {
        
    }

    public void ExitState()
    {
        Debug.Log("[GAME STATE] exiting OUTCOME state.");
    }
}