using TMPro;
using UnityEngine;

public class BudgetManager : MonoBehaviour
{
    [SerializeField] private float startingBudget = 45f;
    [SerializeField] private float currentMoney;
    [SerializeField] private TextMeshProUGUI budgetText;
    public void Awake()
    {
        currentMoney = startingBudget;
        Debug.Log("You have " + currentMoney + " money to start with.");
        UpdateBudgetText();
    }
    public bool TryBuy(Item_SO item)
    {
        if (CanAfford(item.cost))
        {
            currentMoney -= item.cost;   // reads cost directly from SO
            UpdateBudgetText();
            return true;
        }
        UpdateBudgetText();
        return false;
    }

    public void SellItem(Item_SO item)
    {
        currentMoney += item.sellValue;  // reads sellValue directly from SO
        UpdateBudgetText();
    }

    public bool CanAfford(float cost)
    {
        return currentMoney >= cost;
    }

    public void CurrentMoney()
    {
        Debug.Log("Current money: " + currentMoney);
    }
    
    private void UpdateBudgetText()
    {
        budgetText.text = "$" + currentMoney.ToString();
    }

    public void AddBudget(int amount)
    {
        currentMoney += amount;
        UpdateBudgetText();
    }
}
