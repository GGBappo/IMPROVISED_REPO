using System.Collections;
using UnityEngine;

public class MiniGameSwitch : MonoBehaviour
{
    [SerializeField] private Transform miniGameCameraPos;
    [SerializeField] private Transform mainGameCameraPos;
    [SerializeField] private Transform leftGameCameraPos;
    [SerializeField] private Transform rightGameCameraPos;
    [SerializeField] private Camera mainCamera;

    [SerializeField] private GameObject arrowLeft;
    [SerializeField] private GameObject arrowRight;
    [SerializeField] private GameObject curvedArrowLeft;
    [SerializeField] private GameObject curvedArrowRight;

    Coroutine switchCoroutine;
    enum SwitchState { MiniGame, MainGame, LeftGame, RightGame };
    SwitchState switchState = SwitchState.MainGame;

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
        switchState = SwitchState.MiniGame;
        StartCoroutine(SwitchToMiniGame());
        arrowLeft.SetActive(true);
        arrowRight.SetActive(false);
        curvedArrowLeft.SetActive(false);
        curvedArrowRight.SetActive(false);
    }

    public void OnClickSwitchToMainGame()
    {
        mainCamera.fieldOfView = 50f;
        switchState = SwitchState.MainGame;
        StartCoroutine(SwitchToMainGame());
        arrowLeft.SetActive(false);
        arrowRight.SetActive(true);
        curvedArrowLeft.SetActive(true);
        curvedArrowRight.SetActive(true);
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
    public void OnClickSwitchToLeftGame()
    {
        if (switchState == SwitchState.RightGame)
        {
            StartCoroutine(SwitchToMainGame());
            switchState = SwitchState.MainGame;
            curvedArrowLeft.SetActive(true);
        }
        else
        {
            StartCoroutine(SwitchToLeftGame());
            switchState = SwitchState.LeftGame;
            curvedArrowLeft.SetActive(false);
        }

        mainCamera.fieldOfView = 50f;
        curvedArrowRight.SetActive(true);
        arrowLeft.SetActive(false);
        arrowRight.SetActive(true);
    }

    public void OnClickSwitchToRightGame()
    {
        if (switchState == SwitchState.LeftGame)
        {
            StartCoroutine(SwitchToMainGame());
            switchState = SwitchState.MainGame;
            curvedArrowRight.SetActive(true);
        }
        else
        {
            StartCoroutine(SwitchToRightGame());
            switchState = SwitchState.RightGame;
            curvedArrowRight.SetActive(false);
        }


        mainCamera.fieldOfView = 50f;
        curvedArrowLeft.SetActive(true);
        arrowLeft.SetActive(false);
        arrowRight.SetActive(true);
    }

    private IEnumerator SwitchToLeftGame()
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            mainCamera.transform.position = Vector3.Lerp(startPosition, leftGameCameraPos.position, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, leftGameCameraPos.rotation, t);
            yield return null;
        }
        mainCamera.transform.position = leftGameCameraPos.position;
        mainCamera.transform.rotation = leftGameCameraPos.rotation;
    }

    private IEnumerator SwitchToRightGame()
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            mainCamera.transform.position = Vector3.Lerp(startPosition, rightGameCameraPos.position, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, rightGameCameraPos.rotation, t);
            yield return null;
        }
        mainCamera.transform.position = rightGameCameraPos.position;
        mainCamera.transform.rotation = rightGameCameraPos.rotation;
    }
}