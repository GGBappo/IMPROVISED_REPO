using UnityEngine;

public class TestCore01 : BombPart
{
    [SerializeField] Animator anim;
    public Material disabledMaterial;
    public MeshRenderer render;

    public override bool OnItemUsed(ItemActionType[] itemActions)
    {
        var ctx = new UseContext { itemActions = itemActions };

        if (!UseBase(ref ctx))
        {
            return false;
        }

        RemoveHighlight();
        Solve();
        return true;
    }

    protected override void Solve()
    {
        base.Solve();
        render.material = disabledMaterial;
        anim.SetTrigger("Defused");
    }
}
