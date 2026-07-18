using UnityEngine;
using DG.Tweening;
using static RuntimeSettings;

public class NPCController : MonoBehaviour
{
    [SerializeField] private Transform _targetPosition;

    public Tween WalkToTarget()
    {
        return transform.DOMove(_targetPosition.position, defaultTweenDuration).SetEase(Ease.InOutSine);
    }
}

