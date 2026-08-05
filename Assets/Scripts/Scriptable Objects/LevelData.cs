using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    [Tooltip("The index of the level. This is used not only to determine the level's position in the level select menu and story events, but also to determine which level is unlocked when the player completes a level.")]
    public int levelIndex;

    [Tooltip("The screen name of the level. This is what will be displayed on the level select menu.")]
    public string levelName;

    [Tooltip("The location of the level. This is what will be displayed on the level select menu.")]
    public string levelLocation;

    [Tooltip("The description of the level. This is what will be displayed on the level select menu.")]
    public string levelDescription;

    [Tooltip("Drag the physical scene file here in order for the scene manager to load the level. This pulls the level's name in order to load. See the SceneOperator for more information.")]
    public SceneReference sceneToLoad;

    [Tooltip("The starting budget for the level.")]
    public int startingBudget;

    [Header("Bomb Info")]
    [Tooltip("The prefab for the bomb. Once added, you can use the inspector preview to view the bomb.")]
    public GameObject bombPrefab;

    [Tooltip("The timer for the bomb. This timer will be paused during story events and if the player is in the menu.")]
    public float timerSeconds;

    [Tooltip("Whether the bomb is a dud or not. If checked, the level will NOT have a bomb defusal, and will instead be a narrative cutscene.")]
    public bool isDud;    

    [Header("Story Events")]
    [Tooltip("The story events for the level.")]
    public StoryEvent[] storyEvents;

    [Header("Unlocking")]
    [Tooltip("Whether the level is unlocked. If checked, this means the player is able to access the level at any time.")]
    public bool isUnlocked;

}
