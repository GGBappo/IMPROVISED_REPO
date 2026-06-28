using System.Security.Cryptography;
using UnityEngine;

public class RubberDuck : InteractableItem
{
    public override ItemActionType ActionType => ItemActionType.Squeak;

    public override void OnUse()
    {
        base.OnUse();
    }
}
