using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class QTE : MonoBehaviour
{
    [Header("QTE Settings")]
    [SerializeField] private float moveSpeed = 0.02f; // Speed of the pointer movement
    [SerializeField] private float strenght = 0f; // Strenght of the throw
    [SerializeField] private Slider slider;
    [SerializeField] private float minStrenght = 0.25f; 
    [SerializeField] private float maxStrenght = 2.25f;
    
    public float Strenght { get => strenght; }
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

        strenght = slider.value * (maxStrenght - minStrenght) + minStrenght;
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
