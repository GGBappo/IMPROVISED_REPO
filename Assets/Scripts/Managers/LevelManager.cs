using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelData[] allLevels;
    public LevelData currentLevel;
    public int currentLevelIndex = 0;
    private bool isReady = false; // for wakeup when getting called by game manager
    public void Setup()
    {
        isReady = true;
    }
    
    public void LoadLevel(int index)
    {
        if (index < 0 || index >= allLevels.Length)
        {
            Debug.LogError("Invalid level index: " + index);
            return;
        }
        currentLevelIndex = index;
        currentLevel = allLevels[currentLevelIndex];
    }
    public void UnlockNextLevel()
    {
        
    }

    public bool isLevelUnlocked(int index)
    {
        return true;
    }

    /*
    public LevelData GetLevelData(int index)
    {
        
    }
    */
}
