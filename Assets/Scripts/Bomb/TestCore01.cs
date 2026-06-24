using UnityEngine;

public class TestCore01 : BombPart
{
    [SerializeField] Animator anim;
    public Material disabledMaterial;
    public MeshRenderer render;

    //I think, that we can change it from void to bool, and if everything goes according to plan, it returns true, and item is consumed, else it returns false and item is not consumed
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
        RemoveHighlight();
        Solve();
        return true;
    }

    protected override void Solve()
    {
        if (isSolved)
        {
            return;
        }
        isSolved = true;
        render.material = disabledMaterial;
        anim.SetTrigger("Defused");
        bomb.TriggerDefused();
    }

    protected override void OnWrongItem()
    {
        StrikeSystem.AddStrike();
    }

    public override void Open()
    {
        isLocked = false;
        anim.SetTrigger("Open");
        lockAnim.SetBool("IsLocked", false);
    }

    protected override void Update()
    {

    }
}
