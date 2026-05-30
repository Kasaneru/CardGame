using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotPlayer : MonoBehaviour {
    public static BotPlayer Instance;

    [Header("Настройки")]
    [SerializeField] private float actionDelay = 0.5f;
    [SerializeField] public int spawnHeight = 5;

    private List<Card> handCards = new List<Card>();

    private void Awake()
    {
        Instance = this;
    }

    public void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Player2Turn)
        {
            StartCoroutine(TakeTurn());
        }
    }

    public void AddCardToHand(Card card)
    {
        handCards.Add(card);
        Debug.Log($"Бот получил карту: {card.cardName} (мана: {card.manaCost})");
    }

    private IEnumerator TakeTurn()
    {
        yield return new WaitForSeconds(0.3f);

        int mana = GameManager.Instance.p2Mana;

        while (handCards.Count > 0 && mana > 0)
        {
            List<Card> playable = handCards.FindAll(c => c.manaCost <= mana);
            if (playable.Count == 0) break;

            Card cardToPlay = playable[Random.Range(0, playable.Count)];

            Tile targetTile = FindFreeTileForBot();
            if (targetTile == null)
            {
                Debug.Log("Бот: нет свободных клеток для призыва");
                break;
            }

            targetTile.AddUnit(cardToPlay);
            mana -= cardToPlay.manaCost;
            GameManager.Instance.p2Mana = mana;
            handCards.Remove(cardToPlay);

            Debug.Log($"Бот призвал {cardToPlay.cardName} на клетку {targetTile.coords}");
            yield return new WaitForSeconds(actionDelay);
        }

        List<Unit> myUnits = GetAllMyUnits();
        foreach (Unit unit in myUnits)
        {
            while (unit.moves > 0)
            {
                Unit nearestEnemy = FindNearestEnemy(unit);
                if (nearestEnemy == null)
                    break;

                int dist = ManhattanDistance(unit.coords, nearestEnemy.coords);

                if (dist == 1)
                {
                    Tile enemyTile = GridManager.Instance.GetTileAtPosition(nearestEnemy.coords);
                    if (enemyTile != null)
                    {
                        enemyTile.AttackUnit(unit);
                        yield return new WaitForSeconds(actionDelay);
                        break;
                    }
                }
                else
                {
                    Tile nextTile = FindNextStepTowards(unit, nearestEnemy);
                    if (nextTile != null)
                    {
                        nextTile.MoveUnitHere(unit);
                        yield return new WaitForSeconds(actionDelay);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.EndTurn();
    }

    private List<Unit> GetAllMyUnits()
    {
        List<Unit> units = new List<Unit>();
        foreach (var tile in GridManager.Instance.GetAllTiles())
        {
            if (tile.IsOccupied() && tile.unit.owner == 2)
                units.Add(tile.unit);
        }
        return units;
    }

    private Unit FindNearestEnemy(Unit myUnit)
    {
        Unit closest = null;
        int minDist = int.MaxValue;
        foreach (var tile in GridManager.Instance.GetAllTiles())
        {
            if (tile.IsOccupied() && tile.unit.owner == 1)
            {
                int dist = ManhattanDistance(myUnit.coords, tile.unit.coords);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = tile.unit;
                }
            }
        }
        return closest;
    }

    private int ManhattanDistance(Vector2 a, Vector2 b)
    {
        return (int)(Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y));
    }

    private Tile FindFreeTileForBot()
    {
        List<Tile> allFree = new List<Tile>();

        foreach (var tile in GridManager.Instance.GetAllTiles())
        {
            if (!tile.IsOccupied())
            {
                if (tile.coords.y >= spawnHeight)
                    allFree.Add(tile);
            }
        }

        if (allFree.Count > 0)
            return allFree[Random.Range(0, allFree.Count)];
        return null;
    }

    private Tile FindNextStepTowards(Unit unit, Unit target)
    {
        Vector2 current = unit.coords;
        Vector2 goal = target.coords;
        int dx = (int)Mathf.Abs(goal.x - current.x);
        int dy = (int)Mathf.Abs(goal.y - current.y);

        List<Vector2> candidates = new List<Vector2>();
        if (dx >= dy) {
            candidates.Add(new Vector2(current.x + Mathf.Sign(goal.x - current.x), current.y));
            candidates.Add(new Vector2(current.x, current.y + Mathf.Sign(goal.y - current.y)));
        } else {
            candidates.Add(new Vector2(current.x, current.y + Mathf.Sign(goal.y - current.y)));
            candidates.Add(new Vector2(current.x + Mathf.Sign(goal.x - current.x), current.y));
        }

        foreach (var pos in candidates) {
            Tile tile = GridManager.Instance.GetTileAtPosition(pos);
            if (tile != null && !tile.IsOccupied())
                return tile;
        }
        return null;
    }
}
