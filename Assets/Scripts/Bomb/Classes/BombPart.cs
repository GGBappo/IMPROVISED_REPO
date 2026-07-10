using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class BombPart : MonoBehaviour
{
    [Tooltip("Reference to the parent BombFragmentManager")]
    [SerializeField] protected BombFragmentManager fragment;


    [NonReorderable]
    [Tooltip("Action Types, compatibile with the Part")]
    public ItemActionType[] compatibileItems;

    public bool isSolved {  get; protected set; }

    [Tooltip("If True, you will be able to interact with this part ONLY without any item, so by simple mouse clicking")]
    public bool dontNeedTool;

    [Tooltip("Animator of the bomb itself(not the lock)")]
    [SerializeField] protected Animator bombAnim;

    [Tooltip("If True, will send 'Solve' trigger to the bombAnim")]
    [SerializeField] protected bool animateOnSolve;

    [Tooltip("If True, part will not be unlocked the same moment the Fragment does. In order to unlock this part, you will need to trigger Unlock()")]
    public bool selfLocked = false;

    [Tooltip("Animator of lock. Leave empty, if part isnt selfLocked")]
    [SerializeField] protected Animator lockAnim;
    public bool isLocked { get; protected set; } = true;


    [HideInInspector] public bool isHighlighted;

    [Tooltip("Will the part be highlighted, when mouse is over it(setting it to false dont interrupt children of being highlightable)")]
    [SerializeField] protected bool highlightable;

    [Tooltip("Temporary white plane, that imitates Highlight")]
    public GameObject highlight;


    [Tooltip("Triggers, when part is Solved")]
    public UnityEvent onPartSolved;

    [Tooltip("Triggers, when part is Unlocked")]
    public UnityEvent onPartUnlocked;

    [Tooltip("Triggers, when the wrong item is used on the part")]
    public UnityEvent onPartWrongItem;


    protected BombTimer timer;

    [Tooltip("On Awake, adds the Strike as the listener of the onPartWrongItem")]
    [SerializeField] private bool sendStrikeOnWrongItem = true;

    protected Tasks tasks;

    public abstract bool OnItemUsed(ItemActionType type);

    public virtual void Highlight()
    {
        if (!highlightable || isSolved || isLocked) return;
        highlight.SetActive(true);
        isHighlighted = true;
    }
    public virtual void RemoveHighlight()
    {
        if (!highlightable || isSolved || isLocked) return;
        highlight.SetActive(false);
        isHighlighted = false;
    }

    protected virtual void Solve()
    {
        isSolved = true;
        SilentLock(); 
        onPartSolved?.Invoke();
        if (tasks != null)
            tasks.TaskCompleted();

        if (animateOnSolve && bombAnim != null)
        {
            bombAnim.SetTrigger("Solve");
        }

    }

    protected bool IsCompatibile(ItemActionType type)
    {
        for (int i = 0; i < compatibileItems.Length; i++)
        {
            if (compatibileItems[i] == type)
            {
                return true;
            }
        }
        return false;
    }

    public virtual void Unlock()
    {
        isLocked = false;
        if(lockAnim != null) lockAnim.SetBool("IsLocked", isLocked);
        if (dontNeedTool && !compatibileItems.Contains(ItemActionType.Empty))
        {
            compatibileItems = new ItemActionType[1];
            compatibileItems[0] = ItemActionType.Empty;
        }
        onPartUnlocked?.Invoke();
    }
    public virtual void SilentLock()
    {
        isLocked = true;
    }

    public virtual void InitializePart()
    {
        timer = FindAnyObjectByType<BombTimer>();

        if (sendStrikeOnWrongItem)
        {
            onPartWrongItem.AddListener(timer.RegisterStrike);
        }
    }

    #region UseBase()

    /// <summary>
    ///Checks basic conditions, returns false, if any of them aren't met
    /// </summary>
    protected bool UseBase()
    {
        if (isLocked) { return false; }
        if (isSolved) { return false; }
        return true;
    }

    /// <summary>
    ///Checks basic conditions including checking if any element is hovered, returns false, if any of them aren't met
    /// </summary>
    protected bool UseBase(PartElement[] elements)
    {
        if (isLocked) { return false; }
        if (isSolved) { return false; }

        var hoverOverAnything = false;
        for (int i = 0; i < elements.Length; i++)
        {
            if (elements[i].mouseHover && !elements[i].disabled)
            {
                hoverOverAnything = true;
                break;
            }
        }
        if (!hoverOverAnything) { return false; }

        return true;
    }

    /// <summary>
    ///Checks basic conditions including checking item compatibility(invokes OnPartWrongItem), returns false, if any of them aren't met
    /// </summary>
    protected bool UseBase(ItemActionType itemType)
    {
        if (isLocked) { return false; }
        if (isSolved) { return false; }

        if (!IsCompatibile(itemType))
        {
            onPartWrongItem?.Invoke();
            return false;
        }

        return true;
    }

    /// <summary>
    ///Checks basic conditions including checking if any element is hovered and item compatibility(invokes OnPartWrongItem), returns false, if any of them aren't met
    /// </summary>
    protected bool UseBase(PartElement[] elements, ItemActionType itemType)
    {
        if (isLocked) { return false; }
        if (isSolved) { return false; }

        var hoverOverAnything = false;
        for (int i = 0; i < elements.Length; i++)
        {
            if (elements[i].mouseHover && !elements[i].disabled)
            {
                hoverOverAnything = true;
                break;
            }
        }
        if (!hoverOverAnything) { return false; }

        if (!IsCompatibile(itemType))
        {
            onPartWrongItem?.Invoke();
            return false;
        }

        return true;
    }

    /// <summary>
    ///Checks basic conditions including checking if any element is hovered, returns false, if any of them aren't met. Addtionally changes WhatIsHovered to the index of the hovered element
    /// </summary>
    protected bool UseBase(PartElement[] elements, ref int whatIsHovered)
    {
        if (isLocked) { return false; }
        if (isSolved) { return false; }

        var hoverOverAnything = false;
        for (int i = 0; i < elements.Length; i++)
        {
            if (elements[i].mouseHover && !elements[i].disabled)
            {
                hoverOverAnything = true;
                whatIsHovered = i;
                break;
            }
        }
        if (!hoverOverAnything) { return false; }

        return true;
    }

    /// <summary>
    ///Checks basic conditions including checking if any element is hovered and item compatibility(invokes OnPartWrongItem), returns false, if any of them aren't met. Addtionally changes WhatIsHovered to the index of the hovered element
    /// </summary>
    protected bool UseBase(PartElement[] elements, ItemActionType itemType, ref int whatIsHovered)
    {
        if (isLocked) { return false; }
        if (isSolved) { return false; }

        var hoverOverAnything = false;
        for (int i = 0; i < elements.Length; i++)
        {
            if (elements[i].mouseHover && !elements[i].disabled)
            {
                hoverOverAnything = true;
                whatIsHovered = i;
                break;
            }
        }
        if (!hoverOverAnything) { return false; }

        if (!IsCompatibile(itemType)) 
        {
            onPartWrongItem?.Invoke();
            return false; 
        }

        return true;
    }
    #endregion
}
