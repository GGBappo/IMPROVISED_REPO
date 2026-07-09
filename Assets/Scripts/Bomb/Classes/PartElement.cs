using System.Collections;
using UnityEngine;

public abstract class PartElement : MonoBehaviour
{
    public bool disabled;
    public bool mouseHover {  get; private set; } 
    [SerializeField] BombPart parentPart;
    [SerializeField] protected bool highlightable;
    //TMP
    public GameObject highlight;

    public void SetHover(bool value)
    {
        mouseHover = value;
    }
    public virtual void Highlight()
    {
        if (!highlightable || parentPart.isSolved || parentPart.isLocked || disabled) return;
        highlight.SetActive(true);
    }
    public virtual void RemoveHighlight()
    {
        if (!highlightable || parentPart.isSolved || parentPart.isLocked || disabled) return;
        highlight.SetActive(false);
    }
}
