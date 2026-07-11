using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// InventorySlot.cs
// One slot in the inventory bar — handles hover rise and click to pick up

public class InventorySlot : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{

    [Header("State")]
    public Item_SO item;
    public bool isHeld = false;

    private InventoryBar inventoryBar;
    private PlayerInputManager inputManager;

    private Vector3 defaultPosition;
    private Vector3 raisedPosition;
    private bool isHovered = false;

    public void Initialize(Item_SO assignedItem, InventoryBar bar)
    {
        item = assignedItem;
        inventoryBar = bar;
        inputManager = FindObjectOfType<PlayerInputManager>();

        defaultPosition = transform.localPosition;
        raisedPosition = defaultPosition + new Vector3(0, bar.riseAmount, 0);
    }

    /*private void Update()
    {
        if (isHeld) return;  // don't animate if item is following cursor

        // smooth rise on hover
        Vector3 target = isHovered ? raisedPosition : defaultPosition;
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            target,
            Time.deltaTime * inventoryBar.riseSpeed
        );
    }*/

    // Hover 
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHeld)
            isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isHeld)
            isHovered = false;
    }

    // Click to pick up 
    public void OnPointerClick(PointerEventData eventData)
    {
        // left click — pick up item
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!isHeld && !inputManager.isHoldingItem)
            {
                PickUp();
            }
        }
    }

    public void PickUp()
    {
        isHeld = true;
        isHovered = false;
        transform.localPosition = raisedPosition; // snap to raised while held
        inputManager.OnItemPickedUp(item, this);
    }

    // Called by PlayerInputManager on right click cancel
    public void ResetPosition()
    {
        isHeld = false;
        isHovered = false;
    }

    // Called when item is consumed (single use)
    public void ConsumeSlot()
    {
        isHeld = false;
        inventoryBar.OnItemConsumed(item);
    }
}