using UnityEngine;
using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup _loadingScreen;
    [SerializeField] private CanvasGroup _gameOverScreen;
    [SerializeField] private CanvasGroup _pauseScreen;
    [SerializeField] private CanvasGroup _winScreen;

    [Header("Scene Transition Speed")]
    [SerializeField] private float _globalTransitionSpeed = 1f;

    [Header("Outline Shader References")]
    [SerializeField] private Material _blackHighlight;
    [SerializeField] private Material _yellowHighlight;

    private readonly Dictionary<TransitionType, ITransition> _transitions = new Dictionary<TransitionType, ITransition>();

    private void Awake()
    {
        HideGameOverScreen();
        HideWinScreen();
        _transitions.Add(TransitionType.Fade, new Fade());
    }

    private void OnEnable() {
        GameEvents.OnTransitionINRequested += HandleTransitionIn; 
        GameEvents.OnTransitionOUTRequested += HandleTransitionOut;
        GameEvents.OnShowGameOverRequested += ShowGameOverScreen;
        GameEvents.OnHideGameOverRequested += HideGameOverScreen;
        GameEvents.OnShowWinScreenRequested += ShowWinScreen;
        GameEvents.OnHideWinScreenRequested += HideWinScreen;
        GameEvents.OnFadeOutUIElementRequested += FadeOutUIElement;
        GameEvents.OnFadeInUIElementRequested += FadeInUIElement;
    }

    private void OnDisable() {
        GameEvents.OnTransitionINRequested -= HandleTransitionIn; 
        GameEvents.OnTransitionOUTRequested -= HandleTransitionOut;
        GameEvents.OnShowGameOverRequested -= ShowGameOverScreen;
        GameEvents.OnHideGameOverRequested -= HideGameOverScreen;
        GameEvents.OnShowWinScreenRequested -= ShowWinScreen;
        GameEvents.OnHideWinScreenRequested -= HideWinScreen;
        GameEvents.OnFadeOutUIElementRequested -= FadeOutUIElement;
        GameEvents.OnFadeInUIElementRequested -= FadeInUIElement;
    }

    private void HandleTransitionIn(TransitionType type, Action onComplete)
    {
        if (_transitions.TryGetValue(type, out ITransition activeTransition))
            activeTransition.TransitionIN(canvas: _loadingScreen, speed: _globalTransitionSpeed, onComplete: onComplete);
    }

    private void HandleTransitionOut(TransitionType type, Action onComplete)
    {
        if (_transitions.TryGetValue(type, out ITransition activeTransition))
            activeTransition.TransitionOUT(canvas: _loadingScreen, speed: _globalTransitionSpeed, onComplete: onComplete);
    }

    private void ShowGameOverScreen()
    {
        _gameOverScreen.alpha = 1f;
        _gameOverScreen.interactable = true;
        _gameOverScreen.blocksRaycasts = true;
        Debug.Log("[UIManager] Showing Game Over Screen");
    }

    private void HideGameOverScreen()
    {
        _gameOverScreen.alpha = 0f;
        _gameOverScreen.interactable = false;
        _gameOverScreen.blocksRaycasts = false;
        Debug.Log("[UIManager] Hiding Game Over Screen");
    }
    
    private void ShowPauseScreen()
    {
        _pauseScreen.alpha = 1f;
        _pauseScreen.interactable = true;
        _pauseScreen.blocksRaycasts = true;
        Debug.Log("[UIManager] Showing Pause Screen");
    }
    private void HidePauseScreen()
    {
        _pauseScreen.alpha = 0f;
        _pauseScreen.interactable = false;
        _pauseScreen.blocksRaycasts = false;
        Debug.Log("[UIManager] Hiding Pause Screen");
    }

    private void ShowWinScreen()
    {
        _winScreen.alpha = 1f;
        _winScreen.interactable = true;
        _winScreen.blocksRaycasts = true;
        Debug.Log("[UIManager] Showing Win Screen");
    }
    private void HideWinScreen()
    {
        _winScreen.alpha = 0f;
        _winScreen.interactable = false;
        _winScreen.blocksRaycasts = false;
        Debug.Log("[UIManager] Hiding Win Screen");
    }
    private void FadeOutUIElement(float duration, CanvasGroup canvasGroup = null, Canvas canvas = null)
    {
        if (canvasGroup == null && canvas == null)
        {
            Debug.LogWarning("[UIManager] No CanvasGroup or Canvas provided for fade out.");
            return;
        }
        else if(canvasGroup != null)
        {
            canvasGroup.DOFade(0f, duration);
        }
        else
        {
            CanvasGroup cg = canvas.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = canvas.gameObject.AddComponent<CanvasGroup>();
            }
            cg.DOFade(0f, duration);
        }
    }
    private void FadeInUIElement(float duration, CanvasGroup canvasGroup = null, Canvas canvas = null)
    {
        if (canvasGroup == null && canvas == null)
        {
            Debug.LogWarning("[UIManager] No CanvasGroup or Canvas provided for fade in.");
            return;
        }
        else if(canvasGroup != null)
        {
            canvasGroup.DOFade(1f, duration);
        }
        else
        {
            CanvasGroup cg = canvas.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = canvas.gameObject.AddComponent<CanvasGroup>();
            }
            cg.DOFade(1f, duration);
        }
    }
}