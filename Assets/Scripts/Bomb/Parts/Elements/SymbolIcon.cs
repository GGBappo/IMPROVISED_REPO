using UnityEngine;

public class SymbolIcon : MonoBehaviour
{
    [SerializeField] Material[] iconMat;
    [SerializeField] Material disabledMat;
    [SerializeField] MeshRenderer render;
    public int current { private set; get; }
    public bool disabled;

    public void NextColor()
    {
        if (disabled)
        {
            render.material = disabledMat;
            return;
        }
        current++;
        if(current >= iconMat.Length)
        {
            current = 0;
        }
        render.material = iconMat[current];
    }
}
