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
    private BombPart hoveredPart;

    private GlobalStateType _currentGameState;

    private void OnEnable() => GameEvents.OnGlobalStateChanged += UpdateLocalState;
    private void OnDisable() => GameEvents.OnGlobalStateChanged -= UpdateLocalState;

    private void Update()
    {
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
}