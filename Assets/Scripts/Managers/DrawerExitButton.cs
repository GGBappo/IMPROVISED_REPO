using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class DrawerButton : MonoBehaviour
{
    private bool _isPressed = false; // if _isPressed is true, that would mean the drawer is open
    public TMP_Text hoverTextDuringClosedDrawer;
    public TMP_Text hoverTextDuringOpenDrawer;

    public void HandleOpenAndClose()
    {
        if (!_isPressed)
        {
            GameEvents.StartMenuStateChanged(StartMenuState.LevelChoose);
            _isPressed = true;
        }
        else
        {
            GameEvents.StartMenuStateChanged(StartMenuState.Await);
            _isPressed = false;
        }
    }
    /*
    public void TextHoverHandler()
    {
        if (!_isPressed)
        {
            
        }
    }
    */
}