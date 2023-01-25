using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;

public class LightningController : MonoBehaviour, IEvent
{
    [MinMaxSlider(0, 25)]
    public Vector2Int MaxStrikes = new Vector2Int(10, 15);
    public GameObject LightningPrefab;

    [Header("Event Info")]
    [ReadOnly]
    public int RemainingStrikes = 0;

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
        _tileGrid = TileGrid.FindTilemap();

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
                // todo: fix this

                // Choose random tile
                var gridBounds = _tileGrid.Tilemap.cellBounds;

                var x = Random.Range(gridBounds.xMin + 2, gridBounds.xMax - 1);
                var y = Random.Range(gridBounds.yMin + 1, gridBounds.yMax - 1);

                var worldPos = _tileGrid.GetTile(new Vector2Int(x, y)).WorldCenter;

                // Spawn lightning as child
                var lightning = Instantiate(LightningPrefab, worldPos, Quaternion.identity, transform);

                // Remove strike time
                _strikeTimes.Remove(strikeTime);
            }
        }

        RemainingStrikes = _strikeTimes.Count;

        if (_strikeTimes.Count == 0)
            Destroy(gameObject);
    }
}
