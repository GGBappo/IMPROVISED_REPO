using System.Collections;
using UnityEngine;

public class WirePart : BombPart
{
    [SerializeField] Wire[] wires;
    [SerializeField] bool inOrder = false;
    private int current = 0;

    //I think, that we can change it from void to bool, and if everything goes according to plan, it returns true, and item is consumed, else it returns false and item is not consumed
    public override bool OnItemUsed(string item)
    {
        if (isLocked) { return false; }

        if (isSolved) { return false; }

        var hoverOverAnything = false;
        for (int i = 0; i < wires.Length; i++)
        {
            if (wires[i].mouseHover && !wires[i].isCut)
            {
                hoverOverAnything = true;
            }
        }
        if (!hoverOverAnything) { return false; }
        
        if (!wires[current].mouseHover && inOrder)
        {
            //Also shouldnt consume Item
            StrikeSystem.AddStrike();
            return false;
        }

        wires[current].RemoveHighlight();
        wires[current].isCut = true;
        current++;

        if (current >= wires.Length)
        {
            Solve();
        }
        return true;
    }

    protected override void Solve()
    {
        bomb.OnPartSolved(this);
    }

    protected override void OnWrongItem()
    {
        StrikeSystem.AddStrike();
    }
}
