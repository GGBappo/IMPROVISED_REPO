using UnityEngine;

public class OUTCOME : IGameState
{
    public void EnterState()
    {
        if (GameSessionData.won)
        {
            GameEvents.RequestShowWinScreen();
        }
        else if (GameSessionData.lostOnTime || GameSessionData.lostOnStrikes || GameSessionData.lost) // should we have two different screens for cause of death?
        {
            GameEvents.RequestShowGameOverScreen();
        }
    }

    public void UpdateState()
    {
        // this shouldn't do anytihng in this state
    }

    public void ExitState()
    {
        GameEvents.RequestHideGameOverScreen();
        GameEvents.RequestHideWinScreen();
    }
}