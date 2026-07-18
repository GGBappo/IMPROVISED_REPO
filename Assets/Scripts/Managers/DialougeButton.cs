using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class DialougeButton : MonoBehaviour
{   
    private bool _isSpawned = false; // temp
    private void Awake() {
        UnityEngine.UI.Button button = GetComponent<UnityEngine.UI.Button>();

        button.onClick.AddListener(() =>
        {
            if (!_isSpawned)
            {
                GameEvents.RequestLatestAssignmentFolderSpawn();
                _isSpawned = true;
            }
            else
            {
                GameEvents.StartMenuStateChanged(StartMenuState.LevelChoose);
            }
        });
    }
}
