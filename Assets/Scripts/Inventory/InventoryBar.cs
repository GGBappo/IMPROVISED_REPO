// InventoryBar.cs
// Controls the bottom bar UI � hover rise, click to hold, right click to cancel

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

    public GameObject[] itemPositions;
    private GameObject itemPrefab;

    private void Start()
    {
        inventory.OnItemAdded += OnItemAdded;
        inventory.OnItemRemoved += OnItemRemoved;
    }


   /* private void RefreshBar()
    {
        // clear existing slots
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        slots.Clear();

        // rebuild from inventory
        foreach (Item_SO item in inventory.items)
        {
            SpawnSlot(item);
        }
    }*/

    private void OnItemAdded(Item_SO item)
    {
        OnItemBought(item);
    }

    private void OnItemRemoved(Item_SO item)
    {
        // find the slot holding this item and destroy it
        InventorySlot slot = slots.Find(s => s.item == item);

        if (slot != null)
        {
            slots.Remove(slot);
            Destroy(slot.gameObject);
            // no shifting � empty space stays until filled
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

    private GameObject FindOpenSlot()
    {
        for (int i = 0; i< itemPositions.Length; i++)
        {
            if (itemPositions[i].transform.childCount == 0)
            {
                // Found an open slot
                return itemPositions[i]; // give back the empty slot
            }
        }
        return null; // no open slots
    }

    private void OnItemBought(Item_SO item)
    {
        GameObject openSlot = FindOpenSlot();

        if (openSlot != null)
        {
            Debug.Log("Placing bought item in open slot: " + openSlot.name);
            // Instantiate the item prefab in the open slot
            //GameObject itemObj = Instantiate(itemPrefab, openSlot.transform);
            // Optionally, set the item data on the instantiated object here
            //itemObj.GetComponent<InventorySlot>().Initialize(item, this);
        }
        else
        {
            Debug.Log("No open slots available to place the bought item.");
        }
    }
}