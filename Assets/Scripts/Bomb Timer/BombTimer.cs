using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Simple bomb countdown timer with an exponential strike penalty system.
/// - Call <see cref="RegisterStrike"/> when a wrong mistake occurs; each strike removes time.
/// - Penalty increases per strike (exponential growth by default).
/// - Raises optional events when a strike occurs or when the timer expires.
/// </summary>
public class BombTimer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI bombTimer; // TMP field to show mm:ss

    [Header("Timer")]
    [SerializeField] private int timeSeconds = 60; // initial time (seconds)
    [SerializeField] private bool startOnAwake;

    [Header("Strike / Penalty")]
    [SerializeField] private float baseStrikePenaltySeconds = 5f; // penalty for first strike (seconds)
    [SerializeField] private float strikePenaltyMultiplier = 1.5f; // multiplier per subsequent strike (exponential)

    // Current runtime state
    private float remaining; // remaining time in seconds (float for smooth decrement)
    private bool running;
    private int strikeCount;

    private void Awake()
    {
        // initialize remaining time and optionally start
        remaining = Mathf.Max(0, timeSeconds);
        strikeCount = 0;
        if (startOnAwake) StartTimer();
        UpdateTimerText();
    }

    private void Update()
    {
        if (!running) return;

        // decrement timer while running
        if (remaining > 0f)
        {
            remaining -= Time.deltaTime;

            if (remaining <= 0f)
            {
                // clamp, stop, and notify
                remaining = 0f;
                running = false;
                GameEvents.TimerExpired();
            }

            UpdateTimerText();
        }
    }

    /// <summary>
    /// Registers a strike (wrong mistake). This removes time from the remaining timer.
    /// Penalty applied = baseStrikePenaltySeconds * (strikePenaltyMultiplier ^ (strikeCount-1)).
    /// </summary>
    public void RegisterStrike()
    {
        if (remaining <= 0f) return; // nothing to do if timer already finished

        // increment strike count and compute exponential penalty
        strikeCount = Mathf.Max(0, strikeCount) + 1;
        float penalty = baseStrikePenaltySeconds * Mathf.Pow(strikePenaltyMultiplier, strikeCount - 1);

        // subtract penalty from remaining time
        remaining = Mathf.Max(0f, remaining - penalty);

        // notify listeners about the strike and update UI
        GameEvents.StrikeOccurred(strikeCount, penalty);
        UpdateTimerText();

        // if time is up after penalty, trigger expiry
        if (remaining <= 0f)
        {
            remaining = 0f;
            running = false;
            GameEvents.TimerExpired();
        }
    }

    /// <summary>
    /// Set the timer value in seconds (does not start by itself).
    /// </summary>
    public void SetTime(int seconds)
    {
        timeSeconds = Mathf.Max(0, seconds);
        remaining = timeSeconds;
        strikeCount = 0;
        UpdateTimerText();
    }

    /// <summary>
    /// Start or resume the timer.
    /// </summary>
    public void StartTimer()
    {
        if (remaining <= 0f) remaining = Mathf.Max(0, timeSeconds);
        running = remaining > 0f;
        UpdateTimerText();
    }

    /// <summary>
    /// Stop/pause the timer.
    /// </summary>
    public void StopTimer()
    {
        running = false;
        UpdateTimerText();
    }

    /// <summary>
    /// Reset the timer to the serialized initial value and clear strikes.
    /// </summary>
    public void ResetTimer()
    {
        running = false;
        strikeCount = 0;
        remaining = Mathf.Max(0, timeSeconds);
        UpdateTimerText();
    }

    /// <summary>
    /// Formats remaining time as mm:ss and writes to the assigned TMP text.
    /// </summary>
    private void UpdateTimerText()
    {
        if (bombTimer == null)
        {
            Debug.LogWarning($"BombTimer: TextMeshProUGUI reference not assigned on '{name}'.");
            return;
        }

        int secs = Mathf.CeilToInt(remaining); // ceil so 1.9s still shows as 02 until the last frame
        int minutes = secs / 60;
        int seconds = secs % 60;
        bombTimer.text = $"{minutes:00}:{seconds:00}";
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        // Keep sane serialized values and keep UI updated in editor
        baseStrikePenaltySeconds = Mathf.Max(0f, baseStrikePenaltySeconds);
        strikePenaltyMultiplier = Mathf.Max(1f, strikePenaltyMultiplier);
        timeSeconds = Mathf.Max(0, timeSeconds);
        remaining = Mathf.Max(0, timeSeconds);

        if (!Application.isPlaying) UpdateTimerText();
    }
#endif
}
