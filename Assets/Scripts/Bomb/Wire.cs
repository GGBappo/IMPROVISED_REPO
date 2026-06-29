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
    }

    //I dont think we need them to be Overriden, but it could depend on the part
    public override void Highlight()
    {
        if (isCut) return;
        base.Highlight();
    }

    public override void RemoveHighlight()
    {
        if (isCut) return;
        base.RemoveHighlight();
    }
}
