using UnityEngine;

public class IceCube : InteractableItem
{
    public override ItemActionType ActionType => ItemActionType.Cool;

    public override void OnUse()
    {
        base.OnUse();
    }
}
