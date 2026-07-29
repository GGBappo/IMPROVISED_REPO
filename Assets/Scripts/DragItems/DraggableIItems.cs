using UnityEngine;

public class DraggableIItems : MonoBehaviour
{
    private float lockedY;
    private float lockedZ;
    private GameObject pongBall;

    private Vector3 mousePos;

    private void OnMouseDrag()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3(mousePos.x, lockedY, lockedZ);
        pongBall.transform.position = mousePos;

        Debug.Log("Mouse Position: " + mousePos);
    }
}
