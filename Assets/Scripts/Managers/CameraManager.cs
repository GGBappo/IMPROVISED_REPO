using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private CinemachineCamera _cinemachineCamera;
    private const float _defaultFOV = 50;

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
        EnsureCameraSetup();
    }

    private bool EnsureCameraSetup()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        if (_mainCamera == null)
        {
            return false;
        }

        bool createdCamera = false;
        if (_cinemachineCamera == null)
        {
            _cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
        }

        if (_cinemachineCamera == null)
        {
            GameObject cameraObject = new GameObject("Cinemachine Camera");
            _cinemachineCamera = cameraObject.AddComponent<CinemachineCamera>();
            createdCamera = true;
        }

        if (_mainCamera.GetComponent<CinemachineBrain>() == null)
        {
            _mainCamera.gameObject.AddComponent<CinemachineBrain>();
        }

        if (createdCamera)
        {
            _cinemachineCamera.transform.SetPositionAndRotation(
                _mainCamera.transform.position,
                _mainCamera.transform.rotation);
            SetFOV(_mainCamera.fieldOfView);
        }

        _cinemachineCamera.Priority.Value = 10;
        return true;
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
        if (!EnsureCameraSetup())
        {
            Debug.LogWarning("[CameraManager] Ignoring camera move because no Unity camera is available.");
            return;
        }

        _cinemachineCamera.transform.DOKill();
        _cinemachineCamera.DOKill();
        if (lookAtMarker.HasValue)
        {
            _cinemachineCamera.transform.DOLookAt(lookAtMarker.Value, duration)
                .SetEase(Ease.InOutSine);
        } 
        else
        {
            _cinemachineCamera.transform.DORotateQuaternion(rotation, duration)
                .SetEase(Ease.InOutSine);
        }

        if (FOV.HasValue && FOV.Value != _cinemachineCamera.Lens.FieldOfView)
        {
            TweenFOV(FOV.Value, duration);
        }

        _cinemachineCamera.transform.DOMove(position, duration)
            .SetEase(Ease.InOutSine);
    }
    
    private void LookAtTarget(Vector3 targetPosition, float duration, float FOV = _defaultFOV)
    {
        if (!EnsureCameraSetup())
        {
            Debug.LogWarning("[CameraManager] Ignoring look-at request because no Unity camera is available.");
            return;
        }

        _cinemachineCamera.transform.DOKill();
        _cinemachineCamera.DOKill();

        _cinemachineCamera.transform.DOLookAt(targetPosition, duration)
            .SetEase(Ease.InOutSine);

        TweenFOV(FOV, duration);
    }

    private void LookAtTarget(GameObject target, float duration, float FOV = _defaultFOV)
    {
        if (!EnsureCameraSetup())
        {
            Debug.LogWarning("[CameraManager] Ignoring look-at request because no Unity camera is available.");
            return;
        }

        _cinemachineCamera.transform.DOKill();
        _cinemachineCamera.DOKill();

        _cinemachineCamera.transform.DOLookAt(target.transform.position, duration)
            .SetEase(Ease.InOutSine);

        TweenFOV(FOV, duration);
    }
    

    private void ChangeCameraFOV(float FOV, bool slowZoom = false, float duration = 1f){
        if (!EnsureCameraSetup())
        {
            Debug.LogWarning("[CameraManager] Ignoring FOV request because no Unity camera is available.");
            return;
        }

        if (!slowZoom){
            TweenFOV(FOV, duration);
        }
        else
        {
            SetFOV(FOV);
        }
    }

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
    private void SetFOV(float FOV)
    {
        LensSettings lens = _cinemachineCamera.Lens;
        lens.FieldOfView = FOV;
        _cinemachineCamera.Lens = lens;
    }

    private void TweenFOV(float targetFOV, float duration)
    {
        _cinemachineCamera.DOKill();
        DOTween.To(
            () => _cinemachineCamera.Lens.FieldOfView,
            SetFOV,
            targetFOV,
            duration)
            .SetEase(Ease.InOutSine);
    }
}
