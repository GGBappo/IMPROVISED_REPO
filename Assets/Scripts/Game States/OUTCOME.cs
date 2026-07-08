using UnityEngine;

public class OUTCOME : IGameState
{
    public void EnterState()
    {
        if (GameSessionData.won)
        {
            // for the time being till we work on a win screen it'll call a game over
            GameEvents.RequestShowGameOverScreen();
        }
        else if (GameSessionData.lostOnTime || GameSessionData.lostOnStrikes) // should we have two different screens for cause of death?
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
    }
}