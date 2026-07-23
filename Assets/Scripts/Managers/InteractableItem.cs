using System.Collections;
using System.Collections.Generic;
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
    private bool isReturning = false;

    [Header("Special Item Spawn Points")]
    [SerializeField] private List<Transform> specialSpawnPoints = new List<Transform>();

    private Coroutine returner;

    public virtual ItemActionType ActionType
    {
        get
        {
            Debug.LogWarning(gameObject.name + " does not override ActionType!");
            return ItemActionType.Cut;
        }
    }

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
        if (itemData == null || isReturning) return;

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
            if (isDragging || isHovered || isReturning)
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

    private void StopReturnRoutine()
    {
        if (returner != null)
        {
            StopCoroutine(returner);
            returner = null;
        }

        isReturning = false;
        UpdateGravityState();
    }

    public virtual void OnUse()
    {
        if (BombHoveringManager.hoveredBombPart != null)
        {
            if (BombHoveringManager.hoveredBombPart.OnItemUsed(ActionType))
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
        if (isDragging || isReturning) return;
        isHovered = true;
        UpdateGravityState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging || isReturning) return;
        isHovered = false;
        UpdateGravityState();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isReturning) return;

        StopReturnRoutine();

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
        if (isReturning) return;

        isDragging = false;
        isHovered = false;
        UpdateGravityState();

        if (itemCollider != null)
        {
            itemCollider.enabled = true;
        }

        OnUse();
        OnDrop(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        LocateSpecialItem(this);

        if (isReturning) return;

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
        if (isReturning) return;

        Debug.Log("Dropped on: " + gameObject.name);

        StopReturnRoutine();
        returner = StartCoroutine(ReturnToSpawnPoint());
    }

    // Fallbacks for standard physics mouse events
    private void OnMouseEnter()
    {
        if (isDragging || isReturning) return;
        isHovered = true;
        UpdateGravityState();
    }

    private void OnMouseExit()
    {
        if (isDragging || isReturning) return;
        isHovered = false;
        UpdateGravityState();
    }

    private void OnMouseDown()
    {
        if (isReturning) return;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        OnBeginDrag(eventData);
    }

    private void OnMouseDrag()
    {
        if (isReturning) return;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        OnDrag(eventData);
    }

    private void OnMouseUp()
    {
        if (isReturning) return;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        OnEndDrag(eventData);
    }

    //While Item is not being dragged or used and the mouse is on it and then
    //the player right clicks, the item will be sold.
    private void OnRightClick()
    {
        budgetManager.SellItem(itemData);
        itemShopPanel.FreeSpawnPoint(this);

        Destroy(gameObject);

        Debug.Log("Sold: " + gameObject.name);
        budgetManager.CurrentMoney();
    }

    //Used for when item is let go. Item when let go returns to spawnpoint in which it came from.
    public IEnumerator ReturnToSpawnPoint()
    {

        Debug.Log("Waiting 3 seconds Returning " + gameObject.name + " to spawn point.");

        yield return new WaitForSeconds(3f);

        isReturning = true;
        UpdateGravityState();

        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }

        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Vector3 targetPosition = spawnPos;
        Quaternion targetRotation = Quaternion.Euler(spawnRot);

        if (CompareTag("Special-Item"))
        {
            Transform specialSpawnPoint = GetAvailableSpecialSpawnPoint();
            if (specialSpawnPoint != null)
            {
                targetPosition = specialSpawnPoint.position;
                targetRotation = specialSpawnPoint.rotation;
            }
        }

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = wasGravityEnabled;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (itemCollider != null)
        {
            itemCollider.enabled = true;
        }

        isReturning = false;
        returner = null;

        Debug.Log("Returned " + gameObject.name + " to spawn point.");
    }

    private Transform GetAvailableSpecialSpawnPoint()
    {
        for (int i = 0; i < specialSpawnPoints.Count; i++)
        {
            if (specialSpawnPoints[i] == null)
            {
                continue;
            }

            if (specialSpawnPoints[i].childCount == 0)
            {
                return specialSpawnPoints[i];
            }
        }

        return null;
    }

    private void LocateSpecialItem(InteractableItem item)
    {
        if(item.CompareTag("Special-Item"))
        {
            Debug.Log("Special Item Found: " + item.name);
        }
    }
}
