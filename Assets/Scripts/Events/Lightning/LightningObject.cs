using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightningStrike
{
    public Vector2Int GridPos;
    TileGrid _tileGrid;

    int _damage;
    float _timeBetweenWarningAndLightning;

    GameObject _lightningVFX;
    GameObject _lightningWarningVFX;

    Transform _transform;

    public float StrikeTime;
    public bool PlayedWarning = false;
    public bool Struck = false;
    public bool PlayedLightning = false;

    AudioSource _audioSource;
    bool _isSecondary = false;

    public LightningStrike(
        TileGrid tileGrid,
        Vector2Int gridPos,
        Transform transform,
        GameObject lightningVFX,
        GameObject lightningWarningVFX,
        AudioSource audioSource,
        int strikeDamage = 1,
        float timeBetweenWarningAndLightning = 1f,
        bool isSecondary = false)
    {
        _tileGrid = tileGrid;
        _transform = transform;
        _lightningVFX = lightningVFX;
        _lightningWarningVFX = lightningWarningVFX;
        _audioSource = audioSource;
        _damage = strikeDamage;
        _timeBetweenWarningAndLightning = timeBetweenWarningAndLightning;
        _isSecondary = isSecondary;

        StrikeTime = Time.time + (isSecondary ? Random.Range(0f, 0.5f) : 0f);
        GridPos = gridPos;
    }

    public void OnUpdate()
    {
        if (Struck && StrikeTime + _timeBetweenWarningAndLightning + 5f < Time.time)
            PlayedLightning = true;

        if (Struck)
            return;

        if (Time.time >= StrikeTime && !PlayedWarning)
        {
            var tile = _tileGrid.GetTile(GridPos);
            if (tile == null)
            {
                Debug.LogError("LightningStrike: No tile found at position " + GridPos);
                return;
            }

            // Instantiate lightning warning
            var warning = GameObject.Instantiate(_lightningWarningVFX, tile.WorldCenter, Quaternion.identity, _transform);
            PlayedWarning = true;

            // Play lightning sound after _timeBetweenWarningAndLightning - buildup so it plays at the same time as the lightning
            if (!_isSecondary)
                _audioSource.PlayDelayed(_timeBetweenWarningAndLightning - 0.54f);
        }
        else if (Time.time >= StrikeTime + _timeBetweenWarningAndLightning)
            StrikeTile();
    }

    void StrikeTile()
    {
        var tile = _tileGrid.GetTile(GridPos);
        if (tile == null)
            return;

        // Get all objects on tile
        var objects = tile.GetObjects();

        // Set tile on fire and damage the top object
        TileDamageable damageableTile = null;
        GameObject highestObject = null;
        float highestObjectY = float.MinValue;

        foreach (var obj in objects)
        {
            // Skip if a clip brush
            if (obj.layer == LayerMask.NameToLayer("ClipBrush"))
                continue;

            var _damagableTile = obj.GetComponent<TileDamageable>();
            if (damageableTile == null && _damagableTile != null)
            {
                damageableTile = _damagableTile;
                continue;
            }

            if (damageableTile != null)
            {
                // If object is a child in the tile, skip it
                if (obj.transform.IsChildOf(damageableTile.transform))
                    continue;
            }

            if (obj.transform.position.y > highestObjectY)
            {
                highestObjectY = obj.transform.position.y;
                highestObject = obj;
            }
        }

        if (damageableTile != null && highestObject == null)
            damageableTile.SetFire(true);
        else if (highestObject != null)
        {
            var damageable = highestObject.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.Damage(_damage);
            else
                Debug.LogWarning("No damageable component found on object " + highestObject.name);
        }
        else if (!damageableTile && highestObject == null)
            Debug.LogWarning("No damageable component found on tile " + tile.GridPosition);

        // Instantiate lightning
        GameObject.Instantiate(_lightningVFX, tile.WorldCenter, Quaternion.identity, _transform);
        Struck = true;
    }
}

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

    bool _playedSFX = false;

    AudioSource _audioSource;

    float _strikeTime = -1f;
    bool _struck = false;

    List<LightningStrike> _strikes = new List<LightningStrike>();

    void OnEnable()
    {
        _tileGrid = TileGrid.FindTileGrid();
        _centerTile = _tileGrid.GetTile(transform.position);
        _strikeTime = Time.time;

        // Select random audio source
        _audioSource = GetComponentsInChildren<AudioSource>().OrderBy(a => Random.value).First();
        _audioSource.enabled = true;
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

        // Process each strike
        foreach (var strike in _strikes)
        {
            strike.OnUpdate();
        }

        // Remove strikes that have finished
        _strikes.RemoveAll(s => s.PlayedLightning);

        if (_struck && _strikes.Count == 0)
            Destroy(gameObject);
    }

    void AddStrike(Vector2Int gridPos, bool isSecondary = false)
    {
        // i do not even care anymore
        var strike = new LightningStrike(_tileGrid, gridPos, transform, LightningVFX, LightningWarningVFX, _audioSource, StrikeDamage, TimeBetweenWarningAndLightning, isSecondary);
        _strikes.Add(strike);
    }

    void SmallLightning()
    {
        AddStrike(_centerTile.GridPosition);
    }

    void MediumLightning()
    {
        AddStrike(_centerTile.GridPosition);

        var neighbors = _centerTile.GetNeighbors(false);
        neighbors = SanitizeNeighbors(neighbors);

        // If no neighbors, strike center tile
        if (neighbors.Count == 0)
            return;

        var randomNeighbor = neighbors[Random.Range(0, neighbors.Count - 1)];
        AddStrike(randomNeighbor.GridPosition, true);
    }

    void BigLightning()
    {
        AddStrike(_centerTile.GridPosition);

        // Get all neighbors
        var neighbors = _centerTile.GetNeighbors(false);
        neighbors = SanitizeNeighbors(neighbors);

        // If no neighbors, strike center tile
        if (neighbors.Count <= 0)
            return;

        // Strike 4 random neighbors
        for (int i = 0; i < 4; i++)
        {
            if (neighbors.Count == 0)
                break;

            var randomNeighbor = neighbors[Random.Range(0, neighbors.Count - 1)];
            AddStrike(randomNeighbor.GridPosition, true);

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

            // Find damageable object
            var objects = neighbor.GetObjects();
            TileDamageable damageable = null;

            foreach (var obj in objects)
            {
                damageable = obj.GetComponent<TileDamageable>();
                if (damageable != null)
                    break;
            }

            // If no damageable object, try again
            if (damageable == null || damageable.OnFire)
                neighbors.Remove(neighbor);
        }

        return neighbors;
    }
}
