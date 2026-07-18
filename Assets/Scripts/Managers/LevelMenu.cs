using UnityEngine;

public class LevelMenu : MonoBehaviour
{
    [SerializeField] private GameObject _levelCardPrefab;
    [SerializeField] private Transform _layoutContainer;
    [SerializeField] private LevelDatabase levelDatabase; 

    private void Start()
    {
        BuildMenu();
    }

    private void BuildMenu()
    {
        foreach (LevelData level in levelDatabase.allLevels)
        {
            GameObject newCard = Instantiate(_levelCardPrefab, _layoutContainer);
            LevelButton cardScript = newCard.GetComponent<LevelButton>();
            cardScript.SetLevelButtonInfo(level);
        }
    }
}
