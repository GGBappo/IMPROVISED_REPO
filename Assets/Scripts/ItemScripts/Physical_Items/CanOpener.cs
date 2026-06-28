using System.Security.Cryptography;
using UnityEngine;

public class CanOpener : InteractableItem
{
    public override ItemActionType ActionType => ItemActionType.Open;

    public override void OnUse()
    {
        base.OnUse();
    }
}
