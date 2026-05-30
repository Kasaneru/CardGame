using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
    public static GridManager Instance;

    public int width, height;

    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private Transform _cam;

    private Dictionary<Vector2, Tile> _tiles;

    void Awake() {
        Instance = this;
    }

    // void Start() {
    //     GenerateGrid();
    // }

    public void GenerateGrid() {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, 0, z), Quaternion.identity);
                spawnedTile.coords = new Vector2(x, z);
                spawnedTile.name = $"Tile {x} {z}";

                Debug.Log($"Spawned {x} {z}");

                var isOffset = (x % 2 == 0 && z % 2 != 0) || (x % 2 != 0 && z % 2 == 0);
                spawnedTile.Init(isOffset);

                _tiles[new Vector2(x, z)] = spawnedTile;
            }
        }

        // _cam.transform.position = new Vector3((float)_width/2 -0.5f, (float)_height / 2 - 0.5f,-10);
        _cam.transform.position = new Vector3(2, 9, 2);
        _cam.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
    }

    public Tile GetTileAtPosition(Vector2 pos) {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }

    public IEnumerable<Tile> GetAllTiles() => _tiles.Values;
}
