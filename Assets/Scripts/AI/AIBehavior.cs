using System.Collections.Generic;

using UnityEngine;

using GD.MinMaxSlider;

public enum AIBehaviorState
{
    Idle,
    Wander,
    Ability,
    STATE_MAX,
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
    public Vector2 IdleDuration = new Vector2(1f, 3f);
    [MinMaxSlider(0f, 120f)]
    public Vector2 WanderDuration = new Vector2(30f, 45f);
    [MinMaxSlider(0f, 120f)]
    public Vector2 AbilityDuration = new Vector2(5f, 10f);
    public float KnockOutTime = 15f;

    public AIBehaviorState ForceState = AIBehaviorState.STATE_MAX;

    public ICustomBehavior CustomBehavior = null;

    AIBehaviorState _state = AIBehaviorState.Idle;
    AIBehaviorState _prevState = AIBehaviorState.Wander;
    AIBehaviorState _nextState = AIBehaviorState.Wander;
    float _nextActionTime = -1f;
    public AIBehaviorState State { get => _state; private set => _state = value; }

    TileGrid _tileGrid;
    TileGeneric _tile;
    Rigidbody _rb;

    AIWander _wander;
    PlayerLocomotion _player;

    void OnEnable()
    {
        _tileGrid = TileGrid.FindTileGrid();
        _rb = GetComponent<Rigidbody>();
        _wander = GetComponent<AIWander>();

        CustomBehavior = GetComponent<ICustomBehavior>();

        // Find the player and get the locomotion component
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _player = player.GetComponent<PlayerLocomotion>();
        else
            Debug.LogWarning("Player not found. This container will not be able to get hit.");

        if (_rb == null)
            Debug.LogWarning("Rigidbody component not found on " + gameObject.name + ". This container will not be able to get hit.");
        if (_wander == null)
            Debug.LogWarning("AIWander component not found on " + gameObject.name + ". This container will not be able to move.");
        if (CustomBehavior == null)
            Debug.LogWarning("CustomBehavior component not found on " + gameObject.name + ". This container will not be able to use abilities.");

        _nextActionTime = Time.time + Random.Range(IdleDuration.x, IdleDuration.y);
        Health = MaxHealth;
    }

    void Update()
    {
        _tile = _tileGrid.GetTile(transform.position);

        SwitchState();
        ProcessRules();

        if (ForceState != AIBehaviorState.STATE_MAX)
            _state = ForceState;

        switch (_state)
        {
            case AIBehaviorState.Idle:
                Idle();
                break;
            case AIBehaviorState.Wander:
                Wander();
                break;
            case AIBehaviorState.Ability:
                Ability();
                break;
        }
    }

    public void ResetToDefault()
    {
        _state = AIBehaviorState.Idle;
        _prevState = AIBehaviorState.Wander;
        _nextState = AIBehaviorState.Wander;
        _nextActionTime = -1f;
        Health = MaxHealth;
    }

    public void ResetTimers(bool addRandomDelay = true)
    {
        _nextActionTime = Time.time + (addRandomDelay ? Random.Range(IdleDuration.x, IdleDuration.y) : 0f);
    }

    void SwitchState()
    {
        if (Time.time < _nextActionTime)
            return;

        List<AIBehaviorState> otherStates = new List<AIBehaviorState>((int)AIBehaviorState.STATE_MAX - 1);

        // Add all states except the current one
        for (int i = 0; i < (int)AIBehaviorState.STATE_MAX; i++)
        {
            if ((CustomBehavior == null || !CustomBehavior.Enabled) && (AIBehaviorState)i == AIBehaviorState.Ability)
                continue;

            if ((AIBehaviorState)i == _state)
                continue;

            if ((AIBehaviorState)i == AIBehaviorState.Wander && CustomBehavior != null && CustomBehavior.Enabled)
                otherStates.Add(AIBehaviorState.Ability);
            else
                otherStates.Add((AIBehaviorState)i);
        }

        // Select a random new state 
        _nextState = otherStates[Random.Range(0, otherStates.Count)];

        // Set the next action time
        switch (_nextState)
        {
            case AIBehaviorState.Idle:
                _nextActionTime = Time.time + Random.Range(IdleDuration.x, IdleDuration.y);
                break;
            case AIBehaviorState.Wander:
                _nextActionTime = Time.time + Random.Range(WanderDuration.x, WanderDuration.y);
                break;
            case AIBehaviorState.Ability:
                _nextActionTime = Time.time + Random.Range(AbilityDuration.x, AbilityDuration.y);
                break;
        }

        // Set the new state
        _prevState = _state;
        _state = _nextState;
    }

    void ProcessRules()
    {
        // If health is 0 and we're wandering, switch to Idle
        if (Health == 0 && _state == AIBehaviorState.Wander) // aka Knocked out
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
    }

    void Idle()
    {
        // Disable AIWander
        if (_wander != null && _wander.enabled)
            _wander.enabled = false;
    }

    void Wander()
    {
        // Enable AIWander
        if (_wander != null && !_wander.enabled)
        {
            _wander.enabled = true;
            if (Health < MaxHealth)
                SetHealth(MaxHealth);

            if (!_wander.RandomWander)
                _wander.RandomWander = true;
        }

    }

    void Ability()
    {
        if (CustomBehavior == null) return;

        CustomBehavior.Ability();
        if (CustomBehavior.WanderWhileUsingAbility)
            Wander();
        else
            Idle();
    }

    public void Damage(int damage)
    {
        Health = Mathf.Clamp(Health - damage, 0, MaxHealth);

        // Get container generic
        var container = GetComponent<ContainerGeneric>();
        if (container != null)
        {
            container.HitEffect.OnDamage();
        }
    }
    public void Heal(int heal) => Health = Mathf.Clamp(Health + heal, 0, MaxHealth);
    public void SetHealth(int health) => Health = Mathf.Clamp(health, 0, MaxHealth);
}
