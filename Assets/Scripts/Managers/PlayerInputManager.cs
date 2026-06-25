// PlayerInputManager.cs
// Handles item following cursor, hover highlight, use on part, right click cancel
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public LayerMask bombPartLayer;

    [Header("State")]
    public bool isHoldingItem = false;

    private Item_SO heldItem;
    private InventorySlot heldSlot;
    private BombPart hoveredPart;

    private GlobalStateType _currentGameState;

    private void OnEnable() => GameEvents.OnGlobalStateChanged += UpdateLocalState;
    private void OnDisable() => GameEvents.OnGlobalStateChanged -= UpdateLocalState;

    private void Update()
    {
        if (isHoldingItem)
        {
            MoveItemWithCursor();
            CheckHoverHighlight();

            // right click � cancel, return to bar
            if (Input.GetMouseButtonDown(1))
            {
                CancelHeldItem();
            }

            // left click � try to use on a bomb part
            if (Input.GetMouseButtonDown(0))
            {
                TryUseOnPart();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_currentGameState == GlobalStateType.Menu)
            {
                GameEvents.StateChanged(GlobalStateType.Active);
            }
            else
            {
                GameEvents.StateChanged(GlobalStateType.Menu);
            }
        }
    }

    private void UpdateLocalState(GlobalStateType newState)
    {
        _currentGameState = newState;
    }
    // Item icon follows mouse cursor
    private void MoveItemWithCursor()
    {
        // move the slot UI with the cursor
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)heldSlot.transform.parent,
            Input.mousePosition,
            null,
            out mousePos
        );
        heldSlot.transform.localPosition = mousePos;
    }

    // Cast ray to check if cursor is over a compatible BombPart
    private void CheckHoverHighlight()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            BombPart part = hit.collider.GetComponent<BombPart>();

            if (part != null)
            {
                // new part hovered
                if (hoveredPart != part)
                {
                    // clear old highlight
                    if (hoveredPart != null)
                        hoveredPart.RemoveHighlight();

                    hoveredPart = part;

                    //if (heldItem.IsCompatibleWith(part))
                        part.Highlight();
                }
                return;
            }
        }

        // not hovering any part � clear highlight
        if (hoveredPart != null)
        {
            hoveredPart.RemoveHighlight();
            hoveredPart = null;
        }
    }

    private void TryUseOnPart()
    {
        if (hoveredPart == null) return;

        bool success = heldItem.Use(hoveredPart);

        if (success)
        {
            // compatible and used successfully
            if (heldItem.isSingleUse)
                heldSlot.ConsumeSlot();   // remove from inventory bar
            else
                ReturnToBar();            // multi-use, go back to bar

            ClearHeldState();
        }
        else
        {
            // wrong item on this part � strike penalty
            StrikeSystem.AddStrike();
            CancelHeldItem();
        }
    }

    // Called by InventorySlot when item is clicked
    public void OnItemPickedUp(Item_SO item, InventorySlot slot)
    {
        heldItem = item;
        heldSlot = slot;
        isHoldingItem = true;
    }

    // Right click � go back to bar
    private void CancelHeldItem()
    {
        ReturnToBar();
        ClearHeldState();
    }

    private void ReturnToBar()
    {
        if (hoveredPart != null)
        {
            hoveredPart.RemoveHighlight();
            hoveredPart = null;
        }
        heldSlot.ResetPosition();
    }

    private void ClearHeldState()
    {
        heldItem = null;
        heldSlot = null;
        isHoldingItem = false;
    }
}