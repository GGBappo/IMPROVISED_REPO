using System;
using System.Collections.Generic;
using UnityEngine;

public class ClothingManager : MonoBehaviour
{
    [Serializable]
    public class ClosetEntry
    {
        public GameObject visual;   // the model shown in the closet
        public Clothing_SO data;    // name, lore, buff
    }

    [Tooltip("Entry 0 must be the base outfit (no buffs).")]
    [SerializeField] private List<ClosetEntry> outfits = new List<ClosetEntry>();

    private int currentIndex;

    public Clothing_SO Current => outfits[currentIndex].data;
    public bool IsCurrentEquipped => GameSessionData.equippedClothing == Current;

    // A separate view script (text, button label) subscribes to this
    public event Action<Clothing_SO> OnOutfitChanged;

    private void Start()
    {
        if (outfits.Count == 0)
        {
            Debug.LogWarning("ClothingManager has no outfits assigned.", this);
            return;
        }

        // Nothing equipped yet? Start in the base outfit.
        if (GameSessionData.equippedClothing == null)
            GameSessionData.equippedClothing = outfits[0].data;

        // Open the closet on whatever is currently worn.
        currentIndex = Mathf.Max(0, outfits.FindIndex(o => o.data == GameSessionData.equippedClothing));
        Refresh();
    }

    public void Next() => Step(1);
    public void Previous() => Step(-1);

    private void Step(int direction)
    {
        if (outfits.Count == 0) return;
        currentIndex = (currentIndex + direction + outfits.Count) % outfits.Count;
        Refresh();
    }

    // Browsing: changes what's SHOWN. Does not equip anything.
    private void Refresh()
    {
        for (int i = 0; i < outfits.Count; i++)
            outfits[i].visual.SetActive(i == currentIndex);

        OnOutfitChanged?.Invoke(Current);
    }

    // Equipping: the only place the closet writes to GameSessionData.
    public void EquipCurrent()
    {
        if (outfits.Count == 0) return;

        GameSessionData.equippedClothing = Current;
        Debug.Log("Equipped: " + Current.clothingName);
        OnOutfitChanged?.Invoke(Current); // lets the button switch to "Equipped"
    }
}