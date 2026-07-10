using System.Collections;
using UnityEngine;

public class Wire : PartElement
{
    public bool isCut;
    public bool dontCut;
    public GameObject model;
    public GameObject brokenModel;

    void Update()
    {
        model.SetActive(!isCut);
        brokenModel.SetActive(isCut);
        disabled = isCut;
    }
}
