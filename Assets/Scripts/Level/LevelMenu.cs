using UnityEngine;
using DG.Tweening;
using UnityEngine.Splines;
using UnityEngine.UIElements;
using static RuntimeSettings;

// this class will handle everything related to spawning and file handling!
public class LevelSelector : MonoBehaviour
{
    [SerializeField] private LevelDatabase levelDatabase;
    [SerializeField] private CameraMarkersHolder _cameraMarkersHolder;
    [SerializeField] private GameObject _folderPrefab;
    private GameObject _instantiatedFolder;
    [SerializeField]
    private SplineContainer _folderSplinePath;


    [SerializeField]
    private CanvasGroup _latestAssignmentFolderHoverText;

    [SerializeField]
    private CabinetController _drawer;

    [Header("File Spawning Settings")]
    [SerializeField]
    private float startPadding = 0.1f;
    [SerializeField]
    private float fileThickness = 0.05f;

    void OnEnable()
    {
        GameEvents.OnDataPassLatestAssignmentFolderSpawn += SpawnLatestAssignmentFolder;
        GameEvents.OnRequestDrawerOpen += LevelChooserEntranceSequence; 
        GameEvents.OnRequestDrawerClose += LevelChooserExitSequence;
    }
    
    void OnDisable()
    {
        GameEvents.OnDataPassLatestAssignmentFolderSpawn -= SpawnLatestAssignmentFolder;
        GameEvents.OnRequestDrawerOpen -= LevelChooserEntranceSequence; 
        GameEvents.OnRequestDrawerClose -= LevelChooserExitSequence;
    }

    private void Start()
    {
        _drawer.Close();
        BuildMenu();
    }

    private void BuildMenu()
    {
        BoxCollider spawnArea = _drawer.fileSpawningArea;
        Vector3 localStartPoint = spawnArea.center;

        localStartPoint.z -= spawnArea.size.z / 2f;
        localStartPoint.y -= spawnArea.size.y / 2f;

        float heightOffset = 0.2538f; 
        localStartPoint.y += heightOffset;

        float xOffset = 0.1315f;
        localStartPoint.x -= xOffset;
        
        for (int i = 0; i < levelDatabase.latestLevelIndex; i++)
        {
            Vector3 localSpawnPos = localStartPoint;
            localSpawnPos.z += startPadding + (fileThickness * i);

            Vector3 worldSpawnPos = spawnArea.transform.TransformPoint(localSpawnPos);

            Quaternion spawnRot = Quaternion.Euler(90f, 0f, 0f);

            GameObject file = Instantiate(_folderPrefab, worldSpawnPos, spawnRot, spawnArea.transform);
            LevelButton levelButton = file.GetComponent<LevelButton>();

            levelButton.LevelDataInjection(
                levelDatabase.allLevels[i].levelName,
                levelDatabase.allLevels[i].levelLocation,
                levelDatabase.allLevels[i].levelDescription,
                levelDatabase.allLevels[i].levelIndex
            );
        }
    }

    private void SpawnLatestAssignmentFolder(Transform spawnPosition)
    {
        _instantiatedFolder = Instantiate(_folderPrefab, spawnPosition.transform.position, spawnPosition.transform.rotation);

        LevelButton levelButton = _instantiatedFolder.GetComponent<LevelButton>();
        levelButton.hoverText = _latestAssignmentFolderHoverText;
        levelButton.LevelDataInjection(
            levelDatabase.allLevels[levelDatabase.latestLevelIndex].levelName, 
            levelDatabase.allLevels[levelDatabase.latestLevelIndex].levelLocation, 
            levelDatabase.allLevels[levelDatabase.latestLevelIndex].levelDescription, 
            levelDatabase.latestLevelIndex
        );

        SplineAnimate splineAnimate = _instantiatedFolder.GetComponent<SplineAnimate>();
        splineAnimate.Container = _folderSplinePath;

        splineAnimate.Play();
    }

    private void LevelChooserEntranceSequence()
    {
        Sequence movingSequence = DOTween.Sequence();

        movingSequence.AppendCallback(() => GameEvents.RequestCameraMove(_cameraMarkersHolder.cameraMarkers[2].transform.position, _cameraMarkersHolder.cameraMarkers[2].transform.rotation, defaultTweenDuration));
        movingSequence.Append(_drawer.Open());
    }

    private void LevelChooserExitSequence()
    {
        Sequence movingSequence = DOTween.Sequence();

        movingSequence.AppendCallback(() => GameEvents.RequestCameraMove(_cameraMarkersHolder.cameraMarkers[0].transform.position, _cameraMarkersHolder.cameraMarkers[0].transform.rotation, defaultTweenDuration));
        movingSequence.Append(_drawer.Close());
    }
}
