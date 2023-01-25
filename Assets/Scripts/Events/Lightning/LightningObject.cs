using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightningObject : MonoBehaviour
{
    public int StrikeDamage = 1;
    public float TimeBetweenWarningAndLightning = 1;

    [Header("Size Selection Chances")]
    [Range(0f, 1f)]
    public float SmallWeight = 0.5f;
    [Range(0f, 1f)]
    public float MediumWeight = 0.3f;
    [Range(0f, 1f)]
    public float BigWeight = 0.2f;

    [Header("Prefabs")]
    public GameObject LightningVFX;
    public GameObject LightningWarningVFX;

    TileGrid _tileGrid;
    TileGeneric _centerTile;

    float _strikeTime = -1f;
    bool _struck = false;

    void OnEnable()
    {
        _tileGrid = TileGrid.FindTilemap();
        _centerTile = _tileGrid.GetTile(transform.position);

        // Instantiate lightning warning
        var warning = Instantiate(LightningWarningVFX, _centerTile.WorldCenter, Quaternion.identity, transform);

        _strikeTime = Time.time;
    }

    void Update()
    {
        if (_strikeTime + TimeBetweenWarningAndLightning <= Time.time && !_struck)
        {
            // Choose lightning size
            float random = Random.Range(0f, 1f);

            if (random <= SmallWeight)
                SmallLightning();
            else if (random <= SmallWeight + MediumWeight)
                MediumLightning();
            else
                BigLightning();

            _struck = true;
        }

        if (_strikeTime + TimeBetweenWarningAndLightning + 1f <= Time.time && _struck)
        {
            Destroy(gameObject);
        }
    }

    void SmallLightning()
    {
        StrikeTile(_centerTile.GridPosition);
    }

    void MediumLightning()
    {
        StrikeTile(_centerTile.GridPosition);

        var neighbors = _centerTile.GetNeighbors(false);
        neighbors = SanitizeNeighbors(neighbors);

        var randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];
        StrikeTile(randomNeighbor.GridPosition);
    }

    void BigLightning()
    {
        // Get all neighbors
        var neighbors = _centerTile.GetNeighbors(false);
        neighbors = SanitizeNeighbors(neighbors);

        // Strike 4 random neighbors
        for (int i = 0; i < 4; i++)
        {
            var randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];
            StrikeTile(randomNeighbor.GridPosition);

            neighbors.Remove(randomNeighbor);
        }
    }

    List<TileGeneric> SanitizeNeighbors(List<TileGeneric> neighbors)
    {
        var copy = new List<TileGeneric>(neighbors);

        var bounds = _tileGrid.Tilemap.cellBounds;
        foreach (var neighbor in copy)
        {
            if (neighbor.GridPosition.x <= bounds.xMin + 2 || neighbor.GridPosition.x >= bounds.xMax - 1)
                neighbors.Remove(neighbor);
        }

        return neighbors;
    }

    void StrikeTile(Vector2Int gridPos)
    {
        var tile = _tileGrid.GetTile(gridPos);
        if (tile == null)
            return;

        // Get all objects on tile
        var objects = tile.GetObjects();

        // Set tile on fire and damage the top object
        GameObject highestObject = null;
        float highestObjectY = float.MinValue;

        foreach (var obj in objects)
        {
            var tileDamageable = obj.GetComponent<TileDamageable>();
            if (tileDamageable != null)
                tileDamageable.SetFire(true);

            if (obj.transform.position.y > highestObjectY)
            {
                highestObjectY = obj.transform.position.y;
                highestObject = obj;
            }
        }

        if (highestObject != null)
        {
            var damageable = highestObject.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.Damage(StrikeDamage);
            else
                Debug.LogWarning("No damageable component found on object " + highestObject.name);
        }

        // Play lightning VFX
        var lightning = Instantiate(LightningVFX, tile.WorldCenter, Quaternion.identity, transform);
    }
}
