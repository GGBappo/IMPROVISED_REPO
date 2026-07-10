using UnityEngine;

public class LittleLamp : MonoBehaviour
{
    [SerializeField] Material[] lightColors;
    [SerializeField] Material disabledLight;
    [SerializeField] MeshRenderer render;
    public int current { private set; get; }
    public bool disabled;

    public void NextColor()
    {
        if (disabled)
        {
            render.material = disabledLight;
            return;
        }
        current++;
        if(current >= lightColors.Length)
        {
            current = 0;
        }
        render.material = lightColors[current];
    }
}
