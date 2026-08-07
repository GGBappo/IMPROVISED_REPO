using UnityEngine;

public class StartButton : MonoBehaviour
{
    public void PressStart()
    {
        GameEvents.StartMenuStateChanged(StartMenuState.TaskHandout);
    }
}
