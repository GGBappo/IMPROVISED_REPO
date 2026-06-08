using UnityEngine;

public abstract class GeneralState : MonoBehaviour
{
    public virtual void EnterState(GameStateManager manager) {}
    public virtual void StateAction(GameStateManager manager) {}
    public virtual void ExitState(GameStateManager manager) {}
}
