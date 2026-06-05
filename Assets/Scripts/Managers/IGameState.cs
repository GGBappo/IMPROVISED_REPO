public interface IGameState
{
    public void EnterState(GameStateManager manager);
    public void UpdateState(GameStateManager manager);
    public void ExitState(GameStateManager manager);
}
