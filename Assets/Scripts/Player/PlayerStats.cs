using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private BudgetManager moneyStat;
    [SerializeField] private BombTimer timeStat;
    [SerializeField] private float susStat;

    private void Start()
    {
        var worn = GameSessionData.equippedClothing;
        if (worn != null) worn.Apply(this);
    }
    public void UpdateSusStat(float value)
    {
        susStat += value;
        Debug.Log("Sus Stat updated to: " + susStat);
    }
    public void IncreaseMoney(int amount)
    {
        moneyStat.AddBudget(amount);
    }
    public void IncreaseTime(int amount)
    {
        timeStat.AddTime(amount);
    }
}
