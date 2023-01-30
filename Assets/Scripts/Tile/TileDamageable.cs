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

    bool _onFire = false;
    int _maxHealth = -1;
    public int MaxHealth => _maxHealth;

    bool _wasDead = false;

    float _lastFireTick = -1f;

    void OnEnable()
    {
        _grid = TileGrid.FindTileGrid();
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

        _localFire = Instantiate(FirePrefab, transform);
        _localFire.transform.position = _tile.WorldCenter;

        _localFire.SetActive(false);

        _maxHealth = Health;
    }

    void OnDisable()
    {
        Destroy(_localFire);
        _localFire = null;
    }

    private void Update()
    {
        if (Health == 0)
        {
            Kill();
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
        _onFire = onFire;
        _localFire.SetActive(_onFire);
        _lastFireTick = Time.time;
    }

    public void Damage(int damage) => Health = Mathf.Clamp(Health - damage, 0, _maxHealth);
    public void Heal(int heal) => Health = Mathf.Clamp(Health + heal, 0, _maxHealth);

    void Kill()
    {
        _gridCell.Break();
        _wasDead = true;

        SetFire(false);
    }

    void Repair()
    {
        _gridCell.Repair();
        _wasDead = false;

        SetFire(false);
    }
}
