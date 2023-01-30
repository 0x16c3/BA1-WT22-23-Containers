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
    float _timePassed = 0, _timePassedAfterFire;
    [SerializeField]
    float _changingColorTime = 10f;


    //public Color MaxlMatColor;
    //Color _minMatColor, _tempColor,
    Color _initMatColor;

    public Color _strongDamage, _middleDamage, _lightDamage;

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

        //_initMatColor = _tempColor = _minMatColor = transform.Find("Cube").GetComponent<MeshRenderer>().material.color;
        _initMatColor = transform.Find("Cube").GetComponent<MeshRenderer>().material.color;

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

        #region Damage Indicator 
        /*
        if (Health < _maxHealth && !_stopDamageIndication)
        {
            _timePassedAfterFire += Time.deltaTime;
            Debug.Log("Wtf1");
            if (_timePassedAfterFire > FireTickInterval)
            {
                _tempColor.r = Mathf.Lerp(_minMatColor.r, MaxlMatColor.r, _timePassed);
                _tempColor.g = Mathf.Lerp(_minMatColor.g, MaxlMatColor.g, _timePassed);
                _tempColor.b = Mathf.Lerp(_minMatColor.b, MaxlMatColor.b, _timePassed);
                transform.Find("Cube").GetComponent<MeshRenderer>().material.color = _tempColor;
                Debug.Log("Wtf2");
                if (_timePassed > _changingColorTime)
                {
                    highlightIteration++;
                    _tempColor = MaxlMatColor;
                    MaxlMatColor = _minMatColor;
                    _minMatColor = _tempColor;
                    _timePassed = 0f;

                    if (highlightIteration > 2)
                        _stopDamageIndication = true;
                }
            }
        }
        else if (Health == _maxHealth || _stopDamageIndication)
        {
            _timePassed = 0;
            transform.Find("Cube").GetComponent<MeshRenderer>().material.color = _initMatColor;
        }
        */
        #endregion

        if (Health < _maxHealth && Health >= 8)
        {
            _timePassed += Time.deltaTime;
            if (_timePassed >= _changingColorTime)
            {
                transform.Find("Cube").GetComponent<MeshRenderer>().material.color = GetIdicateColor(0);
                _timePassed = 0;
            }
        }
        else if (Health < 8 && Health >= 4)
        {
            _timePassed += Time.deltaTime;
            if (_timePassed >= _changingColorTime)
            {
                transform.Find("Cube").GetComponent<MeshRenderer>().material.color = GetIdicateColor(1);
                _timePassed = 0;
            }
        }
        else if (Health < 4)
        {
            _timePassed += Time.deltaTime;
            if (_timePassed >= _changingColorTime)
            {
                transform.Find("Cube").GetComponent<MeshRenderer>().material.color = GetIdicateColor(2);
                _timePassed = 0;
            }
        }
        else
            transform.Find("Cube").GetComponent<MeshRenderer>().material.color = _initMatColor;
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

    Color GetIdicateColor(int _damagePower)
    {
        switch (_damagePower)
        {
            case 0:
                return _lightDamage;
            case 1:
                return _middleDamage;
            case 2:
                return _strongDamage;

            default:
                return _initMatColor;
        }
    }
}
