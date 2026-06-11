using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

//Controls all the interactions with the items in the inventory,
//such as dragging and dropping, using, and hovering over them.
public class InteractableItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public Item_SO itemData;
    public Vector3 spawnPos;
    private Vector3 spawnRot;

    private Collider itemCollider;
    private Rigidbody rb;
    private bool wasGravityEnabled;

    private BudgetManager budgetManager;
    private ItemShopPanel itemShopPanel;

    private bool isHovered = false;
    private bool isDragging = false;

    private void Awake()
    {
        budgetManager = FindObjectOfType<BudgetManager>();
        itemShopPanel = FindObjectOfType<ItemShopPanel>();
    }   

    private void Start()
    {
        spawnPos = transform.position;
        spawnRot = transform.eulerAngles;
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            wasGravityEnabled = rb.useGravity;
        }
    }

    private void Update()
    {
        if (itemData == null) return;

        // If not being interacted with, do not apply any updates
        if (!isHovered && !isDragging) return;

        float targetY = spawnPos.y;
        float targetRotY = spawnRot.y;

        if (isDragging)
        {
            targetY = itemData.dragHeight;
            targetRotY = itemData.dragRot;
        }
        else if (isHovered)
        {
            targetY = itemData.hoverHeight;
            targetRotY = itemData.hoverRot;
        }

        if (isHovered && !isDragging && Input.GetMouseButtonDown(1))
        {
            OnRightClick();
        }

            // Smoothly transition Y position
            Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * 10f);

        if (isDragging)
        {
            Vector3 dragPos = transform.position;
            dragPos.y = pos.y;
            transform.position = dragPos;
        }
        else
        {
            transform.position = pos;
        }

        // Smoothly transition Y rotation
        Quaternion targetRotation = Quaternion.Euler(spawnRot.x, targetRotY, spawnRot.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    private void UpdateGravityState()
    {
        if (rb != null)
        {
            if (isDragging || isHovered)
            {
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                rb.useGravity = wasGravityEnabled;
            }
        }
    }

    public virtual void OnUse()
    {
        if (BombHoveringManager.hoveredBombPart != null)
        {
            if (BombHoveringManager.hoveredBombPart.OnItemUsed(itemData.itemName))
            {
                //Item Used Succesfully
            }
            else
            {
                //Item Used Unsuccesfully (strike was already added)
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging) return;
        isHovered = true;
        UpdateGravityState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging) return;
        isHovered = false;
        UpdateGravityState();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        UpdateGravityState();
        itemCollider = GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        isHovered = false;
        UpdateGravityState();

        if (itemCollider != null)
        {
            itemCollider.enabled = true;
        }

        // Reset spawnPos to the drop location if it's placed in the world
        spawnPos = new Vector3(transform.position.x, spawnPos.y, transform.position.z);

        OnUse();
        Debug.Log("Dropped: " + gameObject.name);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Camera cam = eventData.pressEventCamera != null ? eventData.pressEventCamera : Camera.main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // Raycast against the environment/world
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        }
        else
        {
            // Fallback: project onto a horizontal plane at spawnPos height
            Plane plane = new Plane(Vector3.up, spawnPos);
            if (plane.Raycast(ray, out float enter))
            {
                Vector3 planePoint = ray.GetPoint(enter);
                transform.position = new Vector3(planePoint.x, transform.position.y, planePoint.z);
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
    }

    // Fallbacks for standard physics mouse events
    private void OnMouseEnter()
    {
        if (isDragging) return;
        isHovered = true;
        UpdateGravityState();
    }

    private void OnMouseExit()
    {
        if (isDragging) return;
        isHovered = false;
        UpdateGravityState();
    }

    private void OnMouseDown()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        OnBeginDrag(eventData);
    }

    private void OnMouseDrag()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        OnDrag(eventData);
    }

    private void OnMouseUp()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        OnEndDrag(eventData);
    }

    //While Item is not being dragged or used and the mouse is on it and then
    //the player right clicks, the item will be sold. 
    private void OnRightClick()
    {
        budgetManager.SellItem(itemData);

        Destroy(gameObject);

        itemShopPanel.FreeSpawnPoint();

        Debug.Log("Sold: " + gameObject.name);
        budgetManager.CurrentMoney();
    }
}
