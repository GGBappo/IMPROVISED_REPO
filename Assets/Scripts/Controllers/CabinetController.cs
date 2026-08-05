using UnityEngine;
using static RuntimeSettings;
using DG.Tweening;

public class CabinetController : MonoBehaviour
{   
    private Vector3 _defaultDrawerPosition = new Vector3(0.511f,-0.06f,1.623f);
    private Vector3 _finalDrawerPosition = new Vector3(0.511f,-0.06f,1.203f);
    public bool drawerIsOpen = false;
    public BoxCollider fileSpawningArea;

    public Tween Close()
    {
        drawerIsOpen = false; 
        return transform.DOMove(_defaultDrawerPosition, defaultTweenDuration).SetEase(Ease.InOutSine);
    }
    
    public Tween Open()
    {
        drawerIsOpen = true;
        return transform.DOMove(_finalDrawerPosition, defaultTweenDuration).SetEase(Ease.InOutSine);
    }
    
}
