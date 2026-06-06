using UnityEngine;

public class ItemShopPanel : MonoBehaviour
{
    public Shop_SO shop;
    public ItemShop itemPrefab; // Prefab for displaying items in the shop
    public Transform itemsParent; // Parent transform for instantiated item prefabs

    private void Start()
    {
        foreach (var item in shop.shopItems)
        {
            ItemShop itemGO = Instantiate(itemPrefab, itemsParent);
            if (itemGO != null)
            {
                itemGO.SetValues(item);
            }
        }
    }
}
