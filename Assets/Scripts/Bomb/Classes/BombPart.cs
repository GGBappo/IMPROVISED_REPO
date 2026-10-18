using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base class for every bomb component.
/// Lifecycle: InitializePart() -> Unlock() (skipped while selfLocked) -> OnItemUsed() -> Solve().
/// Solving fires onPartSolved, which is the drag-and-drop wiring point for gating other parts,
/// fragments and listeners (see also AutoSolveListener / ChainLink).
/// </summary>
[AddComponentMenu("Improv/Bomb/Bomb Part")]
public abstract class BombPart : MonoBehaviour
{
    #region Fields

    [Tooltip("Reference to the parent BombFragmentManager")]
    [SerializeField] protected BombFragmentManager fragment;

    [NonReorderable]
    [Tooltip("Verbs this part accepts. An item works on this part when at least ONE of its actions matches ONE of these. Use Empty for bare-click parts.")]
    public ItemActionType[] compatibleItems;

    public bool isSolved { get; protected set; }

    [Tooltip("If True, you can interact with this part WITHOUT any item, by simply clicking it (e.g. Reveal components, switch banks).")]
    public bool interactByClick;

    [Tooltip("If True, part will not be unlocked the same moment the Fragment does. In order to unlock this part, you will need to trigger Unlock() (via UnityEvent, ChainLink or code).")]
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

    [Tooltip("On Initialize, adds the Strike as the listener of the onPartWrongItem")]
    [SerializeField] private bool sendStrikeOnWrongItem = true;

    #endregion

    /// <summary>
    /// The single interaction entry point.
    /// Receives the full set of verbs the used item provides, or [Empty] for a bare mouse click.
    /// The PART decides which verb applies — this is what lets multi-verb items (e.g. a Multitool)
    /// work on several different part types without any mode-picking UI.
    /// </summary>
    public abstract bool OnItemUsed(ItemActionType[] itemActions);

    #region Highlighting

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

    #endregion

    #region Solving / locking

    protected virtual void Solve()
    {
        if (highlightable)
        {
            RemoveHighlight();
        }
        isSolved = true;
        SilentLock();
        onPartSolved?.Invoke();
    }

    /// <summary>
    /// Public solve entry for AutoSolveListener, ChainLink and editor test buttons.
    /// </summary>
    public void ForceSolve()
    {
        if (!isSolved)
        {
            Solve();
        }
    }

    public virtual void Unlock()
    {
        isLocked = false;
        if (lockAnim != null) lockAnim.SetBool("IsLocked", isLocked);
        if (interactByClick && (compatibleItems == null || !compatibleItems.Contains(ItemActionType.Empty)))
        {
            compatibleItems = new ItemActionType[1];
            compatibleItems[0] = ItemActionType.Empty;
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

    #endregion

    #region Compatibility

    /// <summary>
    /// True when at least one of the offered verbs matches one of the part's compatibleItems.
    /// </summary>
    protected bool IsCompatible(ItemActionType[] itemActions)
    {
        if (itemActions == null || compatibleItems == null) return false;

        for (int i = 0; i < itemActions.Length; i++)
        {
            if (compatibleItems.Contains(itemActions[i]))
            {
                return true;
            }
        }
        return false;
    }

    #endregion

    #region UseBase

    /// <summary>
    /// Filled in by concrete parts before calling UseBase. Leave a field null to skip that check.
    /// </summary>
    protected struct UseContext
    {
        /// <summary> Verbs offered by the used item. Null skips the compatibility check. </summary>
        public ItemActionType[] itemActions;
        /// <summary> Sub-elements that must be hovered for the interaction to count (wires, symbols...). Null skips the hover check. </summary>
        public PartElement[] elements;
        /// <summary> After UseBase, holds the index of the hovered element (only meaningful when elements were passed). </summary>
        public int hoveredIndex;
    }

    /// <summary>
    /// Single guarded entry that replaces the old overload family:
    /// checks locked/solved state, element hover (optional) and item compatibility (optional).
    /// Fires onPartWrongItem when the item doesn't match. Returns false when any check fails.
    /// </summary>
    protected bool UseBase(ref UseContext ctx)
    {
        if (isLocked) { return false; }
        if (isSolved) { return false; }

        if (ctx.elements != null)
        {
            var hoverOverAnything = false;
            for (int i = 0; i < ctx.elements.Length; i++)
            {
                if (ctx.elements[i].mouseHover && !ctx.elements[i].disabled)
                {
                    hoverOverAnything = true;
                    ctx.hoveredIndex = i;
                    break;
                }
            }
            if (!hoverOverAnything) { return false; }
        }

        if (ctx.itemActions != null && !IsCompatible(ctx.itemActions))
        {
            onPartWrongItem?.Invoke();
            return false;
        }

        return true;
    }

    #endregion
}
