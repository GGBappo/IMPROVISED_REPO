using UnityEngine;
using UnityEngine.Rendering;

public class BeerPong : MiniGame
{
    [SerializeField] private QTE qte; // Reference to the QTE script
    [SerializeField] private GameObject pongBall;// Reference to the pong ball GameObject
    [SerializeField] private Transform pongBallSpawnPoint; // Reference to the spawn point for the pong ball


    private float lockedY;
    private float lockedZ;

    private Vector3 mousePos;


    private void Start()
    {
        lockedY = pongBallSpawnPoint.position.y;
        lockedZ = pongBallSpawnPoint.position.z;

        mousePos = new Vector3(mousePos.x, lockedY, lockedZ);
    }

    private void OnMouseDrag()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3(mousePos.x, lockedY, lockedZ);
        pongBall.transform.position = mousePos;

        Debug.Log("Mouse Position: " + mousePos);
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
