using UnityEngine;

public class DIALOGUE : IGameState
{
    public void EnterState()
    {
        Debug.Log("[GAME STATE] entered DIALOGUE state.");
    }

    public void UpdateState()
    {
    }

    public void ExitState()
    {
        Debug.Log("[GAME STATE] exiting DIALOGUE state.");
    }
}