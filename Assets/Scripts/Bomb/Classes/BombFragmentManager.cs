using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BombFragmentManager : MonoBehaviour
{
    [Tooltip("Reference to the parent BombManager")]
    [SerializeField] BombManager bomb;

    [Tooltip("Parts, that are the part of the fragment")]
    [SerializeField] BombPart[] parts;

    [Tooltip("How many of the parts need to be solved")]
    [SerializeField] int toSolveParts;


    protected int solvedParts;
    
    [Tooltip("Triggers, when all necessary parts are solved")]
    public UnityEvent onFragmentSolved;

    [Tooltip("Triggers, when fragment is unlocked")]
    public UnityEvent onFragmentUlnocked;

    [Tooltip("Animator of the Lock")]
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
