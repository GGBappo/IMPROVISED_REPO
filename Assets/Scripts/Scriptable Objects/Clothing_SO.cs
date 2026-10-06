using UnityEngine;

[CreateAssetMenu(fileName = "New Clothing", menuName = "Clothing/New Clothing")]
public class Clothing_SO : ScriptableObject
{
    [SerializeField] private string clothingName;
    [SerializeField] private string clothingBuffText;
    [SerializeField] private bool isEquipped;
}
