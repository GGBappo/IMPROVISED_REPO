using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class MainMenuButton : MonoBehaviour
{
    [SerializeField] private TransitionType _transition = TransitionType.Fade;
    private void Awake()
    {
        UnityEngine.UI.Button myButton = GetComponent<UnityEngine.UI.Button>();

        myButton.onClick.AddListener(() => 
        {
            GameEvents.RequestEndLevel();
        });
        
    }
}
