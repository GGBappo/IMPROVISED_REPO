using UnityEngine;

public class Shoe : InteractableItem
{
    public override ItemActionType ActionType => ItemActionType.Disable;

    public override void OnUse()
    {
        base.OnUse();
    }
}
