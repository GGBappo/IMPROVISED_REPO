using UnityEngine;

public class DRAWER : IGameState
{
    private readonly CameraMarker _drawerCameraMarker;
    private readonly CameraMarker _awaitCameraMarker;

    public DRAWER(CameraMarker drawerCameraMarker, CameraMarker awaitCameraMarker)
    {
        _drawerCameraMarker = drawerCameraMarker;
        _awaitCameraMarker = awaitCameraMarker;
    }

    public void EnterState()
    {
        RequestCameraMove(_drawerCameraMarker, "enter");
        GameEvents.RequestDrawerOpen();
    }

    public void UpdateState()
    {
        
    }

    public void ExitState()
    {
        GameEvents.RequestDrawerClose();
        RequestCameraMove(_awaitCameraMarker, "exit");
    }

    private void RequestCameraMove(CameraMarker cameraMarker, string direction)
    {
        if (cameraMarker == null)
        {
            UnityEngine.Debug.LogError($"[StartMenuState] Cannot {direction} Drawer state because its camera marker is missing.");
            return;
        }

        UnityEngine.Debug.Log($"[StartMenuState] Drawer camera move ({direction}) to {cameraMarker.transform.position}.");
        GameEvents.RequestCameraMove(
            cameraMarker.transform.position,
            cameraMarker.transform.rotation,
            RuntimeSettings.defaultTweenDuration);
    }
}