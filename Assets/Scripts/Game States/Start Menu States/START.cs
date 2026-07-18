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
    public START(CanvasGroup startButtonCanvasGroup)
    {
        _startButtonCanvasGroup = startButtonCanvasGroup;
    }
    public void EnterState()
    {
        
    }

    public void UpdateState()
    {
        
    }

    public void ExitState()
    {
        GameEvents.RequestFadeOutUIElement(defaultTweenDuration, canvasGroup: _startButtonCanvasGroup);
    }
}