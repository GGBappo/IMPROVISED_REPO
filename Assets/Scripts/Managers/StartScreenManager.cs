using UnityEngine;
using DG.Tweening;
using static RuntimeSettings;

public class StartScreenManager : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] _cameraMarkers;
    [SerializeField] 
    private NPCController _npc; 
    [SerializeField] 
    private GameObject _dialogueCanvas;
    [SerializeField]
    private CabinetController _drawer;
    [SerializeField]
    private GameObject _folderSpawnPosition;
    [SerializeField]
    private GameObject _folder;

    void OnEnable(){
        GameEvents.OnRequestNPCInteractionSequence += NPCInteraction; 
        GameEvents.OnRequestDrawerOpen += LevelChooserEntranceSequence; 
        GameEvents.OnRequestDrawerClose += LevelChooserExitSequence; 
        GameEvents.OnRequestSettingsMenuOpen += OpenSettingsMenu; 
        GameEvents.OnRequestSettingsMenuClose += CloseSettingsMenu;
        GameEvents.OnRequestLatestAssignmentFolderSpawn += SpawnLatestAssignmentFolder;
    }
    void OnDisable(){
        GameEvents.OnRequestNPCInteractionSequence -= NPCInteraction; 
        GameEvents.OnRequestDrawerOpen -= LevelChooserEntranceSequence; 
        GameEvents.OnRequestDrawerClose -= LevelChooserExitSequence; 
        GameEvents.OnRequestSettingsMenuOpen -= OpenSettingsMenu; 
        GameEvents.OnRequestSettingsMenuClose -= CloseSettingsMenu; 
        GameEvents.OnRequestLatestAssignmentFolderSpawn -= SpawnLatestAssignmentFolder;
    }

    private void Start()
    {
        GameEvents.RequestCameraMove(_cameraMarkers[0].transform.position, _cameraMarkers[0].transform.rotation, 0f);
        _drawer.Close();
    }

    private void NPCInteraction()
    {
        Sequence startInteraction = DOTween.Sequence();

        startInteraction.AppendCallback(() => GameEvents.RequestCameraMove(_cameraMarkers[2].transform.position, _cameraMarkers[2].transform.rotation, defaultTweenDuration));
        startInteraction.Append(_npc.WalkToTarget()); 
        
        startInteraction.AppendCallback(() => _dialogueCanvas.SetActive(true));
    }
    
    private void LevelChooserEntranceSequence()
    {
        Sequence movingSequence = DOTween.Sequence();

        movingSequence.AppendCallback(() => GameEvents.RequestCameraMove(_cameraMarkers[4].transform.position, _cameraMarkers[4].transform.rotation, defaultTweenDuration));
        movingSequence.Append(_drawer.Open());
    }

    private void LevelChooserExitSequence()
    {
        Sequence movingSequence = DOTween.Sequence();

        movingSequence.AppendCallback(() => GameEvents.RequestCameraMove(_cameraMarkers[0].transform.position, _cameraMarkers[0].transform.rotation, defaultTweenDuration));
        movingSequence.Append(_drawer.Close());
    }

    private void OpenSettingsMenu()
    {
        GameEvents.RequestCameraLookAt(_cameraMarkers[1].transform.position, defaultTweenDuration, FOV: 16f);
    }
    private void CloseSettingsMenu()
    {
        GameEvents.RequestCameraLookAt(_cameraMarkers[0].transform.position, defaultTweenDuration);
    }

    private void SpawnLatestAssignmentFolder()
    {
        Instantiate(_folder, _folderSpawnPosition.transform.position, _folderSpawnPosition.transform.rotation);
    }
}

