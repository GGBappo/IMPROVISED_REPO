using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public int levelIndex;

    public void OnPlayButtonClick()
    {
        GameEvents.RequestCloseFileScreen();
        GameEvents.RequestLevelStart(levelIndex);
    }
}
