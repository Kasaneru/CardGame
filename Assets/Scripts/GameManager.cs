using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;

    public int turn = 0;
    public int p1Mana, p2Mana;
    public int p1MaxMana, p2MaxMana;
    public int winner = 0;

    public Vector2 p1TowerCoords = new Vector2(2, 1);
    public Vector2 p2TowerCoords = new Vector2(2, 5); 
    public Card towerCard;
    public EndGamePanel endGamePanel;  

    public Unit SelectedUnit { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ChangeState(GameState.GenerateGrid);
    }

    public void ChangeState(GameState newState)
    {
        GameState = newState;
        switch (newState)
        {
            case GameState.GenerateGrid:
                GridManager.Instance.GenerateGrid();
                Tile tile1 = GridManager.Instance.GetTileAtPosition(p1TowerCoords);
                Tile tile2 = GridManager.Instance.GetTileAtPosition(p2TowerCoords);
                tile1.AddTower(towerCard, p1TowerCoords, 1);
                tile2.AddTower(towerCard, p2TowerCoords, 2);

                DeckManager.Instance.DrawCard(1);
                DeckManager.Instance.DrawCard(1);
                DeckManager.Instance.DrawCard(2);
                DeckManager.Instance.DrawCard(2);
                ChangeState(GameState.Player1Turn);
                break;
            case GameState.Player1Turn:
                turn += 1;
                if (p1MaxMana != 10) 
                    p1MaxMana += 1;
                p1Mana = p1MaxMana;
                ResetMovementForPlayer(1);
                DeckManager.Instance.DrawCard(1);
                break;
            case GameState.Player2Turn:
                BotPlayer bp = BotPlayer.Instance;
                turn += 1;
                if (p2MaxMana != 10)
                    p2MaxMana += 1;
                p2Mana = p2MaxMana;
                ResetMovementForPlayer(2);
                DeckManager.Instance.DrawCard(2);
                bp.OnGameStateChanged(GameState);
                break;
            case GameState.EndGame:
                Debug.Log($"Победа игрока {winner}");
                endGamePanel.Show(winner);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private void ResetMovementForPlayer(int player) {
        foreach (var tile in GridManager.Instance.GetAllTiles())
        {
            if (tile.IsOccupied() && tile.unit.owner == player)
            {
                tile.unit.moves = tile.unit.maxMovement;
            }
        }
    }

    public void SelectUnit(Unit unit)
    {
        if (unit == null)
        {
            DeselectUnit();
            return;
        }

        if (SelectedUnit != null)
            SelectedUnit.SetHighlight(false);

        SelectedUnit = unit;
        SelectedUnit.SetHighlight(true);
    }

    public void DeselectUnit()
    {
        if (SelectedUnit != null)
        {
            SelectedUnit.SetHighlight(false);
            SelectedUnit = null;
        }
    }

    public void EndTurn() {
        if (GameState == GameState.Player1Turn)
            ChangeState(GameState.Player2Turn);
        else if (GameState == GameState.Player2Turn)
            ChangeState(GameState.Player1Turn);
    }

    public int CurrentPlayerNumber
    {
        get
        {
            if (GameState == GameState.Player1Turn) return 1;
            if (GameState == GameState.Player2Turn) return 2;
            return 0;
        }
    }
}

public enum GameState
{
    GenerateGrid = 0,
    Player1Turn = 1,
    Player2Turn = 2,
    EndGame = 3
}
