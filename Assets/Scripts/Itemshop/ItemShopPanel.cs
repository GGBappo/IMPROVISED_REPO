using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices.ComTypes;
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
    [SerializeField] private BudgetManager budgetManager;

    private int currentSpawnPointIndex = 0;


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
            }
        }
    }

    public void OnItemBought(Item_SO item)
    {        
        if(currentSpawnPointIndex >= spawnPoints.Count)
        {
            Debug.LogWarning("No more spawn points available for bought items.");
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
            //Create an instance of the bought item at the current spawn point
            InteractableItem instItem = Instantiate(item.prefab);
            instItem.itemData = item;
            //instItem.transform.parent = spawnPoints[currentSpawnPointIndex];
            instItem.transform.localPosition = spawnPoints[currentSpawnPointIndex].position;

            currentSpawnPointIndex++;

            Debug.Log($"Item bought: {item.itemName}");
        }
    }
}
