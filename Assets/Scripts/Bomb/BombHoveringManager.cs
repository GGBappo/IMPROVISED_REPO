using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombHoveringManager : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask bombPartMask;
    public LayerMask partOfPartMask;

    static public BombPart hoveredBombPart;
    static public PartOfPart hoveredPartOfPart;

    void Update()
    {
        CheckBombPart();
        CheckPartOfPart();
    }

    private void CheckBombPart()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, bombPartMask))
        {
            BombPart part = hit.collider.GetComponent<BombPart>();

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

    private void CheckPartOfPart()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, partOfPartMask))
        {
            PartOfPart partOP = hit.collider.GetComponent<PartOfPart>();

            if (partOP != null)
            {
                if (hoveredPartOfPart != partOP)
                {
                    if (hoveredPartOfPart != null)
                        hoveredPartOfPart.RemoveHighlight();

                    hoveredPartOfPart = partOP;
                    partOP.Highlight();
                }
                return;
            }
        }

        if (hoveredPartOfPart != null)
        {
            hoveredPartOfPart.RemoveHighlight();
            hoveredPartOfPart = null;
        }
    }
}