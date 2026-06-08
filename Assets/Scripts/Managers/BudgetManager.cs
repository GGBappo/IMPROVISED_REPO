using UnityEngine;

public class BudgetManager : MonoBehaviour
{
    [SerializeField] private float startingBudget = 45f;

    [SerializeField] private float currentMoney;
    private Inventory inventory;

    public void Awake()
    {
        currentMoney = startingBudget;
        Debug.Log("You have " + currentMoney + " money to start with.");
    }
    public bool TryBuy(Item_SO item)
    {
        if (CanAfford(item.cost))
        {
            currentMoney -= item.cost;   // reads cost directly from SO
            return true;
        }
        return false;
    }

    public void SellItem(Item_SO item)
    {
        currentMoney += item.sellValue;  // reads sellValue directly from SO
    }

    public bool CanAfford(float cost)
    {
        return currentMoney >= cost;
    }

    public void CurrentMoney()
    {
        Debug.Log("Current money: " + currentMoney);
    }
}
