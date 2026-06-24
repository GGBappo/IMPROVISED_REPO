using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    private GameStateManager gameStateManager;
    private BombManager bombManager;
    private BudgetManager budgetManager;
    private LevelManager levelManager;
    private ProgressionSystem progressionSystem;
    private TimerManager timerManager;
    private UIManager uiManager;
    private StoryManager storyManager;

    // once i get to work on the LevelManager
    // and the architecture begins to become clearer
    // i think these functions will get removed
    void StartNextLevel()
    {
    }

    void OnLevelComplete()
    {
        
    }

    void OnLevelFinished()
    {
        
    }

    private void Initalize()
    {
        
    }
}
