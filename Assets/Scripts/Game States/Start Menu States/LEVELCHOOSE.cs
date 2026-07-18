using UnityEngine;

public class LEVELCHOOSE : IGameState
{
    public void EnterState()
    {
        GameEvents.RequestDrawerOpen();
    }

    public void UpdateState()
    {
        
    }

    public void ExitState()
    {
        GameEvents.RequestDrawerClose();

    }
}