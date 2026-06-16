using System.Collections;
using UnityEngine;

public abstract class PartOfPart : MonoBehaviour
{
    public bool mouseHover {  get; private set; } 
    [SerializeField] BombPart parentPart;
    [SerializeField] protected bool highlightable;
    //TMP
    public GameObject highlight;


    //If PlayerInputManager could set hover and Highlight, it would be nice
    public void SetHover(bool value)
    {
        mouseHover = value;
    }

    public virtual void Highlight()
    {
        if (!highlightable || parentPart.isSolved || parentPart.isLocked) return;
        highlight.SetActive(true);
    }

    public virtual void RemoveHighlight()
    {
        if (!highlightable || parentPart.isSolved || parentPart.isLocked) return;
        highlight.SetActive(false);
    }
}
