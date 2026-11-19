using System.Collections;
using UnityEngine;

public class WirePart : BombPart
{
    [SerializeField] Wire[] wires;

    [SerializeField] bool inOrder = false;

    private int current = 0;
    public int wiresToCut;

    public override bool OnItemUsed(ItemActionType[] itemActions)
    {
        var ctx = new UseContext { itemActions = itemActions, elements = wires };

        if (!UseBase(ref ctx))
        {
            return false;
        }

        int elementID = ctx.hoveredIndex;

        if ((elementID != current && inOrder) || (wires[elementID].dontCut))
        {
            timer.RegisterStrike();
            return false;
        }

        wires[elementID].RemoveHighlight();
        wires[elementID].isCut = true;
        current++;

        if (current >= wiresToCut)
        {
            Solve();
        }
        return true;
    }
}
