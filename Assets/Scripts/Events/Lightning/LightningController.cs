using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;

public class LightningController : MonoBehaviour, IEvent
{
    [MinMaxSlider(0, 180)]
    public Vector2Int _duration = new Vector2Int(45, 90);
    public Vector2Int Duration
    {
        get => _duration;
        set => _duration = value;
    }

    [MinMaxSlider(0, 25)]
    public Vector2Int MaxStrikes = new Vector2Int(10, 15);
    public GameObject LightningPrefab;

    [Header("Event Info")]
    [ReadOnly]
    public int RemainingStrikes = 0;
    [ReadOnly]
    public float RemainingTime = 0f;

    [HideInInspector]
    EventData _data;
    public EventData Data
    {
        get => _data;
        set => _data = value;
    }

    List<float> _strikeTimes = new List<float>();

    TileGrid _tileGrid;

    bool _initialized = false;

    void Initialize()
    {
        // We cannot access _data in OnEnable, so we do it here
        _tileGrid = TileGrid.FindTileGrid();

        for (int i = 0; i < Random.Range(MaxStrikes.x, MaxStrikes.y); i++)
        {
            _strikeTimes.Add(Random.Range(0f, _data.Duration));
        }

        _initialized = true;
    }

    void Update()
    {
        if (!_initialized && _data.Duration > 0)
            Initialize();

        if (!_initialized)
            return;

        var copy = new List<float>(_strikeTimes);
        foreach (var strikeTime in copy)
        {
            if (strikeTime + _data.StartsAt <= Time.time)
            {
                var tile = RandomTile();

                // Spawn lightning as child
                var lightning = Instantiate(LightningPrefab, tile.WorldCenter, Quaternion.identity, transform);

                // Remove strike time
                _strikeTimes.Remove(strikeTime);
            }
        }

        RemainingStrikes = _strikeTimes.Count;
        RemainingTime = _data.EndsAt - Time.time;

        if (_strikeTimes.Count == 0)
            Destroy(gameObject);
    }

    TileGeneric RandomTile()
    {
        var gridBounds = _tileGrid.cellBounds;

        var x = Random.Range(gridBounds.xMin + 2, gridBounds.xMax - 1);
        var y = Random.Range(gridBounds.yMin + 1, gridBounds.yMax - 1);

        // If tile is not walkable, try again
        var tile = _tileGrid.GetTile(new Vector2Int(x, y));

        if (tile == null || !tile.Walkable)
            return RandomTile();

        // Find damageable object
        var objects = tile.GetObjects();
        TileDamageable damageable = null;

        foreach (var obj in objects)
        {
            damageable = obj.GetComponent<TileDamageable>();
            if (damageable != null)
                break;
        }

        // If no damageable object, try again
        if (damageable == null || damageable.OnFire)
            return RandomTile();

        return tile;
    }
}
