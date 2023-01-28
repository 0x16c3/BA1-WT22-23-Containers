using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PathTile : TileGeneric
{
    TileGeneric _tile;
    TileGrid _tileGrid;

    public PathTile Connection; // Previous tile in path
    public float G; // Cost from start to current tile
    public float H; // Estimated cost from current tile to end
    public float F => G + H; // Total cost

    public bool Walked = false;

    GameObject _gameObject;

    public PathTile(TileGeneric tile, TileGrid tilemap, Vector2Int pos, GameObject ignore = null) : base(tile.Tile, tilemap.Tilemap, pos)
    {
        _tile = tile;
        _tileGrid = tilemap;
        GridPosition = pos;
        _gameObject = ignore;
    }

    public static PathTile FromTile(TileGeneric tile, GameObject ignore = null)
    {
        return new PathTile(tile, tile.TileGrid, tile.GridPosition, ignore);
    }

    public static bool operator ==(PathTile a, PathTile b)
    {
        if (ReferenceEquals(a, b))
            return true;

        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            return false;

        return a.GridPosition == b.GridPosition;
    }

    public static bool operator !=(PathTile a, PathTile b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        PathTile tile = obj as PathTile;
        if ((System.Object)tile == null)
            return false;

        return GridPosition == tile.GridPosition;
    }

    public override int GetHashCode()
    {
        return GridPosition.GetHashCode();
    }

    public List<PathTile> Neighbors
    {
        get
        {
            var neighbors = _tile.GetNeighbors();
            var tiles = new List<PathTile>();

            for (int i = 0; i < neighbors.Count; i++)
            {
                var pathTile = new PathTile(neighbors[i], _tileGrid, neighbors[i].GridPosition, _gameObject);
                if (!pathTile.Walkable)
                    continue;

                tiles.Add(pathTile);
            }

            return tiles;
        }
    }

    public new bool Walkable
    {
        get
        {
            List<GameObject> objects = GetObjects();

            for (int i = 0; i < objects.Count; i++)
            {
                // Ignore if ground layer
                if (objects[i].layer == 3)
                    continue;

                var gridCell = objects[i].GetComponent<TileGridCell>();
                if (gridCell != null && gridCell.Broken)
                    return false;

                var damageable = objects[i].GetComponent<TileDamageable>();
                if (damageable != null && (damageable.OnFire || damageable.Health <= 0))
                    return false;

                // Ignore if current tile is the object
                if (objects[i] == _gameObject)
                    continue;

                // Ignore if current tile is the object
                if (objects[i].transform.position == WorldCenter)
                    continue;

                var collider = objects[i].GetComponent<Collider>();
                if (collider != null && !collider.isTrigger)
                    return false;
            }

            return true;
        }
    }

    public void OnDrawGizmosAnchor(bool start)
    {
        var pos = WorldCenter;
        var pos2 = pos + new Vector3(0, 3, 0);

        if (start)
            Gizmos.color = Color.blue;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawLine(pos, pos2);
    }

    public void OnDrawGizmos(bool searched, bool selected, bool showLabels = true)
    {

        var pos = WorldCenter;
        var size = new Vector3(TileGrid.cellSize.x, 0.1f, TileGrid.cellSize.y);

        if (searched)
            Gizmos.color = Color.red;
        else if (selected)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(pos, size);

        if (!Walkable)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(WorldCenter, TileGrid.cellSize.x / 4);
        }

        if (Connection != null && selected)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pos, Connection.WorldCenter);
        }

        if (!showLabels)
            return;

#if UNITY_EDITOR
        var label = $"G:{G.ToString("0.00")}\nH:{H.ToString("0.00")}\nF:{F.ToString("0.00")}";
        Handles.Label(pos, label);
#endif
    }
}