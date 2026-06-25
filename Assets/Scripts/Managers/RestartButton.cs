using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RestartButton : MonoBehaviour
{
    [SerializeField] private TransitionType _transition = TransitionType.Fade;
    private void Awake()
    {
        Button myButton = GetComponent<Button>();

        myButton.onClick.AddListener(() => 
        {
            GameEvents.RequestLevelReset(_transition);
        });
        
    }
}
