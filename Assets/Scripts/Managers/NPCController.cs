using UnityEngine;
using DG.Tweening;
using static RuntimeSettings;

public class NPCController : NPC
{
    [SerializeField] private Transform _positionOne;
    [SerializeField] private Transform _positionTwo;

    public Tween WalkToPlayer()
    {
        return transform.DOMove(_positionTwo.position, defaultTweenDuration).SetEase(Ease.InOutSine);
    }
    public Tween WalkAwayFromPlayer()
    {
        return transform.DOMove(_positionOne.position, defaultTweenDuration).SetEase(Ease.InOutSine);
    }
}

