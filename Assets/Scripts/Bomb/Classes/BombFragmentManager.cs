using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BombFragmentManager : MonoBehaviour
{
    [SerializeField] BombManager bomb;
    [SerializeField] BombPart[] parts;
    [SerializeField] int toSolveParts;
    protected int solvedParts;
    [Space(10)]

    [Header("Events")]
    [Space(5)]
    public UnityEvent onFragmentSolved;
    [Space(5)]
    public UnityEvent onFragmentUlnocked;
    [Space(10)]

    [Header("Locks")]
    [Space(5)]
    public Animator lockAnim;

    public bool isFragmentLocked {  get; protected set; }


    public void OnPartSolved()
    {
        solvedParts += 1;
        if (CheckIfAllSolved())
        {
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].SilentLock();
            }
            onFragmentSolved.Invoke();
        }
    }

    public bool CheckIfAllSolved()
    {
        return solvedParts >= toSolveParts;
    }

    public void Unlock()
    {
        isFragmentLocked = false;
        lockAnim?.SetBool("IsLocked", isFragmentLocked);
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].selfLocked)
                continue;
            parts[i].Unlock();
        }
    }

    public void InitializeFragment()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i].InitializePart();
        }
    }

}
