using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltips : MonoBehaviour
{
    [SerializeField] private List<string> tooltipLines = new List<string>();

    [Header("Assign the GameObject that has the concrete BombPart script on it")]
    [SerializeField] private GameObject bombPartObject;

    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private GameObject tooltipPanel;

    private BombPart bombPartManager;
    private int strikeCount;

    private void Awake()
    {
        if (bombPartObject != null)
        {
            bombPartManager = bombPartObject.GetComponent<BombPart>();
        }
    }

    private void OnEnable()
    {
        GameEvents.OnStrikeOccurred += HandleStrikeOccurred;
    }

    private void OnDisable()
    {
        GameEvents.OnStrikeOccurred -= HandleStrikeOccurred;
    }

    private void Start()
    {
        HideToolTip();
    }

    private void Update()
    {
        ShowToolTip();
    }

    private void HandleStrikeOccurred(int currentStrikeCount, float penalty)
    {
        strikeCount = currentStrikeCount;
    }

    private void ShowToolTip()
    {
        if (tooltipPanel == null || tooltipText == null || bombPartManager == null || bombPartManager.isSolved)
        {
            HideToolTip();
            return;
        }

        bool isHovered = BombHoveringManager.hoveredBombPart == bombPartManager;

        if (!isHovered)
        {
            if (tooltipPanel.activeSelf && tooltipLines.Contains(tooltipText.text))
                HideToolTip();
            return;
        }

        int index = Mathf.Clamp(strikeCount, 0, tooltipLines.Count - 1);


        if (tooltipPanel.activeSelf && tooltipText.text == tooltipLines[index])
        {
            return; // No need to update if the tooltip is already showing the correct text
        }
        else
        {
            tooltipPanel.SetActive(true);
            tooltipText.text = tooltipLines[index];
        }
    }

    private void HideToolTip()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }
}
