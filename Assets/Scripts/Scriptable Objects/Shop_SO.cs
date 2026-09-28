using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShop", menuName = "Shop/Shop Data")]
public class Shop_SO : ScriptableObject
{
    [Tooltip("The items this level's shop sells. GENERIC items only — setting-exclusive items are delivered in-bomb via SpecialGiver and must never appear here (enforced below and by Improv/Validate All Bombs).")]
    public List<Item_SO> shopItems;

    // Editor-time enforcement: the Inspector itself protects the item economy.
    private void OnValidate()
    {
        if (shopItems == null) return;

        var seenFunctions = new HashSet<GenericItemFunction>();

        foreach (var item in shopItems)
        {
            if (item == null) continue;

            if (!item.isGeneric)
            {
                Debug.LogError($"[Shop] '{name}' lists setting-exclusive item '{item.itemName}'. Only generic items (Insulator/Multitool/Heat Source skins) may be sold. Deliver exclusive items in-bomb via SpecialGiver instead.", this);
            }
            else if (!seenFunctions.Add(item.genericFunction))
            {
                Debug.LogError($"[Shop] '{name}' lists more than one skin of generic function '{item.genericFunction}'. A level's shop carries exactly one skin per function.", this);
            }
        }
    }
}
