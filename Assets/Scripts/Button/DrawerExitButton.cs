using UnityEngine;
using DG.Tweening;
using static RuntimeSettings;
using UnityEngine.EventSystems;

public class DrawerButton : MonoBehaviour
{
    private bool _isPressed = false; // if _isPressed is true, that would mean the drawer is open
    public CanvasGroup hoverTextDuringClosedDrawer;
    public CanvasGroup hoverTextDuringOpenDrawer;
    public EventTrigger eventTrigger;

    public void Awake()
    {
        eventTrigger.enabled = false;
    }

    public void OnEnable()
    {
        GameEvents.OnStartMenuStateChanged += OnStateChanged;
    }
    public void OnDisable()
    {
        GameEvents.OnStartMenuStateChanged -= OnStateChanged;
    }

    public void OnStateChanged(StartMenuState newState)
    {
        _isPressed = newState == StartMenuState.LevelChoose;

        if (newState == StartMenuState.Await || newState == StartMenuState.LevelChoose)
        {
            eventTrigger.enabled = true;
        } 
        else
        {
            eventTrigger.enabled = false;
        }

        TextHoverHandlerOUT();
    }

    public void HandleOpenAndClose()
    {
        if (!_isPressed)
        {
            GameEvents.StartMenuStateChanged(StartMenuState.LevelChoose);
        }
        else
        {
            GameEvents.StartMenuStateChanged(StartMenuState.Await);
        }
    }
    
    public void TextHoverHandlerIN()
    {
        if (!_isPressed)
        {
            GameEvents.RequestFadeInUIElement(defaultTweenDuration, canvasGroup: hoverTextDuringClosedDrawer);
            hoverTextDuringClosedDrawer.transform.DOLocalMoveY(0.4f, defaultTweenDuration).SetEase(Ease.OutSine);
        }
        if (_isPressed)
        {
            GameEvents.RequestFadeInUIElement(defaultTweenDuration, canvasGroup: hoverTextDuringOpenDrawer);
            hoverTextDuringOpenDrawer.transform.DOLocalMoveX(0.1f, defaultTweenDuration).SetEase(Ease.OutSine);
        }
    }
    public void TextHoverHandlerOUT()
    {
        if (!_isPressed)
        {
            GameEvents.RequestFadeOutUIElement(defaultTweenDuration, canvasGroup: hoverTextDuringClosedDrawer);
            hoverTextDuringClosedDrawer.transform.DOLocalMoveY(0.2f, defaultTweenDuration).SetEase(Ease.InSine);
        }
        if (_isPressed)
        {
            GameEvents.RequestFadeOutUIElement(defaultTweenDuration, canvasGroup: hoverTextDuringOpenDrawer);
            hoverTextDuringOpenDrawer.transform.DOLocalMoveX(-0.078f, defaultTweenDuration).SetEase(Ease.InSine);
        }
    }
}