using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
//This class controls the item shop callingthe ItemShopPanel and using its
//functions to add the item to the Slots. We also call the Item_SO
//To set the values of the item in the shop and to add it to the inventory when bought.
public class ItemShop : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private TextMeshProUGUI itemDescription;

    // Store the currently displayed item so the button can add it without needing a parameter.
    public Item_SO currentItem;

    private int currentSpawnIndex = 0;
    private ItemShopPanel shopPanel;

    //gets the Item_SO scriptable object and sets the values of the item in the shop
    public void SetValues(Item_SO item, ItemShopPanel shop)
    {
        currentItem = item;
        itemImage.sprite = item.itemSprite;
        itemName.text = item.itemName;
        itemPrice.text = item.cost.ToString();
        itemDescription.text = item.hint;

        shopPanel = shop;
    }

    public void OnButtonClick()
    {
        shopPanel.OnItemBought(currentItem);
    }
}
