using UnityEngine;
using static RuntimeSettings;

public class SETTINGS : IGameState
{
    public void EnterState()
    {
        GameEvents.RequestSettingsMenuOpen();
    }

    public void UpdateState()
    {
        
    }

    public void ExitState()
    {
        GameEvents.RequestSettingsMenuClose();
    }
}