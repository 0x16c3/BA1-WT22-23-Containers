using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class PathFinder
{
    public TileGrid Tilemap;
    public PathTile StartTile { get; private set; }
    public PathTile EndTile { get; private set; }
    public GameObject GameObject;

    List<PathTile> ToSearch;
    List<PathTile> Searched;

    public List<PathTile> PathProgress { get; private set; }
    public List<PathTile> Path { get; private set; }

    public delegate void FunctionTrigger();
    public event FunctionTrigger OnPathReset;
    public event FunctionTrigger OnPathFound;

    public PathFinder(TileGrid tilemap, GameObject gameObject)
    {
        Tilemap = tilemap;
        GameObject = gameObject;
    }

    public PathFinder(Tilemap tilemap, GameObject gameObject)
    {
        Tilemap = (TileGrid)tilemap;
        GameObject = gameObject;
    }

    public void SetStart(Vector2Int start)
    {
        if (StartTile != null && StartTile.GridPosition != start)
        {
            InitPath();
        }

        StartTile = PathTile.FromTile(Tilemap.GetTile(start), GameObject);
    }

    public void SetTarget(Vector2Int end)
    {
        EndTile = PathTile.FromTile(Tilemap.GetTile(end), GameObject);
    }

    public void OnDrawGizmos()
    {
        if (ToSearch != null)
        {
            ToSearch.ForEach(t => t.OnDrawGizmos(false, Path != null && Path.Contains(t) || PathProgress != null && PathProgress.Contains(t)));
        }

        if (Searched != null)
        {
            Searched.ForEach(t => t.OnDrawGizmos(true, Path != null && Path.Contains(t) || PathProgress != null && PathProgress.Contains(t)));
        }

        // Render start and end tiles
        if (StartTile != null)
        {
            StartTile.OnDrawGizmosAnchor(true);
        }

        if (EndTile != null)
        {
            EndTile.OnDrawGizmosAnchor(false);
        }
    }

    public void InitPath()
    {
        ToSearch = new List<PathTile>() { StartTile };
        Searched = new List<PathTile>();
        Path = null;

        OnPathReset?.Invoke();
    }

    public PathTile GetNextInPath(Vector2Int gridPos)
    {
        if (PathProgress == null)
            return null;

        // Get the tile at the given position, compare grid positions without using .Contains
        var tile = PathProgress.FirstOrDefault(t => t.GridPosition == gridPos);
        if (tile == null)
            return null;

        // If last tile in path, return
        if (tile == EndTile)
            return null;

        // Get next tile in path
        var nextTile = PathProgress.FirstOrDefault(t => t.Connection == tile);
        return nextTile;
    }

    PathTile GetLastObstructedTile(PathTile tile)
    {
        if (PathProgress == null)
            return null;

        PathTile lastObstructedTile = null;
        for (int i = 0; i < PathProgress.Count; i++)
        {
            if (!PathProgress[i].Walkable)
            {
                lastObstructedTile = PathProgress[i];
                break;
            }
        }

        if (lastObstructedTile == null)
            return null;

        if (lastObstructedTile.GridPosition == StartTile.GridPosition)
            return null;

        return lastObstructedTile;
    }

    public bool FoundPath()
    {
        return Path != null;
    }

    public void FindPath()
    {
        if (!ToSearch.Any() || EndTile == null)
            return;

        var current = ToSearch[0];

        if (GetLastObstructedTile(current) != null)
        {
            InitPath();
            PathProgress = null;
            return;
        }

        // If the end tile is obstructed, we can't find a path
        if (!EndTile.Walkable)
            return;

        if (FoundPath())
            return;

        // Select the tile with the lowest F cost
        foreach (var tile in ToSearch)
        {
            if (tile.F < current.F || tile.F == current.F && tile.H < current.H)
                current = tile;
        }

        // Remove the current tile from the toSearch list and add it to the searched list
        ToSearch.Remove(current);
        Searched.Add(current);

        // Get the path that we have found so far
        PathProgress = new List<PathTile>() { current };
        while (PathProgress.Last().Connection != null)
        {
            PathProgress.Add(PathProgress.Last().Connection);
        }

        // If the current tile is the end tile, we have found a path
        if (current.GridPosition == EndTile.GridPosition)
        {
            Path = PathProgress;
            Path.Reverse();

            OnPathFound?.Invoke();
            return;
        }

        // Check the neighbors of the current tile
        foreach (var neighbor in current.Neighbors.Where(t => t.Walkable && !Searched.Any(n => t.GridPosition == n.GridPosition)))
        {
            var willBeSearched = ToSearch.Any(t => t.GridPosition == neighbor.GridPosition);
            var costToNeighbor = current.G + Vector2Int.Distance(current.GridPosition, neighbor.GridPosition);

            if (!willBeSearched || costToNeighbor < neighbor.G)
            {
                // Update the neighbor's G cost
                neighbor.G = costToNeighbor;
                neighbor.Connection = current;

                // If not already in the toSearch list, add it
                if (!willBeSearched)
                {
                    neighbor.H = Vector2Int.Distance(neighbor.GridPosition, EndTile.GridPosition);
                    ToSearch.Add(neighbor);
                }
            }
        }
    }
}