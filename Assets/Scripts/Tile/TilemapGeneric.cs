using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilemapGeneric : GridLayout
{
    public Tilemap Tilemap;

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

    public static explicit operator TilemapGeneric(Tilemap v)
    {
        var tilemapGeneric = new TilemapGeneric();
        tilemapGeneric.Tilemap = v;

        return tilemapGeneric;
    }

    public TileGeneric GetTile(Vector3 worldPosition)
    {
        worldPosition.y = 0;

        var cellPos = Tilemap.WorldToCell(worldPosition);
        return GetTile(new Vector2Int(cellPos.x, cellPos.y));
    }

    public TileGeneric GetTile(Vector2Int gridPosition)
    {
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

    public static TilemapGeneric FindTilemap()
    {
        Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();

        if (tilemaps.Length != 1)
        {
            Debug.LogError("TilemapGeneric: FindTilemap: There should only be one tilemap in the scene");
            return null;
        }

        return (TilemapGeneric)(tilemaps[0]);
    }
}