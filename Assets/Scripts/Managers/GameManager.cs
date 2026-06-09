using UnityEngine;
using System;

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

    [Header("Managers")]
    [Header("Gameplay Related")]
    [SerializeField]private BombManager bombManager;
    [SerializeField]private BudgetManager budgetManager;
    [SerializeField]private LevelManager levelManager;
    [SerializeField]private TimerManager timerManager;


    [Header("System Related")]
    [SerializeField]private SceneOperator sceneManager;
    [SerializeField]private GameStateManager gameStateManager;
    [SerializeField]private UIManager uiManager;
    [SerializeField]private StoryManager storyManager;

    [Header("Progression Related")]
    [SerializeField]private ProgressionSystem progressionSystem;
    [SerializeField]private TaskSystem taskSystem;
    
    [Header("Player Related")]
    [SerializeField]private PlayerInputManager playerInputManager;
    
    [Header("Run Settings")]

    [Tooltip("If true, the game will run with debug features enabled. This is yet to be made, since we dont have much right now.")]
    [SerializeField]private bool runWithDebug;
    [Tooltip("Run the game starting off this scene. This should be used for when you'd like to skip the main menu or a specifiic scene.")]
    [SerializeField]private string runStartingScene;
    
    void Start()
    {
        sceneManager.Setup();            
        gameStateManager.Setup();
        // bombManager.Setup();
        // budgetManager.Setup();
        levelManager.Setup();
        //progressionSystem.Setup();
        //taskSystem.Setup();
        //timerManager.Setup();
        uiManager.Setup();
        //storyManager.Setup();
        //playerInputManager.Setup();

        sceneManager.Load(runStartingScene);
    }

    public void PlayTransitionIN(TransitionType transition, Action onComplete = null)
    {
        uiManager.PlayTransitionIN(transition, onComplete);
    }

    public void PlayTransitionOUT(TransitionType transition, Action onComplete = null)
    {
        uiManager.PlayTransitionOUT(transition, onComplete);
    }

}
