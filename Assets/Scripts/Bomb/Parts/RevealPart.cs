using UnityEngine;

/// <summary>
/// REVEAL component: solving needs no cut, code or item — a single bare click
/// physically peels/wipes/pops the part open to expose a sticker, barcode or
/// handwritten note (see RevealedCode), then solves like any part so it can gate
/// or chain onward. Nothing is consumed.
/// </summary>
[AddComponentMenu("Improv/Bomb/Reveal Part")]
public class RevealPart : BombPart
{
    [Header("Reveal")]
    [Tooltip("Hidden object exposed on reveal (the sticker, barcode or note). Usually starts inactive.")]
    [SerializeField] GameObject revealTarget;

    [Tooltip("Optional peel/wipe/pop animation, triggered with 'Reveal'.")]
    [SerializeField] Animator revealAnim;

    [Tooltip("Optional code this reveal exposes, for CodeEntryParts to reference.")]
    [SerializeField] RevealedCode revealedCode;

    [Tooltip("Activate the reveal target on reveal (uncheck to only play the animation).")]
    [SerializeField] bool activateTarget = true;

    private void OnValidate()
    {
        // Reveal parts are always bare-click — never require an item.
        if (!interactByClick) interactByClick = true;
    }

    public override bool OnItemUsed(ItemActionType[] itemActions)
    {
        var ctx = new UseContext { itemActions = itemActions };

        if (!UseBase(ref ctx))
        {
            return false;
        }

        PerformReveal();
        Solve();
        return true;
    }

    /// <summary> Flips the hidden object visible and/or plays the peel animation. </summary>
    public void PerformReveal()
    {
        if (activateTarget && revealTarget != null && !revealTarget.activeSelf)
        {
            revealTarget.SetActive(true);
        }
        if (revealAnim != null)
        {
            revealAnim.SetTrigger("Reveal");
        }
    }
}
