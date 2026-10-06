using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private BudgetManager moneyStat;
    [SerializeField] private BombTimer timeStat;

    private void Start()
    {
        var worn = GameSessionData.equippedClothing;
        if (worn != null) worn.Apply(this);
    }
}
