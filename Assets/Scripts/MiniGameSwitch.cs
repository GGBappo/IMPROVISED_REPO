using System.Collections;
using UnityEngine;

public class MiniGameSwitch : MonoBehaviour
{
    [SerializeField] private Transform miniGameCameraPos;
    [SerializeField] private Transform mainGameCameraPos;
    [SerializeField] private Camera mainCamera;

    [SerializeField] private GameObject arrowLeft;
    [SerializeField] private GameObject arrowRight;

    Coroutine switchCoroutine;


    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        arrowLeft.SetActive(false);
    }

    public void OnClickSwitchtToMiniGame()
    {
        mainCamera.fieldOfView = 50f;

        StartCoroutine(SwitchToMiniGame());
        arrowLeft.SetActive(true);
        arrowRight.SetActive(false);
    }

    public void OnClickSwitchToMainGame()
    {
        mainCamera.fieldOfView = 50f;

        StartCoroutine(SwitchToMainGame());
        arrowLeft.SetActive(false);
        arrowRight.SetActive(true);
    }

    private IEnumerator SwitchToMiniGame()
    {
        float duration = 1f; // Duration of the transition
        float elapsedTime = 0f;
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            // Smoothly interpolate position and rotation
            mainCamera.transform.position = Vector3.Lerp(startPosition, miniGameCameraPos.position, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, miniGameCameraPos.rotation, t);
            yield return null;
        }
        // Ensure final position and rotation are set
        mainCamera.transform.position = miniGameCameraPos.position;
        mainCamera.transform.rotation = miniGameCameraPos.rotation;
    }

    private IEnumerator SwitchToMainGame()
    {
        float duration = 1f; // Duration of the transition
        float elapsedTime = 0f;
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            // Smoothly interpolate position and rotation
            mainCamera.transform.position = Vector3.Lerp(startPosition, mainGameCameraPos.position, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, mainGameCameraPos.rotation, t);
            yield return null;
        }
        // Ensure final position and rotation are set
        mainCamera.transform.position = mainGameCameraPos.position;
        mainCamera.transform.rotation = mainGameCameraPos.rotation;
    }
}