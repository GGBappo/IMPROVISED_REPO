using UnityEngine;

public class SpecialGiver : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Rigidbody itemBody;
    [SerializeField] InteractableItem item;

    private void Start()
    {
        itemBody.constraints = RigidbodyConstraints.FreezeAll;
        item.enabled = false;
    }

    public void RemoveConstraints()
    {
        itemBody.constraints = RigidbodyConstraints.None;
        item.enabled = true;
    }

    public void Give()
    {
        anim.SetTrigger("Give");
    }


}
