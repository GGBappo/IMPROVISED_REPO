using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    
    void OnEnable()
    {
        GameEvents.OnRequestShowLevelUI += ShowLevelUI;
        GameEvents.OnRequestHideLevelUI += HideLevelUI;
    }
    void OnDisable()
    {
        GameEvents.OnRequestShowLevelUI += ShowLevelUI;
        GameEvents.OnRequestHideLevelUI -= HideLevelUI;
    }

    private void ShowLevelUI()
    {
        GameEvents.RequestShowBudgetUI();
        GameEvents.RequestHideQTE();
        GameEvents.RequestShowShop();
    }

    private void HideLevelUI()
    {
        GameEvents.RequestHideBudgetUI();
        Debug.Log("Ph1");
        GameEvents.RequestShowQTE();
        Debug.Log("Ph2");
        GameEvents.RequestHideShop();
        Debug.Log("Ph3");
    }
}
