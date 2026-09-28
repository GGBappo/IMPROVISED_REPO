using UnityEngine;
using TMPro;

public enum CodeEntryMode { Keypad, SymbolOrder }

/// <summary>
/// CODE ENTRY component (passwords/keypads):
/// - Keypad mode: bare-click the part to start entry, type the code on the keyboard,
///   press Enter to submit, Esc to cancel. The required code can be typed directly
///   or dragged in from a RevealedCode (the password physically comes from a reveal).
/// - SymbolOrder mode: reuses the existing Symbol/SymbolIcon elements — click the
///   symbols to cycle their colors until they match the code order.
/// Wrong submissions fire onPartWrongItem (strike), like any wrong item.
/// </summary>
[AddComponentMenu("Improv/Bomb/Code Entry Part")]
public class CodeEntryPart : BombPart
{
    [Header("Mode")]
    [Tooltip("Keypad = typed code. SymbolOrder = cycle Symbol colors like SymbolPuzzlePart.")]
    [SerializeField] CodeEntryMode mode = CodeEntryMode.Keypad;

    [Header("Keypad")]
    [Tooltip("Required code. If a RevealedCode source is assigned below, it overrides this string.")]
    [SerializeField] string requiredCode = "1234";

    [Tooltip("Optional: drag the reveal that exposes this part's code. Overrides requiredCode.")]
    [SerializeField] RevealedCode codeSource;

    [Tooltip("Optional world-space readout of what the player has typed.")]
    [SerializeField] TMP_Text codeDisplay;

    [Tooltip("Maximum characters the entry accepts.")]
    [SerializeField] int maxCodeLength = 8;

    [Header("Symbol Order")]
    [SerializeField] Symbol[] symbols;
    [SerializeField] int[] symbolCode;

    private bool entering;
    private string entry = "";

    private string EffectiveCode => codeSource != null ? codeSource.code : requiredCode;

    private void OnValidate()
    {
        // Code entry parts are always bare-click — never require an item.
        if (!interactByClick) interactByClick = true;
    }

    private void Update()
    {
        if (!entering || isSolved || isLocked) return;
        if (mode != CodeEntryMode.Keypad) return;

        foreach (char c in Input.inputString)
        {
            if (c == '\b')
            {
                if (entry.Length > 0) entry = entry.Substring(0, entry.Length - 1);
                UpdateDisplay();
            }
            else if (c == '\n' || c == '\r')
            {
                Submit();
            }
            else if (c == 27) // Esc
            {
                Cancel();
            }
            else if (char.IsLetterOrDigit(c) && entry.Length < maxCodeLength)
            {
                entry += c;
                UpdateDisplay();
            }
        }
    }

    public override bool OnItemUsed(ItemActionType[] itemActions)
    {
        var ctx = new UseContext { itemActions = itemActions, elements = mode == CodeEntryMode.SymbolOrder ? symbols : null };

        if (!UseBase(ref ctx))
        {
            return false;
        }

        switch (mode)
        {
            case CodeEntryMode.Keypad:
                entering = true;
                UpdateDisplay();
                break;

            case CodeEntryMode.SymbolOrder:
                int elementID = ctx.hoveredIndex;
                symbols[elementID].anim.SetTrigger("Click");
                symbols[elementID].icon.NextColor();
                symbols[elementID].RemoveHighlight();

                if (SymbolsMatch())
                {
                    Solve();
                }
                break;
        }
        return true;
    }

    private void Submit()
    {
        if (entry == EffectiveCode)
        {
            entering = false;
            entry = "";
            UpdateDisplay();
            Solve();
        }
        else
        {
            onPartWrongItem?.Invoke();
            entry = "";
            UpdateDisplay();
        }
    }

    private void Cancel()
    {
        entering = false;
        entry = "";
        UpdateDisplay();
    }

    private bool SymbolsMatch()
    {
        if (symbols == null || symbolCode == null || symbols.Length != symbolCode.Length) return false;

        for (int i = 0; i < symbols.Length; i++)
        {
            if (symbols[i].icon.current != symbolCode[i])
            {
                return false;
            }
        }
        return true;
    }

    private void UpdateDisplay()
    {
        if (codeDisplay != null)
        {
            codeDisplay.SetText(entering ? entry : "");
        }
    }
}
