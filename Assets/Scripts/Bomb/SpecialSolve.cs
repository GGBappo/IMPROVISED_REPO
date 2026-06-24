using UnityEngine;

public class SpecialSolve : MonoBehaviour
{
    [SerializeField] BombPart thisPart;
    [SerializeField] BombPart toSolvePart;
    [SerializeField] int ssID;


    public void Solve()
    {
        toSolvePart.SpecialSolve(ssID);
    }

}
