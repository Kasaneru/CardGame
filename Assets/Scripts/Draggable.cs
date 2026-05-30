using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    [SerializeField] public LayerMask tileLayerMask;

    Transform parentToReturnTo = null;

    public void OnDrag(PointerEventData eventData) {
        Debug.Log($"onDrag");
        this.transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        Debug.Log($"onBeginDrag");
        parentToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);
        // this.transform.rotation = Quaternion.Euler(new Vector3(60, 0, 0));
        this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log($"onEndDrag");
        Tile tile = GetTileUnderPointer(eventData.position);

        if (tile != null) {
            Vector2 coords = tile.coords; 
            Debug.Log($"Tile found, {coords.x}, {coords.y}");

            CardDisplay cardDisplay = this.GetComponent<CardDisplay>();
            if (cardDisplay != null) {
                Card card = cardDisplay.card;
                Debug.Log($"Card found, {card.cardName}");

                GameManager gm = GameManager.Instance;
                if (gm.CurrentPlayerNumber == 1) {
                    if (gm.p1Mana >= card.manaCost && tile.coords.y <= 2) {
                        gm.p1Mana -= card.manaCost;
                        tile.AddUnit(card);
                        Destroy(gameObject);
                    }
                }
            }
        }

        this.transform.SetParent(parentToReturnTo);
        // this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        this.transform.localScale = new Vector3(1, 1, 1);
    }

    private Tile GetTileUnderPointer(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayerMask))
        {
            return hit.collider.GetComponent<Tile>();
        }
        return null;
    }
}
