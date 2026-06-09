using System.Collections;
using UnityEngine;

public abstract class BombPart : MonoBehaviour
{
    public string partName;
    public bool isSolved;
    public bool isLocked;
    public string hint;
    //public ItemType[] compatibileItems;
    public bool isHighlighted;
    [SerializeField] protected BombManager bomb;
    [SerializeField] protected bool highlightable;
    //Tmp
    public GameObject highlight;


    public abstract bool OnItemUsed(string item);

    //I dont think we need them to be Overriden, but it could depend on the part, so i change them to virtual
    public virtual void Highlight()
    {
        //Adds Highlight (if Highlightable)
    }

    public virtual void RemoveHighlight()
    {
        if (!highlightable) return;
        highlight.SetActive(false);
    }

    protected abstract void Solve();

    protected abstract void OnWrongItem();

    //protected virtual bool IsCompatibile(string item)
    //{
    //    for (int i = 0; i < compatibileItems.Length; i++)
    //    {
    //        if (compatibileItems[i]. == item)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}
}
