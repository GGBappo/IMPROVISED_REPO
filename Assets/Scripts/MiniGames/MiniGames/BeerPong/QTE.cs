using UnityEngine;

public class QTE : MonoBehaviour
{
    [Header("QTE Settings")]
    [SerializeField] private Transform pointA; // Reference to the starting point
    [SerializeField] private Transform pointB; // Reference to the ending point
    [SerializeField] private RectTransform safeZone; // Reference to the safe zone RectTransform
    [SerializeField] private float moveSpeed = 100f; // Speed of the pointer movement
    private float direction = 1f; // 1 for moving towards B, -1 for moving towards A
    private RectTransform pointerTransform;
    private Vector3 targetPosition;



    void Start()
    {
        pointerTransform = GetComponent<RectTransform>();
        targetPosition = pointB.position;
    }

    void Update()
    {
        // Move the pointer towards the target position
        pointerTransform.position = Vector3.MoveTowards(pointerTransform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Change direction if the pointer reaches one of the points
        if (Vector3.Distance(pointerTransform.position, pointA.position) < 0.1f)
        {
            targetPosition = pointB.position;
            direction = 1f;
        }
        else if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f)
        {
            targetPosition = pointA.position;
            direction = -1f;
        }

        // Check for input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckSuccess();
        }
    }

    void CheckSuccess()
    {
        // Check if the pointer is within the safe zone
        if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position, null))
        {
            QTESuccess();
            Debug.Log("Success!");
        }
        else
        {
            QTEFailure();
            Debug.Log("Fail!");
        }
    }

    void QTESuccess()
    {
        //UpdateIdentityMeter(-10f); // Decrease identity meter on success
    }
    void QTEFailure()
    {
        //UpdateIdentityMeter(10f); // Increase identity meter on failure
    }
}
