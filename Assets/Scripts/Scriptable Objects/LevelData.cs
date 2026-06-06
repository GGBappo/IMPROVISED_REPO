using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    [Tooltip("The index of the level. This is used not only to determine the level's position in the level select menu, but also to determine which level is unlocked when the player completes a level.")]
    public int levelIndex;

    [Tooltip("The name of the level.")]
    public string levelName;

    [Tooltip("The location/PATH of the level.")]
    public string levelLocation;

    [Tooltip("The starting budget for the level.")]
    public int startingBudget;

    [Header("Bomb Info")]
    [Tooltip("The prefab for the bomb. Once added, you can use the inspector preview to view the bomb.")]
    public GameObject bombPrefab;

    [Tooltip("The timer for the bomb. This timer will be paused during story events and if the player is in the menu.")]
    public float timerSeconds;

    [Tooltip("Whether the bomb is a dud or not. If checked, the level will NOT have a bomb defusal, and will instead be a narrative cutscene.")]
    public bool isDud;

    public enum Difficulty{Easy, Medium, Hard}
    [Header("Difficulty")]
    [Tooltip("The difficulty of the level. This will be used to determine a few other factors, most notably time and budget.")]
    public Difficulty difficulty;

    [Header("Story Events")]
    [Tooltip("The story events for the level.")]
    public StoryEvent[] storyEvents;

    [Header("Unlocking")]
    [Tooltip("Whether the level is unlocked. If checked, this means the player is able to access the level at any time.")]
    public bool isUnlocked;
}
