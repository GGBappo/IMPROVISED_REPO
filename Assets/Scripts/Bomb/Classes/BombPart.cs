using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class BombPart : MonoBehaviour
{
    [Header("Base")]
    [Space(5)]
    //[SerializeField] protected BombManager bomb;
    [SerializeField] protected BombFragmentManager fragment;
    public ItemActionType[] compatibileItems;
    //public string partName;
    //public string hint;
    public bool isSolved;
    public bool dontNeedTool;
    //public bool countsToBomb;
    //public SpecialSolve sSolver;
    
    [Space(10)]

    [Header("Locks")]
    [Space(5)]
    
    //public BombPart[] toUnlockParts;
    public bool selfLocked = false;
    [SerializeField] protected Animator lockAnim;
    //public ItemType[] compatibileItems;
    public bool isLocked { get; protected set; } = true;
    [Space(10)]

    [Header("Highlights")]
    [Space(5)]
    public bool isHighlighted;
    [SerializeField] protected bool highlightable;
    //Tmp
    public GameObject highlight;
    [Space(10)]

    [Header("Events")]
    [Space(5)]
    public UnityEvent onPartSolved;
    [Space(5)]
    public UnityEvent onPartUnlocked;
    [Space(5)]
    public UnityEvent onPartWrongItem;
    [Space(10)]

    [Header("Timer System")]
    [Space(5)]
    private BombTimer timer;
    [SerializeField] private bool sendStrikeOnWrongItem = true;
    [Space(10)]

    [Header("Task System")]
    [Space(5)]
    private Tasks tasks;

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
