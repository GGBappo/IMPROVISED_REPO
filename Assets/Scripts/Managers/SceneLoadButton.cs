using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartButton : MonoBehaviour
{
    [SerializeField] private string _targetSceneName;
    [SerializeField] private TransitionType _transition = TransitionType.Fade;
    [SerializeField] private string _unloadScene;

    private void Awake()
    {
        Button myButton = GetComponent<Button>();

        myButton.onClick.AddListener(() => 
        {
            GameEvents.RequestSceneLoad(_targetSceneName, _transition);
            GameEvents.RequestSceneUnLoad(_unloadScene);
        });
        
    }
}