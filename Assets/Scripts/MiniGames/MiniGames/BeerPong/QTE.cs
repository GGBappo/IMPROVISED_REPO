using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class QTE : MonoBehaviour
{
    [Header("QTE Settings")]
    [SerializeField] private float moveSpeed = 0.02f; // Speed of the pointer movement
    [SerializeField] private float strenght = 0f; // Strenght of the throw
    [SerializeField] private Slider slider;
    
    public float Strenght { get => strenght; }

    private void OnValidate()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
    }

    void Update()
    {
        slider.value += moveSpeed;

        if ((slider.value <= 0) && moveSpeed < 0)
        {
            moveSpeed *= -1;
        }
        else if ((slider.value >= 1f) && moveSpeed > 0)
        {
            moveSpeed *= -1;
        }

        strenght = slider.value;
    }

    public void Stop()
    {
        moveSpeed = 0;
    }

    public void Start()
    {
        slider.value = 0;
        moveSpeed = 0.02f;
    }

    public void IncreaseSpeed(float value)
    {
        moveSpeed *= value;
    }
}
