using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasCameraLink : MonoBehaviour
{
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
