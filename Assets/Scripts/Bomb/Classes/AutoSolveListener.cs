using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Marks a component as a LISTENER, not a standalone interactable (e.g. a Disco Ball
/// that reacts when the Party Lights are solved). Subscribes to the gate part's
/// onPartSolved and auto-solves its target. The editor shows a banner so teammates
/// don't wire it as a regular interactable component.
/// Put this on the same GameObject as the listener BombPart (it disables that part's
/// highlight so hover/click ignores it).
/// </summary>
[AddComponentMenu("Improv/Bomb/Auto Solve Listener")]
public class AutoSolveListener : MonoBehaviour
{
    [Tooltip("The part whose onPartSolved gates this listener (e.g. Party Lights).")]
    [SerializeField] BombPart gatePart;

    [Tooltip("The part to auto-solve when the gate solves (e.g. Disco Ball). Leave empty to only fire onGateSolved.")]
    [SerializeField] BombPart targetPart;

    [Tooltip("Extra hook when the gate part solves.")]
    public UnityEvent onGateSolved;

    public BombPart GatePart => gatePart;
    public BombPart TargetPart => targetPart;

    private void OnEnable()
    {
        if (gatePart == null)
        {
            Debug.LogWarning($"[AutoSolveListener] '{name}' has no gate part assigned — it will never fire.", this);
            return;
        }
        gatePart.onPartSolved.AddListener(HandleGateSolved);

        // Listeners are not standalone interactables: kill hover highlight on the part we live on.
        var ownPart = GetComponent<BombPart>();
        if (ownPart != null)
        {
            ownPart.isHighlighted = false;
        }
    }

    private void OnDisable()
    {
        if (gatePart != null)
        {
            gatePart.onPartSolved.RemoveListener(HandleGateSolved);
        }
    }

    private void HandleGateSolved()
    {
        if (targetPart != null && !targetPart.isSolved)
        {
            targetPart.ForceSolve();
        }
        onGateSolved?.Invoke();
    }
}
