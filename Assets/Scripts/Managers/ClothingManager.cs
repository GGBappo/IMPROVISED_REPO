using System;
using System.Collections.Generic;
using UnityEngine;

public class ClothingManager : MonoBehaviour
{
    [Serializable]
    // A simple wrapper for a Clothing_SO reference, so we can use a List in the inspector.
    public class ClosetEntry
    {
        public Clothing_SO data;
    }


    [Tooltip("Assign four outfits in hook order. Entry 0 is the default outfit.")]
    [SerializeField] private List<ClosetEntry> outfits = new List<ClosetEntry>();
    [SerializeField] private ClosetDisplay closetDisplay;
    [SerializeField] private ClosetRotator closetRotator;

    // The index of the currently selected outfit in the outfits list.
    private int currentIndex;
    // A flag to indicate whether the manager has been initialized and is ready for interaction.
    private bool initialized;
    // A flag to indicate whether the manager is currently handling a navigation action (Next or Previous).
    private bool handlingNavigation;

    public Clothing_SO Current => GetClothing(currentIndex);
    public bool IsCurrentEquipped =>
        Current != null && GameSessionData.equippedClothing == Current;

    // Events to notify other components when the outfit changes or when navigation buttons are clicked.
    public event Action<Clothing_SO> OnOutfitChanged;
    public event Action OnNextClicked;
    public event Action OnPreviousClicked;

    private void Start()
    {
        if (outfits.Count != 4 || closetDisplay == null || closetRotator == null)
        {
            Debug.LogError("Assign four outfits, a Closet Display, and a Closet Rotator.", this);
            return;
        }
        
        // Get the default outfit (first in the list).
        Clothing_SO defaultOutfit = GetClothing(0);

        // If no outfit is currently equipped and the default outfit is valid and not locked, equip it.
        if (GameSessionData.equippedClothing == null &&
            defaultOutfit != null && !defaultOutfit.ClothingState)
        {
            GameSessionData.equippedClothing = defaultOutfit;
        }

        // If the equipped outfit isn't in this closet, open on the first hook.
        Clothing_SO equipped = GameSessionData.equippedClothing;
        currentIndex = equipped == null ? 0 : Mathf.Max(0,
            outfits.FindIndex(entry => entry != null && entry.data == equipped));

        closetRotator.SetInitialRotation(currentIndex);
        closetDisplay.RefreshAllHooks();

        initialized = true;
        Refresh();
    }

    public Clothing_SO GetClothing(int index)
    {
        if (index < 0 || index >= outfits.Count)
            return null;

        return outfits[index]?.data;
    }

    public void Next()
    {
        Step(1);
    }

    public void Previous()
    {
        Step(-1);
    }

    private void Step(int direction)
    {
        if (!CanInteract())
            return;

        handlingNavigation = true;
        try
        {
            // Adding Count before taking the remainder wraps Previous from 0 to 3.
            currentIndex = (currentIndex + direction + outfits.Count) % outfits.Count;

            // The rotator listens for these events and starts the matching quarter-turn.
            if (direction > 0)
                OnNextClicked?.Invoke();
            else
                OnPreviousClicked?.Invoke();

            Refresh();
        }
        finally
        {
            handlingNavigation = false;
        }
    }

    public void EquipCurrent()
    {
        if (!CanInteract())
            return;

        Clothing_SO selected = Current;
        if (selected == null || selected.ClothingState || IsCurrentEquipped)
            return;

        GameSessionData.equippedClothing = selected;
        OnOutfitChanged?.Invoke(selected);
    }

    private bool CanInteract()
    {
        return initialized && isActiveAndEnabled && !handlingNavigation &&
            closetRotator != null && closetRotator.isActiveAndEnabled &&
            !closetRotator.IsRotating;
    }

    private void Refresh()
    {
        closetDisplay.SetCurrentHookIndex(currentIndex);
        OnOutfitChanged?.Invoke(Current);
    }
}