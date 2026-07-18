using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class SettingsButton : MonoBehaviour
{
    private bool _isSettingsOpen = false;
    private void Awake()
    {
        UnityEngine.UI.Button myButton = GetComponent<UnityEngine.UI.Button>();

        myButton.onClick.AddListener(() => 
        {
            if (_isSettingsOpen)
            {
                GameEvents.StartMenuStateChanged(StartMenuState.Await);
                _isSettingsOpen = false;
                return;
            }
            else
            {
                GameEvents.StartMenuStateChanged(StartMenuState.Settings);
                _isSettingsOpen = true;
            }
        });
    }
}
