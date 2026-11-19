using UnityEngine;

//An enumeration to define the type of buff a clothing item provides.
public enum BuffType { Time, Money, Sus, None }

[CreateAssetMenu(fileName = "New Clothing", menuName = "Clothing/New Clothing")]
public class Clothing_SO : ScriptableObject
{
    [SerializeField] private bool IsLocked;
    public bool ClothingState => IsLocked;


    public GameObject visualPrefab;
    [Tooltip("Artwork shown on the closet hook.")]
    public Sprite closetSprite;
    public string clothingName;
    [TextArea] public string DescriptionText;

    public BuffType buffType;
    public float buffAmount;

    // This method applies the buff to the player's stats based on the buff type and amount.
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
