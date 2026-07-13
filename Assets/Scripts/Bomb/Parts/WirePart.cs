using System.Collections;
using UnityEngine;

public class WirePart : BombPart
{
    [SerializeField] Wire[] wires;

    [SerializeField] bool inOrder = false;

    private int current = 0;
    public int wiresToCut;

    public override bool OnItemUsed(ItemActionType type)
    {
        int elementID = 0;

        if (!UseBase(wires, type, ref elementID))
        {
            return false;
        }

        
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
