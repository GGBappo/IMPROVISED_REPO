using UnityEngine;

/// <summary>
/// The recurring shop functions. Generic items share one FUNCTION but exist as
/// different SKINS per setting (a rubber duck here, a tire piece there) —
/// mechanically identical, visually per-setting. Everything else is setting-exclusive.
/// </summary>
public enum GenericItemFunction { None, Insulator, Multitool, HeatSource }

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Item Data")]
public class Item_SO : ScriptableObject
{
    [Header("Function (what it does)")]
    [TextArea(3, 10)]
    [Tooltip("Verbs this item can fulfill. Multiple entries make a multi-tool (e.g. Cut + Open + Unscrew). This is the item's FUNCTION — the prefab is just the skin.")]
    public ItemActionType[] actions = new ItemActionType[0];

    [Header("Classification")]
    [Tooltip("True ONLY for the recurring generic functions (Insulator, Multitool, Heat Source). Generic items are shop-purchasable; everything else is setting-exclusive, delivered in-bomb via SpecialGiver, and never appears in a shop.")]
    public bool isGeneric;

    [Tooltip("Which generic function this item performs. Only meaningful when isGeneric is on; used to stop two skins of the same function appearing in one shop.")]
    public GenericItemFunction genericFunction;

    [Tooltip("Which setting/level this skin belongs to (e.g. 'Bakery'). Empty = universal/legacy skin.")]
    public string skinSetting = "";

    [Header("Economy & Presentation")]
    public Sprite itemSprite; // Sprite representing the item
    public InteractableItem prefab; // GameObject representing the item in the world
    public string itemName; // Name of the item
    public float cost; // Cost of the item
    public float sellValue; // Sell value of the item
    public string hint; // Item hint
    public bool isSingleUse;

    public float hoverHeight = 1.432f;
    public float hoverRot = 1.757f;

    public float dragHeight;
    public float dragRot;
}
