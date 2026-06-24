using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;

public abstract class BombPart : MonoBehaviour
{
    [Space(10)]
    [Header("Base")]
    [SerializeField] protected BombManager bomb;
    public string[] compatibileItems;
    public string partName;
    public string hint;
    public bool isSolved;
    public bool dontNeedTool;
    public bool countsToBomb;
    public SpecialSolve sSolver;
    [Space(10)]
    [Header("Locks")]
    public BombPart[] toUnlockParts;
    public bool isLocked;
    [SerializeField] protected Animator lockAnim;
    //public ItemType[] compatibileItems;
    [Space(10)]
    [Header("Highlights")]
    public bool isHighlighted;
    [SerializeField] protected bool highlightable;
    //Tmp
    public GameObject highlight;


    public abstract bool OnItemUsed(string item);

    //I dont think we need them to be Overriden, but it could depend on the part, so i change them to virtual
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

    protected abstract void Solve();

    protected abstract void OnWrongItem();

    protected virtual bool IsCompatibile(string item)
    {
        for (int i = 0; i < compatibileItems.Length; i++)
        {
            if (compatibileItems[i] == item)
            {
                return true;
            }
        }
        return false;
    }

    //Most parts need to call base.Update() when is overriden
    protected virtual void Update()
    {
        int x = 0;
        for (int i = 0; i < toUnlockParts.Length; i++)
        {
            if (toUnlockParts[i].isSolved)
            {
                x++;
            }
        }
        if (x >= toUnlockParts.Length)
        {
            isLocked = false;
        }
        else
        {
            isLocked = true;
        }

        if (lockAnim != null)
        {
            lockAnim.SetBool("IsLocked", isLocked);
        }
    }
    public virtual void SpecialSolve(int id)
    {

    }

    public virtual void Open()
    {
        //Special Unlock
    }
}
