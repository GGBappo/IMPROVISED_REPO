using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShop", menuName = "Shop/Shop Data")]
public class Shop_SO: ScriptableObject
{
    public List<Item_SO> shopItems;
}

