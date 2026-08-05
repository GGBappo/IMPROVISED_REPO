using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Level/Level Database", order = 1)]
public class LevelDatabase : ScriptableObject
{
    public LevelData[] allLevels;
    public int latestLevelIndex;


}
