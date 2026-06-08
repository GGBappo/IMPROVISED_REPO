using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public IGameState currentState { get; private set; }
    public void Setup()
    {
        currentState = new MENU();
        currentState.EnterState(this);   
    }
}
