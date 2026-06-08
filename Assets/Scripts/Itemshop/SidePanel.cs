using UnityEngine;

public class SidePanel : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private bool isOpen = false;

    private void OnValidate()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    public void TogglePanel()
    {
        if (!isOpen)
        {
            anim.SetTrigger("Show");
        }
        else
        {
            anim.SetTrigger("Hide");
        }

        isOpen = !isOpen;
    }
}