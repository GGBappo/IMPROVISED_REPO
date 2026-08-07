using UnityEngine;
using UnityEngine.UI;
using static RuntimeSettings;

public class START : IGameState
{
    // given that this is a local state that is called once
    // on launch, i thought it would be okay
    // to provide a simple constructor for the button
    // instead of making a billion events to pass it
    private CanvasGroup _startButtonCanvasGroup;
    private CameraMarkersHolder _cameraMarkerHolder;
    public START(CanvasGroup startButtonCanvasGroup, CameraMarkersHolder cameraMarkerHolder)
    {
        _startButtonCanvasGroup = startButtonCanvasGroup;
        _cameraMarkerHolder = cameraMarkerHolder;
    }
    public void EnterState()
    {
        GameEvents.RequestCameraMove(_cameraMarkerHolder.cameraMarkers[0].transform.position, _cameraMarkerHolder.cameraMarkers[0].transform.rotation, 0f);
    }

    public void UpdateState()
    {
        
    }

    public void ExitState()
    {
        GameEvents.RequestFadeOutUIElement(defaultTweenDuration, canvasGroup: _startButtonCanvasGroup);
        
    }
}