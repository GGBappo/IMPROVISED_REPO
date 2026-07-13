using UnityEngine;

public class Ticket : InteractableItem
{
    public override ItemActionType ActionType => ItemActionType.Special1;

    public override void OnUse()
    {
        base.OnUse();
    }
}
