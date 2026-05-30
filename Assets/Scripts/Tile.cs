using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] public Unit unit;

    public Vector2 coords;
    private Light _highlightLight;

    public void Init(bool isOffset) {
        _renderer.material.color = isOffset ? _offsetColor : _baseColor;
        if (_highlight != null)
            _highlightLight = _highlight.GetComponent<Light>();
    }

    public bool IsOccupied() {
        return unit != null;
    }

    public void AddUnit(Card _card) {
        if (IsOccupied())
        {
            Debug.LogWarning($"Клетка {coords} уже занята!");
            return;
        }

        if (_card.unitPrefab == null)
        {
            Debug.LogError($"У карты {_card.cardName} нет префаба юнита!");
            return;
        }
        Vector3 spawnPosition = transform.position + Vector3.up * 0.15f;

        Quaternion rot;
        if ((int)GameManager.Instance.GameState == 1) {
            rot = Quaternion.identity;
        } else if ((int)GameManager.Instance.GameState == 2) {
            rot = Quaternion.Euler(new Vector3(0, 180, 0));
        } else {
            Debug.Log("Error! Turn unknown!");
            return;
        }

        GameObject unitObj = Instantiate(_card.unitPrefab, spawnPosition, rot);

        if ((int)GameManager.Instance.GameState == 2) {
            Transform canv = unitObj.transform.Find("Canvas");
            if (canv != null)
                canv.localRotation = Quaternion.Euler(new Vector3(90, 0, 180));
        }

        Unit unitComponent = unitObj.GetComponent<Unit>();
        if (unitComponent == null)
        {
            Debug.LogError("Префаб юнита не содержит компонент Unit!");
            Destroy(unitObj);
            return;
        }

        unitComponent.coords = coords;
        unitComponent.owner = (int)GameManager.Instance.GameState;
        unitComponent.setUnit(_card);

        unit = unitComponent;
    }

    public void AddTower(Card _card, Vector2 _coords, int _owner) {
        if (IsOccupied())
        {
            Debug.LogWarning($"Клетка {coords} уже занята! Невозможно поставить башню!");
            return;
        }

        if (_card.unitPrefab == null)
        {
            Debug.LogError($"У карты {_card.cardName} нет префаба юнита!");
            return;
        }
        Vector3 spawnPosition = transform.position + Vector3.up * 0.15f;

        GameObject towerObj = Instantiate(_card.unitPrefab, spawnPosition, Quaternion.identity);

        Unit towerComponent = towerObj.GetComponent<Unit>();
        if (towerComponent == null)
        {
            Debug.LogError("Префаб башни не содержит компонент Unit!");
            Destroy(towerObj);
            return;
        }

        towerComponent.coords = _coords;
        towerComponent.owner = _owner;
        towerComponent.setUnit(_card);

        unit = towerComponent;
    }

    void OnMouseEnter() {
        _highlight.SetActive(true);
        Debug.Log($"Highlight activated");

        if (IsOccupied())
            UnitTooltip.Instance.Show(unit);
    }

    void OnMouseDown() {
        _highlight.SetActive(true);
        _highlightLight.color = new Color32(0, 255, 0, 255);
        Debug.Log($"Tile clicked, {coords.x}, {coords.y}");

        GameManager gm = GameManager.Instance;
        if (gm == null) 
            return;

        if (gm.SelectedUnit == null)
        {
            if (IsOccupied() && unit.owner == gm.CurrentPlayerNumber)
            {
                gm.SelectUnit(unit);
            }
            return;
        }

        Unit selected = gm.SelectedUnit;

        if (IsOccupied() && unit == selected)
        {
            gm.DeselectUnit();
            return;
        }

        if (IsOccupied() && unit.owner == gm.CurrentPlayerNumber)
        {
            gm.SelectUnit(unit);
            return;
        }

        if (IsOccupied() && unit.owner != gm.CurrentPlayerNumber)
        {
            Debug.Log($"Try attack");
            AttackUnit(selected);
            return;
        }

        if (!IsOccupied())
        {
            MoveUnitHere(selected);
            return;
        }

        gm.DeselectUnit();
    }

    public bool MoveUnitHere(Unit selectedUnit) {
        if (IsOccupied())
            return false;
        if (!IsAdjacent(coords, selectedUnit.coords))
            return false;

        if (selectedUnit.moves <= 0)
        {
            Debug.Log("Нет очков движения!");
            GameManager.Instance.DeselectUnit();
            return false;
        }

        StartCoroutine(MoveUnitCoroutine(selectedUnit));
        return true;
    }

    private IEnumerator MoveUnitCoroutine(Unit unitToMove) {
        unitToMove.moves--;

        Tile oldTile = GridManager.Instance.GetTileAtPosition(unitToMove.coords);
        if (oldTile != null) oldTile.unit = null;
        unit = unitToMove;

        Vector3 targetPos = transform.position + Vector3.up * 0.5f;

        yield return unitToMove.MoveToPosition(targetPos, 1.0f);

        unitToMove.coords = coords;

        if (unitToMove.moves <= 0)
            GameManager.Instance.DeselectUnit();
    }

    public bool AttackUnit(Unit attacker) {
        if (!IsOccupied() || unit.owner == attacker.owner)
            return false;
        if (!IsAdjacent(coords, attacker.coords))
            return false;

        if (attacker.moves <= 0)
            return false;

        StartCoroutine(AttackUnitCoroutine(attacker));
        return true;
    }

    public IEnumerator AttackUnitCoroutine(Unit attacker) {
        Unit target = unit;
        Vector3 targetPos = transform.position + Vector3.up * 0.5f;
        yield return attacker.PlayAttackAnimation(targetPos, 1.5f);
        target.health -= attacker.attack;
        target.UpdateCanvas();

        Debug.Log($"Юнит атаковал {target.name}, нанесено {attacker.attack} урона. HP цели: {target.health}");

        // Тратим оставшиеся очки движения
        attacker.moves = 0;

        if (target.health <= 0)
        {
            if (target.isTower == true) {
                GameManager.Instance.winner = attacker.owner;
                GameManager.Instance.ChangeState(GameState.EndGame);
            }
            target.PlayDeathAnimation();
            Destroy(target.gameObject, 1.5f);
            unit = null;
        }

        GameManager.Instance.DeselectUnit();
    }

    private bool IsAdjacent(Vector2 a, Vector2 b)
    {
        Vector2 diff = a - b;
        return Mathf.Abs(diff.x) + Mathf.Abs(diff.y) == 1;
    }

    void OnMouseExit()
    {
        _highlightLight.color = new Color32(255, 255, 255, 255);
        _highlight.SetActive(false);

        UnitTooltip.Instance.Hide();
    }
}
