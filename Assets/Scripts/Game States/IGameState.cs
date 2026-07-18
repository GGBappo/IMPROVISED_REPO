/// <summary>
/// Interface for game states, providing methods for entering, updating, and exiting a state.
/// <br>Should be used for ANY state, meaning local and global.</br>
/// </summary>
public interface IGameState
{
    void EnterState();
    void UpdateState();
    void ExitState();
}
