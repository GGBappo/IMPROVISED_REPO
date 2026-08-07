using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class SettingsButton : MonoBehaviour
{
    private bool _isSettingsOpen = false;

    private void OnEnable() => GameEvents.OnStartMenuStateChanged += OnStartMenuStateChanged;
    private void OnDisable() => GameEvents.OnStartMenuStateChanged -= OnStartMenuStateChanged;

    private void Awake()
    {
        UnityEngine.UI.Button myButton = GetComponent<UnityEngine.UI.Button>();

        myButton.onClick.AddListener(() => 
        {
            GameEvents.StartMenuStateChanged(_isSettingsOpen ? StartMenuState.Await : StartMenuState.Settings);
        });
    }

    private void OnStartMenuStateChanged(StartMenuState newState)
    {
        _isSettingsOpen = newState == StartMenuState.Settings;
    }
}
