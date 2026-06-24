using System.Collections;
using UnityEngine;

public class WirePart : BombPart
{
    [SerializeField] Wire[] wires;
    [SerializeField] bool inOrder = false;
    [SerializeField] private BombTimer timer;
    [SerializeField] private Tasks taskSystem;

    private int current = 0;
    public int wiresToCut;

    private void Awake()
    {
   
    }
    //I think, that we can change it from void to bool, and if everything goes according to plan, it returns true, and item is consumed, else it returns false and item is not consumed
    public override bool OnItemUsed(string item)
    {
        int cw = 0;

        if (isLocked) { return false; }

        if (isSolved) { return false; }

        var hoverOverAnything = false;
        for (int i = 0; i < wires.Length; i++)
        {
            if (wires[i] == BombHoveringManager.hoveredPartOfPart && !wires[i].isCut)
            {
                hoverOverAnything = true;
                cw = i; break;
            }
        }
        if (!hoverOverAnything) { return false; }
        
        if ((cw != current && inOrder) || (wires[cw].dontCut) || (!IsCompatibile(item)))
        {
            //Also shouldnt consume Item
            OnWrongItem();
            return false;
        }

        wires[cw].RemoveHighlight();
        wires[cw].isCut = true;
        current++;

        if (current >= wiresToCut)
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
        timer.RegisterStrike();
    }
}
