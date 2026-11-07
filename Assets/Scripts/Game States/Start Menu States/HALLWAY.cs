using UnityEngine;
using static RuntimeSettings;

public class HALLWAY : IGameState
{
    private readonly CameraMarker _awaitCameraMarker;

    public HALLWAY(CameraMarker awaitCameraMarker)
    {
        _awaitCameraMarker = awaitCameraMarker;
    }

    public void EnterState()
    {
        RequestCameraMove(90f);
    }

    public void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            GameEvents.StartMenuStateChanged(StartMenuState.Await);
        }
    }

    public void ExitState()
    {
    }

    private void RequestCameraMove(float yawOffset)
    {
        if (_awaitCameraMarker == null)
        {
            Debug.LogError("[StartMenuState] Cannot enter Hallway state because the await camera marker is missing.");
            return;
        }

        Quaternion targetRotation = _awaitCameraMarker.transform.rotation * Quaternion.Euler(0f, yawOffset, 0f);
        GameEvents.RequestCameraMove(
            _awaitCameraMarker.transform.position,
            targetRotation,
            defaultTweenDuration);
    }
}
