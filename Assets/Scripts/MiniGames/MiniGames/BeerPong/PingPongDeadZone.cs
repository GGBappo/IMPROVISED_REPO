using UnityEngine;

public class PingPongDeadZone : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        GameEvents.PingPongBallMissedCup();
    }
}
