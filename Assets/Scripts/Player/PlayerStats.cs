using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private BudgetManager moneyStat;
    [SerializeField] private BombTimer timeStat;
    [SerializeField] private float susStat;


    public void Awake()
    {
        moneyStat = GetComponent<BudgetManager>();
        timeStat = GetComponent<BombTimer>();
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
