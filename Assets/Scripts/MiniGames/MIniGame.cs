using UnityEngine;

public abstract class MiniGame : MonoBehaviour
{

    [Header("Sus Meter Settings")]
    public float SusMeter = 0f; // Current value of the sus meter
    [SerializeField] private GameObject susMeterUI; // Reference to the sus meter UI GameObject

    [Header("Alert Settings")]
    public bool isAlertActive = false;
    public float alertDuration = 5f; // Duration for which the alert is active
    public float alertCooldown = 10f; // Cooldown before the next alert can be sent
    [SerializeField] private GameObject alertIcon; // Reference to the alert icon GameObject

    [Header("Budget Manager Settings")]
    [SerializeField] private BudgetManager budgetManager; // Reference to the BudgetManager script
    private int moneyGained = 30; // Amount of money gained on win
    private int moneyLost = 15; // Amount of money lost on loss



    public void SendPlayerAlert()
    {
        isAlertActive = true;
        // Implement logic to send an alert to the player
        Debug.Log("Player alert sent.");
    }

    public void UpdateSusMeter(float value)
    {
        SusMeter += value;
        // Implement logic to update the sus meter UI
        Debug.Log($"Sus Meter updated: {SusMeter}");
    }

    public void OnWin()
    {
        budgetManager.AddBudget(moneyGained);
    }
    public void OnLoss()
    {
        budgetManager.AddBudget(-moneyLost);
    }
}
