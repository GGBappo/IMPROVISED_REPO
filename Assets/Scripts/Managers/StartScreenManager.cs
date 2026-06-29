using UnityEngine;

public class StartScreenManager : MonoBehaviour
{
    [SerializeField] 
    private GameObject _cameraStartPosition;
    private void Start()
    {
        GameEvents.RequestCameraMove(_cameraStartPosition.transform.position, _cameraStartPosition.transform.rotation, 0f);
    }
}
