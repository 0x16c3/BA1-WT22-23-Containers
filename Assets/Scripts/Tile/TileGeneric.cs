using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileGeneric
{
    public Tile Tile;
    public TileGrid Tilemap;

    public Vector2Int GridPosition;

    public TileGeneric(Tile tile, Tilemap tilemap, Vector2Int gridPosition)
    {
        Tile = tile;
        Tilemap = (TileGrid)tilemap;
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

                // Ignore if current tile is the object
                if (objects[i].transform.position == WorldCenter)
                    continue;

                if (objects[i].GetComponent<Collider>() != null)
                    return false;
            }

            return true;
        }
    }

    public Vector3 WorldCenter
    {
        get
        {
            var worldPos = Tilemap.CellToWorld(new Vector3Int(GridPosition.x, GridPosition.y, 0));
            var pos = new Vector3(worldPos.x + Tilemap.cellSize.x / 2, 0, worldPos.z + Tilemap.cellSize.y / 2);
            return pos;
        }
    }

    public List<GameObject> GetObjects()
    {
        // Get gameObject at position using spherecast
        Collider[] colliders = Physics.OverlapCapsule(WorldCenter, WorldCenter + new Vector3(0, 1, 0), Tilemap.cellSize.x / 4);

        var objects = new List<GameObject>();
        for (int i = 0; i < colliders.Length; i++)
        {
            objects.Add(colliders[i].gameObject);
        }

        return objects;
    }

    public T GetInstantiatedObject<T>(Vector2Int gridPosition) where T : Component
    {
        // todo: make this a static method

        TileGeneric tile = Tilemap.GetTile(gridPosition);
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

    public GameObject GetNext(Vector2Int position, Vector3Int direction)
    {
        var nextPos = position + new Vector2Int(direction.x, direction.y);
        TileGeneric tile = Tilemap.GetTile(nextPos);

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
        TileGeneric tile = Tilemap.GetTile(nextPos);

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
        return Tilemap.CellToWorld(cellPos) + Vector3.right * Tilemap.cellSize.x / 2 + Vector3.forward * Tilemap.cellSize.y / 2;
    }
}