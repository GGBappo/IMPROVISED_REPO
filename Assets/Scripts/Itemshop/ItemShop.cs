using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemShop : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private TextMeshProUGUI itemDescription;

    //gets the Item_SO scriptable object and sets the values of the item in the shop
    public void SetValues(Item_SO item)
    {
        itemImage.sprite = item.itemSprite;
        itemName.text = item.itemName;
        itemPrice.text = item.cost.ToString();
        itemDescription.text = item.hint;
    }

    public void OnButtonClick()
    {

    }
}
