using UnityEngine;

public class Ticket : InteractableItem
{
    public override ItemActionType ActionType => ItemActionType.Special01;

    public override void OnUse()
    {
        base.OnUse();
    }
}
