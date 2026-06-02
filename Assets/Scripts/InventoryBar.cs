// InventoryBar.cs
// Controls the bottom bar UI — hover rise, click to hold, right click to cancel

using System.Collections.Generic;
using UnityEngine;

public class InventoryBar : MonoBehaviour
{
    [Header("References")]
    public Inventory inventory;
    public PlayerInputManager inputManager;
    public Transform slotContainer;          // parent object holding all slots
    public GameObject slotPrefab;            // individual slot UI prefab

    [Header("Hover Settings")]
    public float riseAmount = 20f;           // how many pixels the item rises
    public float riseSpeed = 8f;             // how fast it rises

    private List<InventorySlot> slots = new List<InventorySlot>();

    private void Start()
    {
        inventory.OnItemAdded += OnItemAdded;
        inventory.OnItemRemoved += OnItemRemoved;
        RefreshBar();
    }

    private void RefreshBar()
    {
        // clear existing slots
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        slots.Clear();

        // rebuild from inventory
        foreach (UsableItem item in inventory.items)
        {
            SpawnSlot(item);
        }
    }

    private void SpawnSlot(Item_SO item)
    {
        GameObject obj = Instantiate(slotPrefab, slotContainer);
        InventorySlot slot = obj.GetComponent<InventorySlot>();
        slot.Initialize(item, this);
        slots.Add(slot);
    }

    private void OnItemAdded(Item_SO item)
    {
        SpawnSlot(item);
    }

    private void OnItemRemoved(Item_SO item)
    {
        // find the slot holding this item and destroy it
        InventorySlot slot = slots.Find(s => s.item == item);

        if (slot != null)
        {
            slots.Remove(slot);
            Destroy(slot.gameObject);
            // no shifting — empty space stays until filled
        }
    }

    // Called by PlayerInputManager when item is consumed (single use)
    public void OnItemConsumed(Item_SO item)
    {
        inventory.RemoveItem(item);
    }

    // Called by PlayerInputManager when right click cancels
    public void ReturnItemToBar(Item_SO item)
    {
        // item never left inventory list, just reset the slot visual
        InventorySlot slot = slots.Find(s => s.item == item);
        if (slot != null)
            slot.ResetPosition();
    }
}