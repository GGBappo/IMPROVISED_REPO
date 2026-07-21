using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombHoveringManager : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask bombPartMask;
    public LayerMask partElementMask;

    static public BombPart hoveredBombPart;
    static public PartElement hoveredPartElement;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        CheckBombPart();
        CheckPartElement();
        if (Input.GetMouseButtonDown(0) && hoveredBombPart != null && hoveredBombPart.dontNeedTool)
        {
            hoveredBombPart.OnItemUsed(ItemActionType.Empty);
        }
    }

    private void CheckBombPart()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, bombPartMask))
        {
            BombPart part = hit.collider.GetComponentInParent<BombPart>();

            if (part != null)
            {
                if (hoveredBombPart != part)
                {
                    if (hoveredBombPart != null)
                        hoveredBombPart.RemoveHighlight();

                    hoveredBombPart = part;
                    part.Highlight();
                }
                return;
            }
        }

        if (hoveredBombPart != null)
        {
            hoveredBombPart.RemoveHighlight();
            hoveredBombPart = null;
        }
    }

    private void CheckPartElement()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, partElementMask))
        {
            PartElement partElement = hit.collider.GetComponent<PartElement>();

            if (partElement != null)
            {
                if (hoveredPartElement != partElement)
                {
                    if (hoveredPartElement != null)
                    {
                        hoveredPartElement.RemoveHighlight();
                        hoveredPartElement.SetHover(false);
                    }

                    hoveredPartElement = partElement;
                    partElement.Highlight();
                    partElement.SetHover(true);
                }
                return;
            }
        }

        if (hoveredPartElement != null)
        {
            hoveredPartElement.RemoveHighlight();
            hoveredPartElement.SetHover(false);
            hoveredPartElement = null;
        }
    }
}