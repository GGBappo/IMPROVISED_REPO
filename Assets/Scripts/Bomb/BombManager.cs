using System.Collections;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    public enum BombState { Armed, Defused, Exploded};
    public BombState currentState;
    public bool isFake;
    public BombPart[] parts;
    public int totalParts;
    public int solvedParts;
    public BombPart core;

    public void InitializeBomb(GameObject prefab)
    {

    }

    public void OnPartSolved(BombPart part)
    {
        part.isSolved = true;
        solvedParts++;
        if (CheckAllSolved())
        {
            core.Open();
            //TriggerDefused();
        }
    }

    public void TriggerExplosion()
    {
        //You NOT win :(
    }

    public void TriggerDefused()
    {
        Debug.Log("BOMB DEFUSED!");
        //You win!
    }

    private bool CheckAllSolved()
    {
        return solvedParts >= totalParts;
    }

    private void RevealDud()
    {

    }
}
