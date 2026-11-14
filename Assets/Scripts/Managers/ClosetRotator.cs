using System.Collections;
using UnityEngine;

public class ClosetRotator : MonoBehaviour
{
    private const float StepAngle = 90f;

    [SerializeField] private ClothingManager clothingManager;
    [SerializeField] private Transform rotatingCloset;
    [SerializeField, Min(0.01f)] private float rotationDuration = 0.35f;

    private Quaternion frontRotation;
    private Quaternion targetRotation;

    public bool IsRotating { get; private set; }

    private void Awake()
    {
        if (rotatingCloset == null)
            rotatingCloset = transform;

        // The authored pose is the one with Hook 0 facing the player.
        frontRotation = rotatingCloset.localRotation;

        if (clothingManager == null)
        {
            Debug.LogError("Assign the Clothing Manager on Closet Rotator.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (clothingManager == null)
            return;

        clothingManager.OnNextClicked += RotateNext;
        clothingManager.OnPreviousClicked += RotatePrevious;
    }

    private void OnDisable()
    {
        if (clothingManager != null)
        {
            clothingManager.OnNextClicked -= RotateNext;
            clothingManager.OnPreviousClicked -= RotatePrevious;
        }

        if (!IsRotating)
            return;

        // Finish at the selected hook if this component is disabled mid-turn.
        StopAllCoroutines();
        rotatingCloset.localRotation = targetRotation;
        IsRotating = false;
    }

    public void SetInitialRotation(int outfitIndex)
    {
        // Set the initial rotation based on the outfit index, ensuring it is within the valid range.
        rotatingCloset.localRotation = RotationForAngle(outfitIndex * StepAngle);
    }

    private void RotateNext()
    {
        BeginRotation(StepAngle);
    }

    private void RotatePrevious()
    {
        BeginRotation(-StepAngle);
    }

    private void BeginRotation(float angle)
    {
        if (IsRotating)
            return;

        // Calculate the target rotation based on the current rotation and the desired angle.
        Quaternion startRotation = rotatingCloset.localRotation;
        targetRotation = Quaternion.AngleAxis(angle, Vector3.up) * startRotation;

        // Set the guard immediately so rapid clicks can't start overlapping turns.
        IsRotating = true;
        StartCoroutine(AnimateRotation(startRotation));
    }

    private Quaternion RotationForAngle(float angle)
    {
        // Calculate the rotation for a given angle relative to the front rotation.
        return Quaternion.AngleAxis(angle, Vector3.up) * frontRotation;
    }

    private IEnumerator AnimateRotation(Quaternion startRotation)
    {
        float duration = Mathf.Max(0.01f, rotationDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            yield return null;
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            rotatingCloset.localRotation =
                Quaternion.Slerp(startRotation, targetRotation, easedProgress);
        }

        rotatingCloset.localRotation = targetRotation;
        IsRotating = false;
    }
}