using UnityEngine;
using UnityEngine.Rendering;

public class BeerPong : MiniGame
{
    [SerializeField] private QTE qte; // Reference to the QTE script
    [SerializeField] private GameObject pongBall;// Reference to the pong ball GameObject
    [SerializeField] private Transform pongBallSpawnPoint; // Reference to the spawn point for the pong ball
    [SerializeField] private PhysicsMaterial bounceMaterial; // Bouncy material for the ball


    private float lockedZ; // Fixed depth from camera
    private Rigidbody ballRb; // Cached Rigidbody
    private Vector3 mousePos;


    private void Start()
    {
        // Cache Rigidbody and disable physics while dragging
        ballRb = pongBall.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            ballRb.isKinematic = true;
            ballRb.useGravity = false;
        }
        else
        {
            Debug.LogWarning("[BeerPong] Pong ball missing Rigidbody component.");
        }

        // Fixed Z depth for dragging
        lockedZ = pongBallSpawnPoint.position.z;
    }

    private void OnMouseDrag()
    {
        // Convert mouse position to world position on fixed Z plane
        Vector3 screenPoint = Input.mousePosition;
        // Distance from camera to the fixed Z plane
        screenPoint.z = Mathf.Abs(Camera.main.transform.position.z - lockedZ);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPoint);
        worldPos.z = lockedZ; // lock depth
        pongBall.transform.position = worldPos;
        Debug.Log("[BeerPong] Dragging ball to: " + worldPos);
    }


    //The ball will only be able to move on the x axis by the player and throw the ball with ballRayCast()
    //Called when the player switches over to the mini game
    private void ballController()
    {
        if (pongBall == null)
        {
            Debug.Log("Ball is missing");
        }



    }

    //The ball will have a raycast infront of it to detect if there is a cup infront of it or not
    //If there is a cup then the cup will glow and when the player presses enter the QTE will appear.
    private void ballRayCast()
    {

    }

    //The "AI" will move once the player has moved and also pick a random cup to throw the ball into.
    //the AI will have a random chance to make the ball as well.
    private void AIMakeMove()
    {

    }
}
