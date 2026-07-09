using UnityEngine;

public class SimpleItemPart : BombPart
{
    [SerializeField] bool destroyOnSolve/*, more different bools*/;
    [SerializeField] Animator solveAnim;


    public override bool OnItemUsed(ItemActionType type)
    {
        if (!UseBase(type))
        {
            return false;
        }

        Solve();
        return true;
    }

    protected override void Solve()
    {
        base.Solve();

        if (solveAnim != null)
        {
            solveAnim.SetTrigger("Solve");
        }
        if (destroyOnSolve)
        {
            gameObject.SetActive(false);
        }
    }
}
