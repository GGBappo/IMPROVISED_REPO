using System.Collections;
using UnityEngine;

public abstract class PartOfPart : MonoBehaviour
{
    public bool mouseHover {  get; private set; } 
    [SerializeField] BombPart parentPart;
    [SerializeField] protected bool highlightable;

    //If PlayerInputManager could set hover and Highlight, it would be nice
    public void SetHover(bool value)
    {
        mouseHover = value;
    }

    public virtual void Highlight()
    {
        //Adds Highlight (if Highlightable)
    }

    public virtual void RemoveHighlight()
    {
        //If Highlightable - Remove Highlight (who could have guessed?)
    }
}
