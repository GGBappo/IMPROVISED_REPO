using System.Security.Cryptography;
using UnityEngine;

//Controls all the interactions with the items in the inventory,
//such as dragging and dropping, using, and hovering over them.
public class InteractableItem : MonoBehaviour
{
    public Vector3 spawnPos;

    public void OnUse()
    {
        // Implement the logic for what happens when the item is used.
        Debug.Log("Using: " + gameObject.name);
    }

    public void OnClick()
    {
        Debug.Log("Item clicked: " + gameObject.name);

        OnDrag();
    }

    public void OnHoverEnter()
    {
        Debug.Log("Hover entered: " + gameObject.name);
    }

    public void OnHoverExit()
    {

    }

    public void OnDrag()
    {
        while (Input.GetMouseButton(0)) // Check if the left mouse button is held down
        {
            this.transform.position = Input.mousePosition; // Move the item to the mouse position
            Debug.Log("Dropped: " + gameObject.name);
        }
    }

    public void OnDrop()
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
