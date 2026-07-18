using UnityEngine;
using static RuntimeSettings;
using DG.Tweening;

public class CabinetController : MonoBehaviour
{   
    private Vector3 _defaultDrawerPosition = new Vector3(0.536f,0.0249f,1.623f);
    private Vector3 _finalDrawerPosition = new Vector3(0.536f,0.0249f,1.203f);

    public Tween Close()
    {
        return transform.DOMove(_defaultDrawerPosition, defaultTweenDuration).SetEase(Ease.InOutSine); 
    }
    
    public Tween Open()
    {
        return transform.DOMove(_finalDrawerPosition, defaultTweenDuration).SetEase(Ease.InOutSine);
    }
}
