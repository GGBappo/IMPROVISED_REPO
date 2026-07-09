using System;
using UnityEngine;

public class SymbolPuzzlePart : BombPart
{
    [SerializeField] private Symbol[] symbols;
    [SerializeField] private int[] code;

    public override bool OnItemUsed(ItemActionType type)
    {
        int elementID = 0;

        if (!UseBase(symbols, type, ref elementID))
        {
            return false;
        }

        symbols[elementID].anim.SetTrigger("Click");
        symbols[elementID].icon.NextColor();
        symbols[elementID].RemoveHighlight();

        bool solved = true;

        for (int i = 0; i < symbols.Length; i++)
        {
            if (symbols[i].icon.current != code[i])
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

}
