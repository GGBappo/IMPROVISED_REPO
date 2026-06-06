// Inventory.cs
// Holds the list of owned items and manages add/remove logic

using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [Header("Settings")]
    public int maxSize = 5;

    [Header("State")]
    public List<Item_SO> items = new List<Item_SO>();

    public event Action<Item_SO> OnItemAdded;
    public event Action<Item_SO> OnItemRemoved;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Called by BudgetManager after successful purchase
    public bool AddItem(Item_SO item)
    {
        if (IsFull())
        {
            Debug.Log("Inventory full");
            return false;
        }

        items.Add(item);
        OnItemAdded?.Invoke(item);
        return true;
    }

    // Called on sell or when single use item is consumed
    public void RemoveItem(Item_SO item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            OnItemRemoved?.Invoke(item);
        }
    }

    public bool IsFull() => items.Count >= maxSize;
    public bool Contains(Item_SO item) => items.Contains(item);
    public int CurrentSize() => items.Count;
}