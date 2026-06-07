using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class ItemShopPanel : MonoBehaviour
{
    public Shop_SO shop;
    public ItemShop itemPrefab; // Prefab for displaying items in the shop
    public Transform itemsParent; // Parent transform for instantiated item prefabs

    [SerializeField] private List<Transform> spawnPoints;

    private int currentSpawnPointIndex = 0;


    private void Awake()
    {
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
        //Create an instance of the bought item at the current spawn point
        InteractableItem instItem = Instantiate(item.prefab);
        instItem.transform.parent = spawnPoints[currentSpawnPointIndex];
        instItem.transform.localPosition = Vector3.zero; // Position the item at the spawn point

        currentSpawnPointIndex++;

        Debug.Log($"Item bought: {item.itemName}");
    }
}
