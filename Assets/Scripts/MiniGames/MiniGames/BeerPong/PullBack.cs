using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PullBack : MonoBehaviour
{
    [Header("Pull Back Settings")]
    [SerializeField] private float moveSpeed = 0.02f;
    [SerializeField] private float strenght = 0f;
    [SerializeField] private float minStrenght = 0.25f; 
    [SerializeField] private float maxStrenght = 2.25f;
    [SerializeField] private float startingMousePositionY;
    [Tooltip("Maximum distance the ball can be pulled back, which correlates to the maximum distance we'll track the mouse too.")]
    [SerializeField] private float maxPullBackDistance = 100f;
    [Header("References")]
    [SerializeField] private Slider slider;
    [SerializeField] private BeerPong beerPong;
    private float currentMousePositionYDelta = 0f;
    [SerializeField] private float baseFOV = 50f;
    [SerializeField] private float maxFOV = 60f;
    [SerializeField] private float shakeIntensity = 0.5f;
    private Tween maxTensionShake;
    
    public float Strenght { get => strenght; }
    private CanvasGroup uiGroup;

    private void Awake()
    {
        uiGroup = GetComponentInParent<CanvasGroup>();
    }

    void OnEnable()
    {
        GameEvents.OnRequestMousePositionFromDragBall += RecieveMousePosition;
        if (uiGroup != null)
        {
            uiGroup.alpha = 1f;
            uiGroup.blocksRaycasts = true;
        }
    }

    void OnDisable()
    {
        GameEvents.OnRequestMousePositionFromDragBall -= RecieveMousePosition;
        if (uiGroup != null)
        {
            uiGroup.alpha = 0f;
            uiGroup.blocksRaycasts = false;
        }
    }
    void Start()
    {
        slider.value = 0;
        // to keep things simple mathematically, im keeping the maxPullBackDistance
        // our max strength multiplied by 100, so when we pull the mouses current position
        // and assign it to the slider we'll just divide by 100 to get the actualy strenght!
        maxPullBackDistance = maxStrenght * 100f;
    }
    void Reset()
    {
        slider = GetComponent<Slider>();
        moveSpeed = 0.02f;
        strenght = 0f;
        minStrenght = 0.25f;
        maxStrenght = 2.25f;
    }

    private void OnValidate()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
    }

    private bool isDragging = false;

    private void RecieveMousePosition(float startingMousePositionY)
    {
        this.startingMousePositionY = startingMousePositionY;
        isDragging = true;
    }

    void Update()
    {
        if (beerPong.useQTE) return;

        if (!Input.GetMouseButton(0))
        {
            if (isDragging)
            {
                GameEvents.RequestCameraFOVChange(baseFOV, false, 0.3f);
                
                if (maxTensionShake != null && maxTensionShake.IsActive())
                {
                    maxTensionShake.Kill(true);
                }
                isDragging = false;
            }
        }

        if (!isDragging)
        {
            slider.value = 0f;
            strenght = minStrenght;
            return;
        }

        if (startingMousePositionY < Input.mousePosition.y)
        {
            currentMousePositionYDelta = 0;
        }
        else
        {
            currentMousePositionYDelta = startingMousePositionY - Input.mousePosition.y;
        }
        
        slider.value = Mathf.Abs(currentMousePositionYDelta) / 100;

        if (slider.value < 0f)
        {
            slider.value = 0f;
        }
        else if (slider.value > maxStrenght)
        {
            slider.value = maxStrenght;
        }
        
        float sliderPercentage = slider.value / slider.maxValue;
        
        GameEvents.RequestCameraFOVChange(Mathf.Lerp(baseFOV, maxFOV, sliderPercentage), true);
        
        if (sliderPercentage >= 0.99f)
        {
            if (maxTensionShake == null || !maxTensionShake.IsActive())
            {
                maxTensionShake = Camera.main.transform.DOShakeRotation(0.1f, new Vector3(shakeIntensity, shakeIntensity, 0f), 15).SetLoops(-1);
            }
        }
        else
        {
            if (maxTensionShake != null && maxTensionShake.IsActive())
            {
                maxTensionShake.Kill(true);
            }
        }

        strenght = slider.value * (maxStrenght - minStrenght) + minStrenght;
    }
}