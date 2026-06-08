// this game state is responsible for only 4 states {ACTIVE, HUB, OUTCOME, MENU}
// the other game state script (general game state) will be responsible for the interal state during each level
public interface IGameState
{
    public void EnterState(GameStateManager manager);
    public void UpdateState(GameStateManager manager);
    public void ExitState(GameStateManager manager);
}