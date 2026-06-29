using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    [SerializeField] 
    private Camera _mainCamera;

    private Coroutine _moveCameraCoroutine; // defining this so that if the camera is in action we could stop it

    private void OnEnable()
    {
        GameEvents.OnCameraMoveRequest += MoveCamera; 
        GameEvents.OnCameraLookAtRequest += LookAtTarget; 
        GameEvents.OnCameraLookAtGameObjectRequest += LookAtTarget;
    }

    private void OnDisable()
    {
        GameEvents.OnCameraMoveRequest -= MoveCamera; 
        GameEvents.OnCameraLookAtRequest -= LookAtTarget; 
        GameEvents.OnCameraLookAtGameObjectRequest -= LookAtTarget;
    }

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
    }

    /// <summary>
    /// Moves the camera to the specified position AND rotation over a period of time. 
    /// If you'd like to look at a target, you should instead use <see cref="LookAtTarget(Vector3, float)"/> or <see cref="LookAtTarget(GameObject, float)"/>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="duration"></param>
    private void MoveCamera(Vector3 position, Quaternion rotation, float duration)
    {
        if (_moveCameraCoroutine != null)
        {
            StopCoroutine(_moveCameraCoroutine);
        }
        _moveCameraCoroutine = StartCoroutine(MoveCameraRoutine(position, rotation, duration)); 
    }

    
    private void LookAtTarget(Vector3 targetPosition, float duration)
    {
        if (_moveCameraCoroutine != null)
        {
            StopCoroutine(_moveCameraCoroutine);
        }
        _moveCameraCoroutine = StartCoroutine(LookAtTargetRoutine(targetPosition, duration));
    }

    private void LookAtTarget(GameObject target, float duration)
    {
        if (_moveCameraCoroutine != null)
        {
            StopCoroutine(_moveCameraCoroutine);
        }
        _moveCameraCoroutine = StartCoroutine(LookAtTargetRoutine(target.transform.position, duration));
    }

    private IEnumerator LookAtTargetRoutine(Vector3 targetPosition, float duration)
    {
        Quaternion startRotation = _mainCamera.transform.rotation;
        Vector3 direction = targetPosition - _mainCamera.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float percentage = elapsedTime / duration;
            percentage = Mathf.SmoothStep(0f, 1f, percentage);

            _mainCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, percentage);

            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        _mainCamera.transform.rotation = targetRotation;
    }

    private IEnumerator MoveCameraRoutine(Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        Debug.Log($"[CameraManager] Starting coroutine to move to position: {targetPosition}, rotation: {targetRotation}, duration: {duration}");
        if (duration <= 0f)
        {
            _mainCamera.transform.position = targetPosition;
            _mainCamera.transform.rotation = targetRotation;
            _moveCameraCoroutine = null;
            Debug.Log($"[CameraManager] Finished moving to position: {targetPosition}, rotation: {targetRotation}");
            yield break;
        }

        Vector3 startPos = _mainCamera.transform.position;
        Quaternion startRot = _mainCamera.transform.rotation;
        
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float percentage = elapsedTime / duration;

            percentage = Mathf.SmoothStep(0f, 1f, percentage);

            _mainCamera.transform.position = Vector3.Lerp(startPos, targetPosition, percentage);
            _mainCamera.transform.rotation = Quaternion.Lerp(startRot, targetRotation, percentage);

            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        _mainCamera.transform.position = targetPosition;
        _mainCamera.transform.rotation = targetRotation;
        
        _moveCameraCoroutine = null;
        Debug.Log($"[CameraManager] Finished moving to position: {targetPosition}, rotation: {targetRotation}");
    }
}
