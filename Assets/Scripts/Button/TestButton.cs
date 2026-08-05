using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class TestButton : MonoBehaviour
{
    [SerializeField] private GameObject _targetObject;
    private void Awake()
    {
        UnityEngine.UI.Button myButton = GetComponent<UnityEngine.UI.Button>();

        myButton.onClick.AddListener(() => 
        {
            GameEvents.StartButtonPressed();
            GameEvents.StartMenuStateChanged(StartMenuState.TaskHandout);
        });
    }
}
