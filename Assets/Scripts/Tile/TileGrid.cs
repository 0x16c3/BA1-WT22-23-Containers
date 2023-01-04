using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TileGrid : MonoBehaviour
{
    public Tilemap Tilemap;

    public Vector3 cellSize => Tilemap.cellSize;
    public Vector3 CellToWorld(Vector2Int gridPosition) => CellToWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));
    public Vector3 CellToWorld(Vector3Int gridPosition) => Tilemap.CellToWorld(gridPosition);

    public void OnEnable()
    {
        Tilemap = GetComponent<Tilemap>();
        if (Tilemap == null)
            Debug.LogError("TilemapGeneric: OnEnable: Tilemap is null");
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

    public static TileGrid FindTilemap()
    {
        Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();

        if (tilemaps.Length != 1)
        {
            Debug.LogError("TilemapGeneric: FindTilemap: There should only be one tilemap in the scene");
            return null;
        }

        return (TileGrid)tilemaps[0];
    }
}