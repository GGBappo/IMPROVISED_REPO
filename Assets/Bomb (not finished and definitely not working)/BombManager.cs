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
