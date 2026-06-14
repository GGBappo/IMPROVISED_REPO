using TMPro;
using UnityEngine;

/// <summary>
/// Simple bomb countdown timer.
/// - Displays remaining time in mm:ss on a TextMeshProUGUI.
/// - Can be started, stopped, reset and seeded with a time value.
/// - Updates UI every frame while running.
/// </summary>
public class BombTimer : MonoBehaviour
{
    // Reference to the TextMeshPro UI element where the timer is shown.
    // Assign this in the Inspector by dragging your TMP text object.
    [SerializeField] private TextMeshProUGUI bombTimer;

    // Initial time (serialized for easy tuning in Inspector) in seconds.
    [SerializeField] private int timeSeconds;

    // If true the timer will start automatically during Awake.
    [SerializeField] private bool startOnAwake;

    // Internal remaining time in seconds (float so we can decrement by Time.deltaTime).
    private float remaining;

    // Whether the timer is currently counting down.
    private bool running;

    private void Awake()
    {
        // Ensure remaining is non-negative and initialized to the serialized value.
        remaining = Mathf.Max(0, timeSeconds);

        // Optionally start the timer immediately on Awake.
        if (startOnAwake) StartTimer();

        // Make sure the UI shows the current value even if not running.
        UpdateTimerText();
    }

    private void Update()
    {
        // Only decrement time if the timer is running.
        if (!running) return;

        if (remaining > 0f)
        {
            // Subtract elapsed frame time.
            remaining -= Time.deltaTime;

            // When we hit zero stop and clamp.
            if (remaining <= 0f)
            {
                remaining = 0f;
                running = false;

                // TODO: add hook/event here to notify other systems the bomb exploded or timer expired.
            }

            // Update the displayed text each frame while counting down.
            UpdateTimerText();
        }
    }

    /// <summary>
    /// Set the timer value (in seconds) and immediately update the display.
    /// This does not start the countdown unless you call StartTimer().
    /// </summary>
    public void SetTime(int seconds)
    {
        timeSeconds = Mathf.Max(0, seconds);
        remaining = timeSeconds;
        UpdateTimerText();
    }

    /// <summary>
    /// Start counting down from the current remaining time.
    /// If remaining is zero, it will be reinitialized from the serialized timeSeconds.
    /// </summary>
    public void StartTimer()
    {
        if (remaining <= 0f) remaining = Mathf.Max(0, timeSeconds);
        running = remaining > 0f;
        UpdateTimerText();
    }

    /// <summary>
    /// Stop/pause the countdown. Current remaining time is preserved.
    /// </summary>
    public void StopTimer()
    {
        running = false;
        UpdateTimerText();
    }

    /// <summary>
    /// Reset the remaining time to the serialized initial value and stop the timer.
    /// </summary>
    public void ResetTimer()
    {
        running = false;
        remaining = Mathf.Max(0, timeSeconds);
        UpdateTimerText();
    }

    /// <summary>
    /// Formats remaining time as mm:ss and writes it to the assigned TMP text.
    /// Uses Ceil so UI shows '00:01' until the last frame transitions to zero.
    /// </summary>
    private void UpdateTimerText()
    {
        if (bombTimer == null)
        {
            Debug.LogWarning($"BombTimer: TextMeshProUGUI reference not assigned on '{name}'.");
            return;
        }

        // Convert remaining float to whole seconds (ceiling so UI doesn't drop prematurely).
        int secs = Mathf.CeilToInt(remaining);
        int minutes = secs / 60;
        int seconds = secs % 60;

        // Format with leading zeros: "MM:SS"
        bombTimer.text = $"{minutes:00}:{seconds:00}";
    }

#if UNITY_EDITOR
    // Called in the Editor when a serialized field changes in the Inspector.
    // Keeps the displayed text in sync with edited values while not playing.
    private void OnValidate()
    {
        remaining = Mathf.Max(0, timeSeconds);
        if (!Application.isPlaying) UpdateTimerText();
    }
#endif
}
