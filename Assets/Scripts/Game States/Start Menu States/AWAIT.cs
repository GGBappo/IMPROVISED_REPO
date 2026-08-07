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
        GameEvents.RequestCameraMove(_cameraMarker.transform.position, _cameraMarker.transform.rotation, defaultTweenDuration);
    }

    public void UpdateState()
    {
        
    }

    public void ExitState()
    {
        
    }
}