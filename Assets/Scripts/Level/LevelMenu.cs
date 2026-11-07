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
        GameEvents.OnRequestDrawerOpen += DrawerEntranceSequence;
        GameEvents.OnRequestDrawerClose += DrawerExitSequence;
    }
    
    void OnDisable()
    {
        GameEvents.OnDataPassLatestAssignmentFolderSpawn -= SpawnLatestAssignmentFolder;
        GameEvents.OnRequestDrawerOpen -= DrawerEntranceSequence;
        GameEvents.OnRequestDrawerClose -= DrawerExitSequence;
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

    private void DrawerEntranceSequence()
    {
        Sequence movingSequence = DOTween.Sequence();

        movingSequence.Append(_drawer.Open());
    }

    private void DrawerExitSequence()
    {
        Sequence movingSequence = DOTween.Sequence();

        movingSequence.Append(_drawer.Close());
    }
}
