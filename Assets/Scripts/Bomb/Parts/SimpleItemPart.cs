using UnityEngine;

/// <summary>
/// One-shot component: correct item (or bare click) -> solved. Optional solve animation / destroy.
/// </summary>
[AddComponentMenu("Improv/Bomb/Simple Item Part")]
public class SimpleItemPart : BombPart
{
    [SerializeField] bool destroyOnSolve/*, more different bools*/;
    [SerializeField] Animator solveAnim;


    public override bool OnItemUsed(ItemActionType[] itemActions)
    {
        var ctx = new UseContext { itemActions = itemActions };

        if (!UseBase(ref ctx))
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
