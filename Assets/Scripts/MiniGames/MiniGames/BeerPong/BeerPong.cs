using UnityEngine;
using UnityEngine.Rendering;

public class BeerPong : MiniGame
{
    [SerializeField] private QTE qte; // Reference to the QTE script
    [SerializeField] private PhysicsMaterial bounceMaterial; // Bouncy material for the ball
    [SerializeField] private int chances;
    [SerializeField] private float baseStrenght;
    [SerializeField] private float strenghtModifier;
    [SerializeField] private DraggableBall dragBall;



    private void Start()
    {
        dragBall.SetBeerPong(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetMinigame();
        }
    }

    public void OnBallRelease()
    {
        // Calculate throw direction and force based on mouse movement
        Vector3 throwDirection = (dragBall.transform.forward).normalized;
        float throwForce = baseStrenght + strenghtModifier * qte.Strenght;
        dragBall.Rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        qte.Stop();
        Debug.Log("[BeerPong] Threw ball with force: " + throwForce);
    }


    //The "AI" will move once the player has moved and also pick a random cup to throw the ball into.
    //the AI will have a random chance to make the ball as well.
    private void AIMakeMove()
    {

    }


    public void ResetMinigame()
    {
        dragBall.transform.rotation = Quaternion.Euler(-30, 0, 0);
        dragBall.ResetMiniGame();
        qte.Start();
    }
}
