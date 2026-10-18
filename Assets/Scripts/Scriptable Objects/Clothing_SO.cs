using UnityEngine;

public enum BuffType { Time, Money, Sus, None }

[CreateAssetMenu(fileName = "New Clothing", menuName = "Clothing/New Clothing")]
public class Clothing_SO : ScriptableObject
{
    public string clothingName;
    [TextArea] public string lore;
    [TextArea] public string clothingBuffText; // UI only

    public BuffType buffType;
    public float buffAmount;

    public void Apply(PlayerStats stats)
    {
        switch (buffType)
        {
            case BuffType.Time: stats.IncreaseTime((int)buffAmount); break;
            case BuffType.Money: stats.IncreaseMoney((int)buffAmount); break;
            case BuffType.Sus: stats.UpdateSusStat(buffAmount); break;
            case BuffType.None: break;
        }
    }
}