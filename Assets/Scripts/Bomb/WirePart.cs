using System.Collections;
using UnityEngine;

public class WirePart : BombPart
{
    [Space(10)]
    [Header("Wire Part")]
    [Space(5)]
    [SerializeField] Wire[] wires;
    [SerializeField] bool inOrder = false;

    private int current = 0;
    public int wiresToCut;

    public override bool OnItemUsed(ItemActionType type)
    {
        int elementID = 0;

        if (isLocked) { return false; }

        if (isSolved) { return false; }

        var hoverOverAnything = false;
        for (int i = 0; i < wires.Length; i++)
        {
            if (wires[i].mouseHover && !wires[i].isCut)
            {
                hoverOverAnything = true;
                elementID = i; break;
            }
        }
        if (!hoverOverAnything) { return false; }
        
        if ((elementID != current && inOrder) || (wires[elementID].dontCut) || (!IsCompatibile(type)))
        {
            onPartWrongItem?.Invoke();
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
