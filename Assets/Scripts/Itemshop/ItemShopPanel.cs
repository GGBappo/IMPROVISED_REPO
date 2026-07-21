using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

//This class controls the item shop panel.
//It calls the Shop_SO to get the items to display 
//and it also handles the buying of items and spawning them in the world.
public class ItemShopPanel : MonoBehaviour
{
    public Shop_SO shop;
    public ItemShop itemPrefab; // Prefab for displaying items in the shop
    public Transform itemsParent; // Parent transform for instantiated item prefabs

    [SerializeField] private List<Transform> spawnPoints;
    //creates a table to keep track of the items that have been bought and their corresponding spawn points
    //Transform is the key, Item is the Value
    [SerializeField] private Dictionary<Transform, InteractableItem> purchasedItems = new Dictionary<Transform, InteractableItem>();
    [SerializeField] private Dictionary<string, bool> boughtItems = new Dictionary<string, bool>();
    [SerializeField] private BudgetManager budgetManager;

    private void Awake()
    {
        if(budgetManager == null)
        {
            //find the budget manager in the scene if it is not assigned in the inspector
            budgetManager = FindObjectOfType<BudgetManager>();
        }
        //for each time that is in the shop scriptable object,
        //it creates an itemGO and sets its values to be displayed in the shop panel
        foreach (var item in shop.shopItems)
        {
            ItemShop itemGO = Instantiate(itemPrefab, itemsParent);
            if (itemGO != null)
            {
                //sends values of the items and it self to the itemGO script to be used for displaying and buying items
                itemGO.SetValues(item, this);

                boughtItems.Add(item.itemName, false);
            }
        }
    }

    public void OnItemBought(Item_SO item)
    {        
        if(purchasedItems.Count >= 5 || boughtItems[item.itemName])
        {
            Debug.LogWarning("Cant buy this item");
            return;
        }

        bool canBuy = budgetManager.TryBuy(item);

        if (!canBuy)
        {
            Debug.LogWarning("Not enough budget to buy this item.");
            return;
        }
        else
        {
            //This does this for each spawn point,
            //but it only updates the table for the current spawn point index, so it is not really a problem
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                //Checks if its occupied
                if (purchasedItems.ContainsKey(spawnPoints[i]))
                {
                    //checks if there is a item in the spawn point
                    if (purchasedItems[spawnPoints[i]] == null)
                    {
                        //Create an instance of the bought item at the current spawn point
                        InteractableItem instItem = Instantiate(item.prefab, spawnPoints[i].position, Quaternion.identity, spawnPoints[i].transform);
                        //sets the data of the item
                        instItem.itemData = item;

                        //if it is, it updates the item in the table
                        purchasedItems[spawnPoints[i]] = instItem;

                        boughtItems[item.itemName] = true;
                        break;
                    }
                    else
                    {
                        //makes the for loop continue to the next check
                        continue;
                    }
                }
                //not occupied add item
                else
                {
                    //Create an instance of the bought item at the current spawn point
                    InteractableItem instItem = Instantiate(item.prefab, spawnPoints[i].position, Quaternion.identity, spawnPoints[i].transform);

                    //sets the data of the item
                    instItem.itemData = item;

                    //if it is not, it adds the item to the table with its corresponding spawn point
                    purchasedItems.Add(spawnPoints[i], instItem);

                    Debug.Log($"Item bought: {item.itemName}");

                    boughtItems[item.itemName] = true;

                    break;
                }
            }
        }
    }

    public void FreeSpawnPoint(InteractableItem item)
    {
        //ContainsValue is used to check if the item is in the table,
        //if it is, it finds the corresponding spawn point and sets it to null

            //find the spawn point that corresponds to the item and set it to null
            foreach (var kvp in purchasedItems)
            {
                if (kvp.Value == null) continue;

                if (kvp.Value.itemData.itemName == item.itemData.itemName)
                {
                    boughtItems[item.itemData.itemName] = false;
                    purchasedItems[kvp.Key] = null;
                    break;
                }
            }
    }
    public Vector3 GetSpawnPoint(InteractableItem item)
    {
        foreach (var kvp in purchasedItems)
        {
            if (kvp.Value == item)
                return kvp.Key.position;
        }

        Debug.LogWarning("No spawn point found for item: " + item.name);
        return item.transform.position;
    }

    //Used for when item is let go. Item when let go returns to spawnpoint in which it came from.
    public void ReturnToSpawnPoint(InteractableItem item)
    {
        //get the spawn point of the item and set its position to it
        
    }
}
