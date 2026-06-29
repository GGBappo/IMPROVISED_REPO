using UnityEngine;

public class CoreManager : BombFragmentManager
{
    [SerializeField] Animator anim;
    public virtual void Open()
    {
        Unlock();
        anim.SetTrigger("Open");
        lockAnim.SetBool("IsLocked", false);
    }
}
