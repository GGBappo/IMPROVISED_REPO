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

    public void InitializeBomb(GameObject prefab)
    {

    }

    public void OnPartSolved(BombPart part)
    {
        part.isSolved = true;
        solvedParts++;
        if (CheckAllSolved())
        {
            TriggerDefused();
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
