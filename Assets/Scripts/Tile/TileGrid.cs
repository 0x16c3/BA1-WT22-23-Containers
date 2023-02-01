using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TileGrid : MonoBehaviour
{
    public Tilemap Tilemap;

    public Vector3 cellSize => Tilemap.cellSize;
    public Vector3 CellToWorld(Vector2Int gridPosition) => CellToWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));
    public Vector3 CellToWorld(Vector3Int gridPosition) => Tilemap.CellToWorld(gridPosition);

    public bool Initialized => _isInitialized;

    bool _isInitialized = false;

    public BoundsInt cellBounds
    {
        get
        {
            if (!_isInitialized)
                throw new System.Exception("TilemapGeneric: cellBounds: TilemapGeneric is not initialized");

            if (Tilemap == null)
                throw new System.Exception("TilemapGeneric: cellBounds: Tilemap is null");

            // add position to min max and set position to 0
            var bounds = Tilemap.cellBounds;
            return bounds;
        }
    }

    public void Awake()
    {
        Tilemap = GetComponent<Tilemap>();
        if (Tilemap == null)
            Debug.LogError("TilemapGeneric: OnEnable: Tilemap is null");

        Tilemap.ClearAllTiles();

        // We need to index all the tiles in the tilemap in order to create the tiles

        // Loop through every child ContainerGridCell
        var containerGridCells = GetComponentsInChildren<TileGridCell>();
        foreach (var containerGridCell in containerGridCells)
        {
            var tile = GetTile(containerGridCell.transform.position, false);
        }

        _isInitialized = true;
    }

    public static explicit operator TileGrid(Tilemap v)
    {
        if (v == null)
            return null;

        // Get the tilemap generic component
        var tilemapGeneric = v.GetComponent<TileGrid>();
        if (tilemapGeneric == null)
            tilemapGeneric = v.gameObject.AddComponent<TileGrid>();

        return tilemapGeneric;
    }

    public Vector2 CellSize
    {
        get
        {
            if (Tilemap == null)
            {
                Debug.LogError("TilemapGeneric: CellSize: Tilemap is null");
                return Vector2.zero;
            }

            return new Vector2(Tilemap.cellSize.x, Tilemap.cellSize.y);
        }
    }

    public TileGeneric GetTile(Vector3 worldPosition, bool checkBounds = true)
    {
        worldPosition.y = 0;

        var cellPos = Tilemap.WorldToCell(worldPosition);
        if (checkBounds && !cellBounds.Contains(cellPos))
            return null;

        return GetTile(new Vector2Int(cellPos.x, cellPos.y), checkBounds);
    }

    public TileGeneric GetTile(Vector2Int gridPosition, bool checkBounds = true)
    {
        // Throw exception if gridPosition is out of bounds (check manually)
        if (checkBounds && (gridPosition.x < cellBounds.xMin || gridPosition.x > cellBounds.xMax ||
            gridPosition.y < cellBounds.yMin || gridPosition.y > cellBounds.yMax))
            throw new System.ArgumentOutOfRangeException("TilemapGeneric: GetTile: gridPosition is out of bounds");

        var cellPos = new Vector3Int(gridPosition.x, gridPosition.y, 0);
        var tile = Tilemap.GetTile<Tile>(cellPos);

        // If tile is null, create a new tile
        if (tile == null)
        {
            var newTile = ScriptableObject.CreateInstance<Tile>();

            newTile.sprite = null;
            newTile.color = Color.clear;
            newTile.colliderType = Tile.ColliderType.None;

            Tilemap.SetTile(cellPos, newTile);
            tile = newTile;
        }

        return new TileGeneric(tile, Tilemap, gridPosition);
    }

    public static TileGrid FindTileGrid()
    {
        Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();

        if (tilemaps.Length != 1)
        {
            Debug.LogError("TilemapGeneric: FindTilemap: There should only be one tilemap in the scene");
            return null;
        }

        return (TileGrid)tilemaps[0];
    }

    public TileGeneric RandomTile()
    {
        var randomX = Random.Range(cellBounds.xMin, cellBounds.xMax);
        var randomY = Random.Range(cellBounds.yMin, cellBounds.yMax);

        // Get all the tiles that are walkable
        var walkableTiles = new List<TileGeneric>();
        for (int x = cellBounds.xMin + 2; x < cellBounds.xMax; x++)
        {
            for (int y = 0; y < cellBounds.yMax; y++)
            {
                var tile = GetTile(new Vector2Int(x, y));
                if (tile.Walkable)
                    walkableTiles.Add(tile);
            }
        }

        if (walkableTiles.Count == 0)
            return null;

        return GetTile(new Vector2Int(randomX, randomY));
    }

    void OnDrawGizmos()
    {
        if (Tilemap == null || !Initialized || !Tilemap.gameObject.activeInHierarchy)
            return;

        Gizmos.color = Color.red;

        // Draw grid
        var bounds = cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var cell = GetTile(new Vector2Int(x, y));

                if (cell == null)
                    continue;

                Gizmos.DrawWireCube(cell.WorldCenter, new Vector3(Tilemap.cellSize.x, 0.1f, Tilemap.cellSize.y));
            }
        }
    }
}