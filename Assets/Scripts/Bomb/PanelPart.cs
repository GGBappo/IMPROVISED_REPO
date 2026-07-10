using UnityEngine;

public class PanelPart : BombPart
{
    public override bool OnItemUsed(ItemActionType type)
    {
        if (isLocked) return false;

        if (isSolved) return false;

        if (!IsCompatibile(type))
        {
            onPartWrongItem?.Invoke();
            return false;
        }
        Solve();
        return true;
    }

    protected override void Solve()
    {
        base.Solve();
        gameObject.SetActive(false);
    }
}
