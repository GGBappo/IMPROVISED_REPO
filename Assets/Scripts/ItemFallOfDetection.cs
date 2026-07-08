using UnityEngine;

public class ItemFallOfDetection : MonoBehaviour
{
    [SerializeField] private ItemShopPanel itemShopPanel;

    private void OnTriggerEnter(Collider other)
    {
        InteractableItem item = other.GetComponent<InteractableItem>();

        if (item == null) return;

        Vector3 spawnPoint = itemShopPanel.GetSpawnPoint(item);
        item.transform.position = spawnPoint;
    }
}