using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Item Data")]
public class Item_SO : ScriptableObject
{
    [TextArea(3, 10)]
    public Sprite itemSprite; // Sprite representing the item
    public string itemName; // Name of the item
    public float cost; // Cost of the item
    public float sellValue; // Sell value of the item
    public string hint; // Array of item hints
    public string[] compatiblePartTypes; // Array of compatible part types for the item
    public bool isSingleUse;

    public bool IsCompatibleWith(BombPart part)
    {
        return true; 
    }

    public bool Use(BombPart part)  
    {
        return true;
    }
}

