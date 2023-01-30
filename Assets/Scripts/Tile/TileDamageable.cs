using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;

public class TileDamageable : MonoBehaviour, IDamageable
{
    public int SetHealth = 12;
    public int Health
    {
        get => SetHealth;
        private set => SetHealth = value;
    }

    [Header("Fire Settings")]
    [MinMaxSlider(0f, 60f)]
    public Vector2 FireDuration = new Vector2(5f, 10f);
    public float FireTickInterval = 1f;
    public int FireDamagePerTick = 1;

    public GameObject FirePrefab;

    [HideInInspector]
    public bool OnFire
    {
        get => _onFire;
        private set => _onFire = value;
    }

    TileGrid _grid;
    TileGeneric _tile;
    GameObject _localFire;

    TileGridCell _gridCell;

    ScoreSystem _scoreSystem;

    bool _onFire = false;
    int _maxHealth = -1;
    public int MaxHealth => _maxHealth;

    bool _wasDead = false;

    float _lastFireTick = -1f;

    bool _initialized = false;

    void Initialize()
    {
        _grid = TileGrid.FindTileGrid();
        if (!_grid.Initialized)
            return;

        _tile = _grid.GetTile(transform.position);
        if (_tile == null)
        {
            Debug.LogError("TileDamageable: No tile found at position " + transform.position);
            return;
        }

        _gridCell = GetComponent<TileGridCell>();
        if (_gridCell == null)
        {
            Debug.LogError("TileDamageable: No ContainerGridCell found at position " + transform.position);
            return;
        }

        _scoreSystem = FindObjectOfType<ScoreSystem>();
        if (_scoreSystem == null)
        {
            Debug.LogError("TileDamageable: No ScoreSystem found in scene");
            return;
        }

        _localFire = Instantiate(FirePrefab, transform);
        _localFire.transform.position = _tile.WorldCenter;

        _localFire.SetActive(false);

        _maxHealth = Health;

        _initialized = true;
    }

    void OnDisable()
    {
        Destroy(_localFire);
        _localFire = null;
    }

    private void Update()
    {
        if (!_initialized)
        {
            Initialize();
            return;
        }

        if (Health == 0)
        {
            Break();
            return;
        }

        if (_wasDead && Health == _maxHealth)
        {
            Repair();
            return;
        }

        if (_onFire)
            FireDamage();
    }

    void FireDamage()
    {
        if (_lastFireTick + FireTickInterval >= Time.time)
            return;

        Damage(FireDamagePerTick);
        _lastFireTick = Time.time;
    }

    public void SetFire(bool onFire)
    {
        // If on the left 2 rows, return
        if (_tile.GridPosition.x < _tile.TileGrid.cellBounds.xMin + 2)
            return;

        _onFire = onFire;
        _localFire.SetActive(_onFire);
        _lastFireTick = Time.time;
    }

    public void Damage(int damage)
    {
        // If on the left 2 rows, return
        if (_tile.GridPosition.x < _tile.TileGrid.cellBounds.xMin + 2)
            return;

        Health = Mathf.Clamp(Health - damage, 0, _maxHealth);
    }
    public void Heal(int heal) => Health = Mathf.Clamp(Health + heal, 0, _maxHealth);

    void Break()
    {
        // If already broken, return
        if (_wasDead)
            return;

        _gridCell.Break();
        _wasDead = true;

        _scoreSystem.OnTileBreak();

        SetFire(false);
    }

    void Repair()
    {
        // If already repaired, return
        if (!_wasDead)
            return;

        _gridCell.Repair();
        _wasDead = false;

        _scoreSystem.OnTileRepair();

        SetFire(false);
    }
}
