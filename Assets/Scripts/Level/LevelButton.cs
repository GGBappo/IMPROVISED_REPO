using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using static RuntimeSettings;

public class LevelButton : MonoBehaviour
{
    [SerializeField]
    private string _levelName;
    private string _levelLocation;
    private string _levelDescription;
    private int _levelIndex;
    public CanvasGroup hoverText;
    public string objectID;

    private void OnEnable()
    {
        GameEvents.OnPingObjectToHighlight += HandleHighlight;
        GameEvents.OnPingObjectToUnhighlight += HandleUnhighlight;
    }

    private void OnDisable()
    {
        GameEvents.OnPingObjectToHighlight -= HandleHighlight;
        GameEvents.OnPingObjectToUnhighlight -= HandleUnhighlight;
    }
    
    private void HandleHighlight(string id)
    {
        if (id == objectID)
        {
            
        }
    }

    private void HandleUnhighlight(string id)
    {
        if (id == objectID)
        {
            
        }
    }
    
    public void LevelDataInjection(string levelName, string levelLocation, string levelDescription, int levelIndex)
    {
        _levelName = levelName;
        _levelLocation = levelLocation;
        _levelDescription = levelDescription;
        _levelIndex = levelIndex;
    }

    public void OpenFileScreen()
    {
        GameEvents.StartMenuStateChanged(StartMenuState.LevelSelect);
        GameEvents.RequestOpenFileScreen(_levelName, _levelLocation, _levelDescription, _levelIndex);
    }
    public void HoverTextIn()
    {
        GameEvents.RequestFadeInUIElement(defaultTweenDuration, canvasGroup: hoverText);
        hoverText.transform.DOMoveY(0.5f, defaultTweenDuration).SetEase(Ease.OutSine);
    }
    public void HoverTextOut()
    {
        GameEvents.RequestFadeOutUIElement(defaultTweenDuration, canvasGroup: hoverText);
        hoverText.transform.DOMoveY(0.275f, defaultTweenDuration).SetEase(Ease.InSine);
    }
}
