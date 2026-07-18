using UnityEngine;
using UnityEngine.EventSystems;

public class DrawerExitButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        GameEvents.StartMenuStateChanged(StartMenuState.Await);
    }
}