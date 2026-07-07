using System.Collections;
using UnityEngine;

public class ButtonCode : BombPart
{
    [SerializeField] BombButton[] buttons;
    [SerializeField] int[] code;
    public override bool OnItemUsed(ItemActionType type)
    {
        int elementID = 0;

        if (isLocked) { return false; }

        if (isSolved) { return false; }

        var hoverOverAnything = false;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].mouseHover)
            {
                hoverOverAnything = true;
                elementID = i; break;
            }
        }
        if (!hoverOverAnything) { return false; }

        buttons[elementID].anim.SetTrigger("Click");
        buttons[elementID].lamp.NextColor();
        buttons[elementID].RemoveHighlight();

        bool solved = true;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].lamp.current != code[i])
            {
                solved = false;
                break;
            }
        }

        if (solved)
        {
            Solve();
        }
        return true;
    }

    public void DisableElectricity()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].lamp.disabled = true;
            buttons[i].lamp.NextColor();
        }
        Solve();
    }
}
