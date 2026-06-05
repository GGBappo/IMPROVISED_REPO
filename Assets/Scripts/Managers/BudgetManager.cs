using UnityEngine;

public class BudgetManager : MonoBehaviour
{
    private float startingBudget = 1000f;

    private float currentMoney;
    private Inventory inventory;

    public bool TryBuy(Item_SO item)
    {
        if (CanAfford(item.cost) && !IsInventoryFull())
        {
            currentMoney -= item.cost;   // reads cost directly from SO
            inventory.Add(item);
            return true;
        }
        return false;
    }

    public void SellItem(Item_SO item)
    {
        currentMoney += item.sellValue;  // reads sellValue directly from SO
        inventory.Remove(item);
    }

    public bool CanAfford(int cost)
    {
        return currentMoney >= cost;
    }
}
