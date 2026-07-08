using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Item Data")]
public class Item_SO : ScriptableObject
{
    [TextArea(3, 10)]
    public Sprite itemSprite; // Sprite representing the item
    public InteractableItem prefab; // GameObject representing the item in the world
    public string itemName; // Name of the item
    public float cost; // Cost of the item
    public float sellValue; // Sell value of the item
    public string hint; // Array of item hints
    public bool isSingleUse;

    public float hoverHeight = 1.432f;
    public float hoverRot = 1.757f;

    public float dragHeight;
    public float dragRot;

    public bool IsCompatibleWith(BombPart part)
    {
        return true;
    }

    public bool Use(BombPart part)
    {
        return true;
    }
}
