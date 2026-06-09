using UnityEngine;

public class PanelPart : BombPart
{
    public override bool OnItemUsed(string item)
    {
        if (isLocked) return false;

        if (isSolved) return false;

        if (!IsCompatibile(item))
        {
            //Also shouldnt consume Item
            Debug.Log("Strike");
            StrikeSystem.AddStrike();
            return false;
        }
        Solve();
        return true;
    }

    protected override void Solve()
    {
        bomb.OnPartSolved(this);
        //some animation of destroying before deleting
        Destroy(gameObject);
    }

    protected override void OnWrongItem()
    {
        StrikeSystem.AddStrike();
    }
}
