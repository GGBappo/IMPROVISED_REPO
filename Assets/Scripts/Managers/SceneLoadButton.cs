using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class StartButton : MonoBehaviour
{
    [SerializeField] private string _targetSceneName;
    [SerializeField] private TransitionType _transition = TransitionType.Fade;
    [SerializeField] private string _unloadScene;

    private void Awake()
    {
        UnityEngine.UI.Button myButton = GetComponent<UnityEngine.UI.Button>();

        myButton.onClick.AddListener(() => 
        {
            GameEvents.RequestLevelStart();
            GameEvents.RequestSceneUnLoad(_unloadScene);
        });
        
    }
}