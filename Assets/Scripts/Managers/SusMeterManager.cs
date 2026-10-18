using UnityEngine;
using UnityEngine.UI;

public class SusMeterManager : MonoBehaviour
{
    [SerializeField] private Slider susSlider;
    [SerializeField] private int maxSusAmount = 3;
    
    private int currentSusAmount = 0;

    private void OnEnable()
    {
        GameEvents.OnPingPongBallMissedCup += HandleMiss;
    }

    private void OnDisable()
    {
        GameEvents.OnPingPongBallMissedCup -= HandleMiss;
    }

    private void HandleMiss()
    {
        currentSusAmount++;
        UpdateUI();

        if (currentSusAmount >= maxSusAmount)
        {
            GameEvents.RequestShowGameOverScreen();
        }
    }

    private void UpdateUI()
    {
        if (susSlider != null)
        {
            susSlider.maxValue = maxSusAmount;
            susSlider.value = currentSusAmount;
        }
    }
}