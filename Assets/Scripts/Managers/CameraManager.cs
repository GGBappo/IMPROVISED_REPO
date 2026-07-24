using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    [SerializeField] 
    private Camera _mainCamera;
    private const float _defaultFOV = 50;
    private Coroutine _moveCameraCoroutine; // defining this so that if the camera is in action we could stop it
    private Coroutine _zoomCoroutine;

    private void OnEnable()
    {
        GameEvents.OnCameraMoveRequest += MoveCamera; 
        GameEvents.OnCameraLookAtRequest += LookAtTarget; 
        GameEvents.OnCameraLookAtGameObjectRequest += LookAtTarget;
        GameEvents.OnCameraFOVChangeRequest += ChangeCameraFOV;
    }

    private void OnDisable()
    {
        GameEvents.OnCameraMoveRequest -= MoveCamera; 
        GameEvents.OnCameraLookAtRequest -= LookAtTarget; 
        GameEvents.OnCameraLookAtGameObjectRequest -= LookAtTarget;
        GameEvents.OnCameraFOVChangeRequest -= ChangeCameraFOV;
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
    /// <param name="lookAtMarker"></param>
    /// <param name="FOV"></param>
    private void MoveCamera(Vector3 position, Quaternion rotation, float duration, Vector3? lookAtMarker = null, float? FOV = null)
    {
        _mainCamera.transform.DOKill();
        _mainCamera.DOKill();
        if (lookAtMarker.HasValue)
        {
            _mainCamera.transform.DOLookAt(lookAtMarker.Value, duration)
                .SetEase(Ease.InOutSine);
        } 
        else
        {
            _mainCamera.transform.DORotateQuaternion(rotation, duration)
                .SetEase(Ease.InOutSine);
        }

        if (FOV.HasValue && FOV.Value != _mainCamera.fieldOfView)
        {
            _mainCamera.DOFieldOfView(FOV.Value, duration)
                .SetEase(Ease.InOutSine);
        }

        _mainCamera.transform.DOMove(position, duration)
            .SetEase(Ease.InOutSine);
    }
    
    private void LookAtTarget(Vector3 targetPosition, float duration, float FOV = _defaultFOV)
    {
        _mainCamera.transform.DOKill();
        _mainCamera.DOKill();

        // 1. Rotate towards the target over the duration
        _mainCamera.transform.DOLookAt(targetPosition, duration)
            .SetEase(Ease.InOutSine);

        _mainCamera.DOFieldOfView(FOV, duration)
            .SetEase(Ease.InOutSine);
    }

    private void LookAtTarget(GameObject target, float duration, float FOV = _defaultFOV)
    {
        _mainCamera.transform.DOKill();
        _mainCamera.DOKill();

        // 1. Rotate towards the target over the duration
        _mainCamera.transform.DOLookAt(target.transform.position, duration)
            .SetEase(Ease.InOutSine);

        _mainCamera.DOFieldOfView(FOV, duration)
            .SetEase(Ease.InOutSine);
    }
    

    private void ChangeCameraFOV(float FOV, bool slowZoom = false, float duration = 1f){
        if (!slowZoom){
            _zoomCoroutine = StartCoroutine(ChangeFOVRoutine(FOV, duration));
        }
        else
        {
            _mainCamera.fieldOfView = FOV;
        }
    }
    /*
    private IEnumerator LookAtTargetRoutine(Vector3 targetPosition, float duration, float FOV)
    {
        Quaternion startRotation = _mainCamera.transform.rotation;
        Vector3 direction = targetPosition - _mainCamera.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float percentage = elapsedTime / duration;
            float smoothPercentage = Mathf.SmoothStep(0f, 1f, percentage);

            _mainCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, smoothPercentage);
            _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, FOV, smoothPercentage);

            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        _mainCamera.transform.rotation = targetRotation;
        _mainCamera.fieldOfView = FOV;
    }
    */

    /*
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
    */
    private IEnumerator ChangeFOVRoutine(float targetFOV, float duration)
    {
        float startFOV = _mainCamera.fieldOfView;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float percentage = elapsedTime / duration;
            percentage = Mathf.SmoothStep(0f, 1f, percentage);

            _mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, percentage);

            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        _mainCamera.fieldOfView = targetFOV;
        _zoomCoroutine = null;
    }
}
