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
            GameEvents.RequestCameraLookAt(_targetObject.transform.position, 1.5f);
        });
        
    }
}
