using UnityEngine;
using DG.Tweening;
using static RuntimeSettings;

public class NPCController : NPC
{
    [SerializeField] private Transform _targetPosition;

    public Tween WalkToTarget()
    {
        return transform.DOMove(_targetPosition.position, defaultTweenDuration).SetEase(Ease.InOutSine);
    }
}

