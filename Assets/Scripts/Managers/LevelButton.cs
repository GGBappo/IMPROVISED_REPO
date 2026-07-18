using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text _levelNameText;
    [SerializeField] private TMP_Text _moduleAmountText;
    [SerializeField] private TMP_Text _bestTimeText;
    

    public void SetLevelButtonInfo(LevelData levelData)
    {
        _levelNameText.text = levelData.levelName;
        // these are going to be barred for the time being until i implement the GameSession data more thoroughly
        //_moduleAmountText.text = $"Modules: {levelData.moduleCount}";
        //_bestTimeText.text = $"Best Time: {levelData.bestTime:F2}s";
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        GameEvents.RequestSceneUnLoad("StartMenu");
        GameEvents.RequestLevelStart();
    }
}
