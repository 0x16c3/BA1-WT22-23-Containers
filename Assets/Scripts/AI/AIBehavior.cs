using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GD.MinMaxSlider;

public enum AIBehaviorState
{
    Idle,
    Wander,
    Ability,
}

public class AIBehavior : MonoBehaviour, IDamageable
{
    int _health;
    public int Health
    {
        get => _health;
        private set => _health = value;
    }

    public int MaxHealth = 6;

    [MinMaxSlider(0f, 120f)]
    public Vector2 IdleToWander = new Vector2(1f, 3f);
    [MinMaxSlider(0f, 120f)]
    public Vector2 WanderToIdle = new Vector2(30f, 45f);
    public float KnockOutTime = 15f;

    AIBehaviorState _state = AIBehaviorState.Idle;
    AIBehaviorState _prevState = AIBehaviorState.Wander;
    public AIBehaviorState State { get => _state; private set => _state = value; }

    TileGrid _tileGrid;
    TileGeneric _tile;
    AIWander _wander;

    float _nextActionTime = -1f;

    AIBehaviorState _oppState => _state == AIBehaviorState.Idle ? AIBehaviorState.Wander : AIBehaviorState.Idle;

    void OnEnable()
    {
        _tileGrid = TileGrid.FindTileGrid();
        _wander = GetComponent<AIWander>();

        if (_wander == null)
            Debug.LogWarning("AIWander component not found on " + gameObject.name + ". This container will not be able to move.");

        _nextActionTime = Time.time + Random.Range(IdleToWander.x, IdleToWander.y);
        Health = MaxHealth;
    }

    void Update()
    {
        _tile = _tileGrid.GetTile(transform.position);

        // Switch state between Idle and Wander if time is up and we haven't already changed state
        if (Time.time > _nextActionTime && _oppState == _prevState)
        {
            var range = _state == AIBehaviorState.Idle ? WanderToIdle : IdleToWander;
            _nextActionTime = Time.time + Random.Range(range.x, range.y);

            // If another container is placed on top of us, switch to Idle
            if (_tile.HighestObject != null && _tile.HighestObject != gameObject)
            {
                _state = AIBehaviorState.Idle;
                _prevState = _oppState;
            }
            else
            {
                _prevState = _state;
                _state = _oppState;
            }
        }

        // If health is 0 and we're wandering, switch to Idle
        if (Health == 0 && _state == AIBehaviorState.Wander)
        {
            _prevState = _state;
            _state = AIBehaviorState.Idle;
        }

        // If health is 0 and we're idle, heal back to MaxHealth over time
        else if (Health == 0 && _state == AIBehaviorState.Idle)
        {
            float healAmount = MaxHealth / KnockOutTime * Time.deltaTime;
            if (healAmount == MaxHealth)
                SetHealth(MaxHealth);
        }

        switch (_state)
        {
            case AIBehaviorState.Idle:
                {
                    // Disable AIWander
                    if (_wander != null && _wander.enabled)
                        _wander.enabled = false;

                    break;
                }
            case AIBehaviorState.Wander:
                {
                    // Enable AIWander
                    if (_wander != null && !_wander.enabled)
                        _wander.enabled = true;

                    break;
                }
            case AIBehaviorState.Ability:
                break;
        }
    }

    public void Damage(int damage) => Health = Mathf.Clamp(Health - damage, 0, MaxHealth);
    public void Heal(int heal) => Health = Mathf.Clamp(Health + heal, 0, MaxHealth);
    public void SetHealth(int health) => Health = Mathf.Clamp(health, 0, MaxHealth);
}
