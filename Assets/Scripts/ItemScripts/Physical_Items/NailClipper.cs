using System.Security.Cryptography;
using UnityEngine;

public class NailClipper : InteractableItem
{
    public override ItemActionType ActionType => ItemActionType.Cut;

    public override void OnUse()
    {
        base.OnUse();
    }
}
