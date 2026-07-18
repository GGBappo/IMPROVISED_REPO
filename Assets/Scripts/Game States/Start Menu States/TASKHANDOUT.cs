using UnityEngine;

public class TASKHANDOUT : IGameState
{
    public void EnterState()
    {
        GameEvents.RequestNPCInteractionSequence();
    }

    public void UpdateState()
    {
        
    }

    public void ExitState()
    {
        
    }
}