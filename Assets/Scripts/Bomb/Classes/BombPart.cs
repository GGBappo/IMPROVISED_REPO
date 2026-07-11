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


    private BombTimer timer;

    [Tooltip("On Awake, adds the Strike as the listener of the onPartWrongItem")]
    [SerializeField] private bool sendStrikeOnWrongItem = true;


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
        tasks = FindAnyObjectByType<Tasks>();

        if (sendStrikeOnWrongItem)
        {
            onPartWrongItem.AddListener(timer.RegisterStrike);
        }
    }
}
