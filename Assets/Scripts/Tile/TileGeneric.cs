using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileGeneric
{
    public Tile Tile;
    public TileGrid TileGrid;

    public Vector2Int GridPosition;

    public TileGeneric(Tile tile, Tilemap tilemap, Vector2Int gridPosition)
    {
        Tile = tile;
        TileGrid = (TileGrid)tilemap;
        GridPosition = gridPosition;
    }

    public bool Walkable
    {
        get
        {
            List<GameObject> objects = GetObjects();

            for (int i = 0; i < objects.Count; i++)
            {
                // Ignore if ground layer
                if (objects[i].layer == 3)
                    continue;

                // Ignore if clip brush
                if (objects[i].layer == 4)
                    continue;

                var gridCell = objects[i].GetComponent<TileGridCell>();
                if (gridCell != null && gridCell.Broken)
                    return false;

                var damageable = objects[i].GetComponent<TileDamageable>();
                if (damageable != null && (damageable.OnFire || damageable.Health <= 0))
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

    public Vector3 WorldCenter
    {
        get
        {
            var worldPos = TileGrid.CellToWorld(new Vector3Int(GridPosition.x, GridPosition.y, 0));
            var pos = new Vector3(worldPos.x + TileGrid.cellSize.x / 2, 0, worldPos.z + TileGrid.cellSize.y / 2);
            return pos;
        }
    }

    public List<GameObject> GetObjects()
    {
        // Get gameObject at position using spherecast
        Collider[] colliders = Physics.OverlapCapsule(WorldCenter, WorldCenter + new Vector3(0, 4, 0), TileGrid.cellSize.x / 4);

        var objects = new List<GameObject>();
        for (int i = 0; i < colliders.Length; i++)
        {
            // Ignore clip brushes
            if (colliders[i].gameObject.layer == LayerMask.NameToLayer("ClipBrush"))
                continue;

            objects.Add(colliders[i].gameObject);
        }

        return objects;
    }

    public GameObject HighestObject
    {
        get
        {
            List<GameObject> objects = GetObjects();

            if (objects.Count == 0) return null;

            // Get highest object
            GameObject highestObject = null;
            float highestY = -Mathf.Infinity;

            for (int i = 1; i < objects.Count; i++)
            {
                // Get object collider
                var collider = objects[i].GetComponent<Collider>();
                if (collider == null)
                    continue;

                // Get object bottom
                var bottom = collider.bounds.min.y;

                // Check if object is higher than current highest
                if (bottom > highestY)
                {
                    highestY = bottom;
                    highestObject = objects[i];
                }
            }

            return highestObject;
        }
    }

    public TileDamageable Damageable
    {
        // get TileGridCell object's damagable
        get
        {
            List<GameObject> objects = GetObjects();

            if (objects.Count == 0) return null;

            // Iterate through objects and return first object that matches type
            foreach (var objectInstance in objects)
            {
                var gridCell = objectInstance.GetComponent<TileGridCell>();
                if (gridCell == null)
                    continue;

                var damageable = gridCell.GetComponent<TileDamageable>();
                if (damageable != null)
                    return damageable;
            }

            return null;
        }
    }

    public T GetInstantiatedObject<T>(Vector2Int gridPosition) where T : Component
    {
        TileGeneric tile = TileGrid.GetTile(gridPosition);
        List<GameObject> objects = GetObjects();

        if (objects.Count == 0) return null;

        // Iterate through objects and return first object that matches type
        foreach (var objectInstance in objects)
        {
            T cell = objectInstance.GetComponent<T>();

            if (cell != null)
                return cell;
        }

        return null;
    }

    public T GetInstantiatedObject<T>() where T : Component
    {
        List<GameObject> objects = GetObjects();

        if (objects.Count == 0) return null;

        // Iterate through objects and return first object that matches type
        foreach (var objectInstance in objects)
        {
            T cell = objectInstance.GetComponent<T>();

            if (cell != null)
                return cell;
        }

        return null;
    }

    public List<TileGeneric> GetNeighbors(bool ignoreDiagonals = true)
    {
        var neighbors = new List<TileGeneric>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                // Skip diagonals
                if (ignoreDiagonals && x != 0 && y != 0)
                    continue;

                var neighborPos = new Vector2Int(GridPosition.x + x, GridPosition.y + y);

                // Check if neighbor pos is out of bounds
                if (TileGrid.cellBounds.Contains(new Vector3Int(neighborPos.x, neighborPos.y, 0)) == false)
                    continue;

                TileGeneric neighbor = null;

                try
                {
                    neighbor = TileGrid.GetTile(neighborPos);
                }
                catch (System.Exception)
                {
                    continue;
                }

                if (neighbor == null)
                    continue;

                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    public GameObject GetNext(Vector2Int position, Vector3Int direction)
    {
        var nextPos = position + new Vector2Int(direction.x, direction.y);
        TileGeneric tile = TileGrid.GetTile(nextPos);

        if (tile == null)
            return null;

        List<GameObject> objects = tile.GetObjects();

        if (objects.Count > 0)
            return objects[0];

        return null;
    }

    public GameObject GetNext(Vector2Int position, Vector3 direction)
    {
        var dir = new Vector3Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y), Mathf.RoundToInt(direction.z));
        return GetNext(position, dir);
    }

    public T GetNext<T>(Vector2Int position, Vector3Int direction) where T : Component
    {
        var nextPos = position + new Vector2Int(direction.x, direction.z);
        TileGeneric tile = TileGrid.GetTile(nextPos);

        if (tile == null)
            return null;

        return tile.GetInstantiatedObject<T>(nextPos);
    }

    public T GetNext<T>(Vector2Int position, Vector3 direction) where T : Component
    {
        var dir = new Vector3Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y), Mathf.RoundToInt(direction.z));
        return GetNext<T>(position, dir);
    }

    public Vector3 GetWorldPosition()
    {
        var cellPos = new Vector3Int(GridPosition.x, GridPosition.y, 0);
        return TileGrid.CellToWorld(cellPos) + Vector3.right * TileGrid.cellSize.x / 2 + Vector3.forward * TileGrid.cellSize.y / 2;
    }
}