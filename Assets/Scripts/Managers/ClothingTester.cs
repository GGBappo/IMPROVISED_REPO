using UnityEngine;

// TEMPORARY: delete once real UI exists
public class ClosetTester : MonoBehaviour
{
    [SerializeField] private ClothingManager closet;

    private void Reset() => closet = GetComponent<ClothingManager>();

    private void OnEnable() => closet.OnOutfitChanged += LogOutfit;
    private void OnDisable() => closet.OnOutfitChanged -= LogOutfit;

    private void LogOutfit(Clothing_SO item)
    {
        Debug.Log($"Showing: {item.clothingName} | equipped: {closet.IsCurrentEquipped}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) closet.Next();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) closet.Previous();
        if (Input.GetKeyDown(KeyCode.Return)) closet.EquipCurrent();
    }
}