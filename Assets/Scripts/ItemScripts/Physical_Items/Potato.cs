using System.Security.Cryptography;
using UnityEngine;

public class Potato : InteractableItem
{
    public override ItemActionType ActionType => ItemActionType.Place;

    public override void OnUse()
    {
        base.OnUse();
    }
}
