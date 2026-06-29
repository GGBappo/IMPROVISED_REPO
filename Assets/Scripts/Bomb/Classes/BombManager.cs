using System.Collections;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    public enum BombState { Armed, Defused, Exploded};
    public BombState currentState;
    public BombFragmentManager[] fragments;
    public BombFragmentManager[] baseUnlock;
    public int totalFragments;
    public int solvedFragments;
    public CoreManager core;

    private void Start()
    {
        InitializeBomb();
    }
    public void InitializeBomb()
    {
        for (int i = 0; i < baseUnlock.Length; i++)
        {
            baseUnlock[i].Unlock();
        }
        for (int i = 0; i < fragments.Length; i++)
        {
            fragments[i].InitializeFragment();
        }
    }
    private void OnEnable()
    {
        GameEvents.OnTimerExpired += TriggerExplosion;
    }
    private void OnDisable()
    {
        GameEvents.OnTimerExpired -= TriggerExplosion;
    }

    public void OnFragmentSolved(/*BombFragmentManager fragment*/)
    {
        solvedFragments++;
        if (CheckAllSolved())
        {
            core.Open();
        }
    }

    public void TriggerExplosion()
    {
        currentState = BombState.Exploded;
        Debug.Log("BOMB EXPLODED! :(");
    }

    public void TriggerDefused()
    {
        currentState = BombState.Defused;
        Debug.Log("BOMB DEFUSED! :)");
    }

    private bool CheckAllSolved()
    {
        return solvedFragments >= totalFragments;
    }
}
