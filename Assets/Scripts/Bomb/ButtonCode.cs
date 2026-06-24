using System.Collections;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class ButtonCode : BombPart
{
    [SerializeField] Button[] buttons;
    [SerializeField] int[] code;
    public override bool OnItemUsed(string item)
    {
        int cb = 0;

        if (isLocked) { return false; }

        if (isSolved) { return false; }

        var hoverOverAnything = false;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == BombHoveringManager.hoveredPartOfPart)
            {
                hoverOverAnything = true;
                cb = i; break;
            }
        }
        if (!hoverOverAnything) { return false; }

        buttons[cb].anim.SetTrigger("Click");
        buttons[cb].lamp.NextColor();
        buttons[cb].RemoveHighlight();
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

    protected override void Solve()
    {
        if (isSolved)
        {
            return;
        }

        if (sSolver != null)
        {
            sSolver.Solve();
        }
        if (countsToBomb)
        {
            bomb.OnPartSolved(this);
        }else
        {
            isSolved = true;
        }

    }

    protected override void OnWrongItem()
    {
        StrikeSystem.AddStrike();
    }

    public override void SpecialSolve(int id)
    {
        if (id == 0)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].lamp.disabled = true;
                buttons[i].lamp.NextColor();
            }
            Solve();
        }
    }
}
