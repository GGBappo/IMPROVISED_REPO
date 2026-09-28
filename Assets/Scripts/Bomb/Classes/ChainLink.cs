using UnityEngine;

public enum ChainAction { Unlock, Solve, SilentLock }

/// <summary>
/// Foolproof one-drag wiring: "When <gatePart> solves -> <action> <targetPart>".
/// Replaces hunting for methods in the UnityEvent dropdown for the most common gate.
/// Raw UnityEvents on the parts remain available for anything more exotic.
/// </summary>
[AddComponentMenu("Improv/Bomb/Chain Link")]
public class ChainLink : MonoBehaviour
{
    [Tooltip("The part whose onPartSolved triggers this link.")]
    [SerializeField] BombPart gatePart;

    [Tooltip("What happens to the target when the gate solves.")]
    [SerializeField] ChainAction action = ChainAction.Unlock;

    [Tooltip("The part the action is applied to.")]
    [SerializeField] BombPart targetPart;

    public BombPart GatePart => gatePart;
    public BombPart TargetPart => targetPart;
    public ChainAction Action => action;

    private void OnEnable()
    {
        if (gatePart == null)
        {
            Debug.LogWarning($"[ChainLink] '{name}' has no gate part assigned — it will never fire.", this);
            return;
        }
        gatePart.onPartSolved.AddListener(Handle);
    }

    private void OnDisable()
    {
        if (gatePart != null)
        {
            gatePart.onPartSolved.RemoveListener(Handle);
        }
    }

    private void Handle()
    {
        if (targetPart == null) return;

        switch (action)
        {
            case ChainAction.Unlock: targetPart.Unlock(); break;
            case ChainAction.Solve: targetPart.ForceSolve(); break;
            case ChainAction.SilentLock: targetPart.SilentLock(); break;
        }
    }
}
