using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static RuntimeSettings;
using TMPro;

public class StartScreenManager : MonoBehaviour
{
    [Header("References")]

    [Header("Markers & Spawn Positions")]

    [SerializeField]
    [Tooltip("These are camera markers for the camera to move to. This array will soon be depricated for a better solution across the entire game.")] 
    private CameraMarkersHolder _cameraMarkerholder;

    [Header("UI References")]
    [SerializeField] 
    private NPCController _npc;
    [SerializeField] 
    private Image _manilaFolderBackdrop;
    [SerializeField]
    private TMP_Text _LevelNameTextBox;
    [SerializeField]
    private TMP_Text _LocationTextBox;
    [SerializeField]
    private TMP_Text _DescriptionBox;
    [SerializeField]
    private GameObject _canvas;
    [SerializeField]
    private GameObject _folderUI;
    [SerializeField]
    private PlayButton _playButton;

    [Header("UI Positions")]
    [SerializeField]
    private Vector2 _closedPosition;
    [SerializeField]
    private Vector2 _openPosition;    

    void OnEnable(){
        GameEvents.OnRequestNPCInteractionSequence += NPCInteraction;  
        GameEvents.OnRequestSettingsMenuOpen += OpenSettingsMenu; 
        GameEvents.OnRequestSettingsMenuClose += CloseSettingsMenu;
        GameEvents.OnRequestOpenFileScreen += OpenFileScreen;
        GameEvents.OnRequestCloseFileScreen += CloseFileScreen;
        GameEvents.OnRequestLatestAssignmentFolderSpawn += ProcessLatestFolderSpawning;
    }

    void OnDisable(){
        GameEvents.OnRequestNPCInteractionSequence -= NPCInteraction;  
        GameEvents.OnRequestSettingsMenuOpen -= OpenSettingsMenu; 
        GameEvents.OnRequestSettingsMenuClose -= CloseSettingsMenu; 
        GameEvents.OnRequestOpenFileScreen -= OpenFileScreen;
        GameEvents.OnRequestCloseFileScreen += CloseFileScreen;
        GameEvents.OnRequestLatestAssignmentFolderSpawn -= ProcessLatestFolderSpawning;
    }

    private void Start()
    {
        GameEvents.RequestCameraMove(_cameraMarkerholder.cameraMarkers[0].transform.position, _cameraMarkerholder.cameraMarkers[0].transform.rotation, 0f);
        CloseFileScreen();
    }

    private void NPCInteraction()
    {
        Sequence startInteraction = DOTween.Sequence();

        startInteraction.AppendCallback(() => GameEvents.RequestCameraMove(_cameraMarkerholder.cameraMarkers[1].transform.position, _cameraMarkerholder.cameraMarkers[1].transform.rotation, defaultTweenDuration));
        startInteraction.Append(_npc.WalkToTarget()); 
        
        startInteraction.AppendCallback(() =>
        {
            GameEvents.RequestShowDialogueUI();
            _npc.Interact();
        });
    }
    
    private void ProcessLatestFolderSpawning()
    {
        GameEvents.DataPassLatestAssignmentFolderSpawn(_npc.transform);
    }

    private void OpenSettingsMenu()
    {
        GameEvents.RequestCameraLookAt(_cameraMarkerholder.cameraMarkers[4].transform.position, defaultTweenDuration, FOV: 16f);
    }

    private void CloseSettingsMenu()
    {
        GameEvents.RequestCameraLookAt(_cameraMarkerholder.cameraMarkers[0].transform.position, defaultTweenDuration);
    }

    private void OpenFileScreen(string levelName, string levelLocation, string levelDescription, int levelIndex)
    {
        _LevelNameTextBox.text = levelName;
        _LocationTextBox.text = levelLocation;
        _DescriptionBox.text = levelDescription;
        _playButton.levelIndex = levelIndex;

        _canvas.SetActive(true);
        _manilaFolderBackdrop.DOColor(new Color(64f,64f,64f,0.4f), defaultTweenDuration).OnComplete(() =>
        {
            _folderUI.transform.DOMoveY(_openPosition.y, defaultTweenDuration).SetEase(Ease.OutSine);
        });
    }

    private void CloseFileScreen()
    {
        _folderUI.transform.DOMoveY(_closedPosition.y, defaultTweenDuration).OnComplete(() =>
        {
            _manilaFolderBackdrop.DOColor(new Color(64f,64f,64f,0f), defaultTweenDuration);
        });
        _canvas.SetActive(false);
    }
}

