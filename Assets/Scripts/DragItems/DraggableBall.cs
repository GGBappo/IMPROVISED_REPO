using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class DraggableBall : MonoBehaviour
{
    [Tooltip("Material that gives the ball its bounciness")]
    public PhysicsMaterial bounceMaterial; // assign in inspector

    Vector3 ballStartPosition;

    private BeerPong beerPong;
    private Rigidbody rb;
    private float fixedZ;           // Z depth locked while dragging
    private float fixedY = 1.6f;    // Y depth locked while dragging

    public Rigidbody Rb { get => rb; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Start as kinematic so we can move it manually
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Start()
    {
        ballStartPosition = gameObject.transform.position;

        // Lock to the initial Z position of the ball (its spawn depth)
        fixedZ = transform.position.z;
    }

    void OnMouseDrag()
    {
        // Build a screen‑space point that includes the distance to the fixed Z plane
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = Mathf.Abs(Camera.main.transform.position.z - fixedZ);

        // Convert to world space and lock the Z coordinate
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPoint);
        worldPos.z = fixedZ;
        worldPos.y = fixedY;
        transform.position = worldPos;
        // Reset any accumulated physics velocity while dragging
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void OnMouseUp()
    {
        // Enable physics and apply bounce material
        // Enable physics and reset any residual velocity before release
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (bounceMaterial != null)
        {
            // Apply bounce material to the collider, not the Rigidbody
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.sharedMaterial = bounceMaterial;
            else
                Debug.LogWarning("[DraggableBall] No Collider found to assign bounce material.");
        }
        else
        {
            Debug.LogWarning("[DraggableBall] No bounce material assigned – using default physics.");
        }

        beerPong.OnBallRelease();
    }

    public void SetBeerPong(BeerPong beerPong)
    {
        this.beerPong = beerPong;
    }

    public void ResetMiniGame()
    {
        gameObject.transform.position = ballStartPosition;
        // Reset the ball's position and physics state
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.useGravity = false;
    }
}
