using UnityEngine;
using static RuntimeSettings;

public class AWAIT : IGameState
{
    private CameraMarker _cameraMarker;
    public AWAIT(CameraMarker cameraMarker)
    {
        _cameraMarker = cameraMarker;
    }
    public void EnterState()
    {
        if (_cameraMarker == null)
        {
            Debug.LogError("[StartMenuState] Cannot enter Await state because its camera marker is missing.");
            return;
        }

        Debug.Log($"[StartMenuState] Entered Await state. Requesting camera move to {_cameraMarker.transform.position}.");
        GameEvents.RequestCameraMove(_cameraMarker.transform.position, _cameraMarker.transform.rotation, defaultTweenDuration);
    }

    public void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            GameEvents.StartMenuStateChanged(StartMenuState.Wardrobe);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            GameEvents.StartMenuStateChanged(StartMenuState.Hallway);
        }
    }

    public void ExitState()
    {
        
    }
}