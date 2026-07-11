using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tasks : MonoBehaviour
{
    [SerializeField] private string taskText;
    [SerializeField] private string[] taskLists;

    [Header("UI (assign one)")]
    [SerializeField] private Text uiText;
    [SerializeField] private TextMeshProUGUI tmpText;

    private int currentBatchStart;
    private int batchCompletedCount;
    private int totalCompletedCount;

    [SerializeField] private BudgetManager budgetManager;

    private void Start()
    {
        InitializeBatches();
    }

    /// <summary>
    /// Initializes batch indices and displays the first batch (up to 4 tasks).
    /// </summary>
    private void InitializeBatches()
    {
        currentBatchStart = 0;
        batchCompletedCount = 0;
        totalCompletedCount = 0;
        UpdateDisplayedTasks();
    }

    /// <summary>
    /// Call this when a single task in the current visible batch is completed.
    /// The next batch of up to 4 tasks is only loaded once all tasks in the current batch are completed.
    /// </summary>
    public void CompleteTask()
    {
        if (taskLists == null || taskLists.Length == 0)
            return;

        int currentBatchSize = Mathf.Min(4, taskLists.Length - currentBatchStart);
        if (currentBatchSize <= 0)
            return;

        batchCompletedCount = Mathf.Clamp(batchCompletedCount + 1, 0, currentBatchSize);
        totalCompletedCount = Mathf.Clamp(totalCompletedCount + 1, 0, taskLists.Length);

        // Only when entire current batch is completed do we bring in the next batch.
        if (batchCompletedCount >= currentBatchSize)
        {
            AdvanceToNextBatch();
        }
    }

    /// <summary>
    /// Advances to the next batch of up to 4 tasks (if any remain).
    /// </summary>
    private void AdvanceToNextBatch()
    {
        currentBatchStart += Mathf.Min(4, taskLists.Length - currentBatchStart);
        batchCompletedCount = 0;
        UpdateDisplayedTasks();
    }

    private void UpdateDisplayedTasks()
    {
        if (taskLists == null || taskLists.Length == 0)
        {
            SetText(taskText);
            return;
        }

        if (currentBatchStart >= taskLists.Length)
        {
            // All tasks finished — show completion message or leave last batch empty.
            SetText("All tasks completed.");
            return;
        }

        int end = Mathf.Min(currentBatchStart + 4, taskLists.Length);

        var sb = new StringBuilder();
        for (int i = currentBatchStart; i < end; i++)
        {
            sb.AppendLine($"- {taskLists[i]}");
        }

        SetText(sb.ToString().TrimEnd());
    }

    private void SetText(string value)
    {
        if (tmpText != null)
            tmpText.text = value;
        else if (uiText != null)
            uiText.text = value;
        else
            taskText = value; // fallback
    }

    /// <summary>
    /// Set or replace the task list at runtime. Resets batching and displays the first batch.
    /// </summary>
    public void SetTasks(string[] tasks)
    {
        taskLists = tasks;
        InitializeBatches();
    }

    /// <summary>
    /// Optional: force-advance to next batch (useful for debugging or skipping).
    /// </summary>
    public void ForceAdvanceBatch()
    {
        if (taskLists == null || taskLists.Length == 0)
            return;

        if (currentBatchStart < taskLists.Length)
        {
            currentBatchStart += Mathf.Min(4, taskLists.Length - currentBatchStart);
            batchCompletedCount = 0;
            UpdateDisplayedTasks();
        }
    }

    // Simple: remove the first visible task (the one at currentBatchStart) and update UI.
    public void TaskCompleted()
    {
        if (taskLists == null || taskLists.Length == 0)
            return;

        // Remove the first visible task (global index = currentBatchStart).
        int indexToRemove = currentBatchStart;
        if (indexToRemove < 0 || indexToRemove >= taskLists.Length)
            indexToRemove = 0;

        var list = new List<string>(taskLists);
        list.RemoveAt(indexToRemove);
        budgetManager.AddBudget(10); // Add budget for completing a task
        taskLists = list.ToArray();

        // If start is past the end, move it back so we show a valid batch.
        if (currentBatchStart >= taskLists.Length)
        {
            currentBatchStart = Mathf.Max(0, taskLists.Length - Mathf.Min(4, taskLists.Length));
        }

        // Reset counters and refresh.
        batchCompletedCount = 0;
        totalCompletedCount = Mathf.Clamp(totalCompletedCount - 1, 0, taskLists.Length);

        UpdateDisplayedTasks();
    }

    // Debug helper: press Q to remove the first visible task.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TaskCompleted();
        }
    }
}
