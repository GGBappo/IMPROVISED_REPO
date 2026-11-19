using UnityEngine;

/// <summary>
/// Carries the code/password a Reveal component exposes (sticker, barcode, handwritten note).
/// Drop this on the revealed object and let a CodeEntryPart reference it,
/// so the password physically comes from the reveal instead of being hardcoded.
/// </summary>
[AddComponentMenu("Improv/Bomb/Revealed Code")]
public class RevealedCode : MonoBehaviour
{
    [Tooltip("The code/password this reveal exposes.")]
    public string code;

    [Tooltip("Optional visual for the code (sticker, barcode, handwriting).")]
    public Sprite codeSprite;
}
