using System.Collections;
using UnityEngine;

public abstract class BombPart : MonoBehaviour
{
    public string partName;
    public bool isSolved;
    public bool isLocked;
    public string hint;
    public ItemType[] compatibileItems;
    public bool isHighlighted;
    [SerializeField] protected BombManager bomb;
    [SerializeField] protected Highlightable highlightable;


    public abstract void /*bool*/ OnItemUsed(UsableItem item);

    //I dont think we need them to be Overriden, but it could depend on the part, so i change them to virtual
    public virtual void Highlight()
    {
        //Adds Highlight (if Highlightable)
    }

    public virtual void RemoveHighlight()
    {
        //If Highlightable - Remove Highlight (who could have guessed?)
    }

    protected abstract void Solve();

    protected abstract void OnWrongItem();
}
