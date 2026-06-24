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
        }
        else
        {
            isSolved = true;
        }
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    protected override void OnWrongItem()
    {
        StrikeSystem.AddStrike();
    }
}
