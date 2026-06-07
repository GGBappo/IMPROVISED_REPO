using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

//Controls all the interactions with the items in the inventory,
//such as dragging and dropping, using, and hovering over them.
public class InteractableItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public Vector3 spawnPos;

    public virtual void OnUse()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        while (Input.GetMouseButton(0)) // Check if the left mouse button is held down
        {
            this.transform.position = Input.mousePosition; // Move the item to the mouse position
            Debug.Log("Dropped: " + gameObject.name);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        //if Target exists and is valid for the item, then use the item on the target
        if (true)
        {
            OnUse();
        }
        else
        {
            this.transform.position = spawnPos; // Move the item back to its original position
        }
        Debug.Log("Dropped: " + gameObject.name);
    }
}
