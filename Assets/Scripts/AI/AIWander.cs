using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AIWander : MonoBehaviour
{
    [Header("Wander Settings")]
    public float ShakeTime = 3f;
    public float InactiveTime = 10.0f;

    [Header("Movement Settings")]
    [Range(0f, 25f)]
    public float MovementSpeed = 9f;
    [Range(0f, 10f)]
    public float RotationSpeed = 1f;
    [Range(0f, 50f)]
    public float RotationDecelerationMultiplier = 10f;
    [Range(0f, 1000f)]
    public float Acceleration = 200f;
    [Range(0f, 1000f)]
    public float MaxAccelerationForce = 150f;

    Rigidbody _rb;
    TileGrid _tileGrid;
    TileGeneric _tile;
    ContainerGeneric _container;

    PathFinder _pathFinder;
    List<PathTile> _paths = new List<PathTile>();

    Vector3 _lastRandPoint = Vector3.zero;
    PathTile _currentTarget;

    bool _firstUpdate = false;
    bool _reachedTarget = false;
    bool _reachedCurrentTarget => _currentTarget != null && Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), _currentTarget.WorldCenter) < 0.05f;

    bool _correctedRotation = false;
    bool _containerGrabbed => _container != null && _container.IsGrabbed;
    bool _containerWasGrabbed = false;
    bool _containerDropped = true;
    bool _grounded => !_containerGrabbed && Mathf.Abs(_rb.velocity.y) < 0.1f;

    float _inactiveTimer = 0f;
    bool _shake = false;
    bool _inactive = false;

    void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();

        _tileGrid = TileGrid.FindTileGrid();
        _tile = _tileGrid.GetTile(transform.position);

        _container = GetComponent<ContainerGeneric>();

        _pathFinder = new PathFinder(_tileGrid, this.gameObject);
        _pathFinder.OnPathReset += ResetPathData;
    }

    void OnDisable()
    {
        Halt();

        // Reset path data
        ResetPathData();
        _lastRandPoint = Vector3.zero;
        _paths = new List<PathTile>();
        _currentTarget = null;

        // Unsubscribe from events
        _pathFinder.ResetAllData();
        _pathFinder.OnPathReset -= ResetPathData;
    }

    void Wander()
    {
        TileGeneric randomTile = _tileGrid.RandomTile();

        // If not walkable, find a new one
        while (!randomTile.Walkable)
            randomTile = _tileGrid.RandomTile();

        _lastRandPoint = randomTile.WorldCenter;

        _pathFinder.SetStart(_tile.GridPosition);
        _pathFinder.SetTarget(randomTile.GridPosition);
        _pathFinder.InitPath();

        _reachedTarget = false;
        _firstUpdate = true;
        _inactive = false;
    }

    void ResetPathData()
    {
        _paths = new List<PathTile>();
        _currentTarget = null;
        _correctedRotation = false;
    }

    void Update()
    {
        if (!_tileGrid || _tile == null)
            return;

        UpdateContainerStates();

        if (_reachedTarget || _lastRandPoint == Vector3.zero || _inactive)
            Wander();

        _pathFinder.FindPath();
        _paths = _pathFinder.Path;
        _tile = _tileGrid.GetTile(transform.position);
    }

    void FixedUpdate()
    {
        InactiveTimer();
        if (_inactiveTimer > ShakeTime)
            Shake();

        if (_paths == null || _paths.Count == 0)
            return;

        if (_reachedTarget)
            return;

        if (!_grounded)
            return;

        var nextTile = _pathFinder.GetNextInPath(_tile.GridPosition);

        SwitchTarget();
        FaceDirection(nextTile);
        MoveTowards(_currentTarget);
    }

    void FaceDirection(PathTile target)
    {
        if (target == null)
            return;

        // Calculate direction towards target
        Vector3 direction = target.WorldCenter - transform.position;
        direction.y = 0;
        direction.Normalize();

        float angle = Vector3.Angle(transform.forward, direction);

        // This will let the AI make small adjustments to its rotation while moving towards the target
        if (!_firstUpdate && !_reachedCurrentTarget && Mathf.Abs(angle) > 25f)
        {
            _correctedRotation = false;
            return;
        }

        // Rotate towards target
        ApplyTorque(direction);

        // If direction is low enough, mark it as corrected
        if (angle < 0.05f)
            _correctedRotation = true;
    }

    void MoveTowards(PathTile target)
    {
        if (target == null)
            return;

        // Calculate direction towards target
        Vector3 direction = _currentTarget.WorldCenter - transform.position;
        direction.y = 0;
        direction.Normalize();

        // Apply force if rotation is correct
        ApplyForce(direction, MovementSpeed);
    }

    void SwitchTarget()
    {
        //If the current target is not null and the target has not been reached, return.
        if (_currentTarget != null && !_reachedCurrentTarget)
            return;

        //Get the next tile from the path finder.
        var nextTile = _pathFinder.GetNextInPath(_tile.GridPosition);

        //If the rotation was corrected, set the current target to the next tile and set the correct rotation to false.
        if (_correctedRotation)
        {
            // Mark the current target as reached
            if (_currentTarget != null)
                _currentTarget.Walked = true;

            _currentTarget = nextTile;
            _correctedRotation = false;

            //If this is the first update, set the first update to false.
            if (_firstUpdate)
                _firstUpdate = false;
        }

        //If the next tile is null, halt the game and set the reached target to true.
        if (nextTile == null)
        {
            Halt();
            _reachedTarget = true;
        }
    }

    void ApplyForce(Vector3 dirNormalized, float speed)
    {
        // Calculate goal velocity
        Vector3 goalVelocity = dirNormalized * speed;

        // Calculate acceleration
        Vector3 acceleration = goalVelocity - _rb.velocity;
        acceleration = Vector3.ClampMagnitude(acceleration, MaxAccelerationForce);

        // Apply acceleration
        _rb.AddForce(acceleration * Acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    void ApplyTorque(Vector3 dirNormalized)
    {
        // Make object look at direction using torque, forcemode velocitychange to make it instant
        Vector3 torque = Vector3.Cross(transform.forward, dirNormalized);

        // Apply torque
        _rb.AddTorque(torque * RotationSpeed, ForceMode.VelocityChange);

        // Apply inverse torque so it doesn't keep spinning
        _rb.AddTorque(-_rb.angularVelocity * RotationDecelerationMultiplier * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    void Halt()
    {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    void UpdateContainerStates()
    {
        if (_containerGrabbed)
            _containerWasGrabbed = true;

        if (_containerWasGrabbed && !_containerGrabbed)
        {
            _containerWasGrabbed = false;
            _containerDropped = true;
        }

        // If not falling & dropped
        if (_containerDropped && _grounded)
        {
            _containerDropped = false;
            _pathFinder.SetStart(_tile.GridPosition);
            _pathFinder.InitPath();
        }
    }

    void InactiveTimer()
    {
        // If velocity is low enough, start timer
        if (_rb.velocity.magnitude < 0.5f && _rb.angularVelocity.magnitude < 0.5f)
        {
            _inactiveTimer += Time.deltaTime;

            // If timer is high enough, set inactive
            if (_inactiveTimer > InactiveTime)
                _inactive = true;
        }
        else
        {
            _inactiveTimer = 0;
            _inactive = false;
            _shake = false;
        }
    }

    void Shake()
    {
        if (_shake)
            return;

        // Do some random shaking when inactive for a while to get the AI moving again 
        _rb.AddForce(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * 1f, ForceMode.Impulse);
        _rb.AddTorque(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * 1f, ForceMode.Impulse);
        _shake = true;
    }

    void OnDrawGizmos()
    {
        if (_tileGrid == null)
            return;

        if (_pathFinder != null)
            _pathFinder.OnDrawGizmos();

        if (_tile != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(_tile.GetWorldPosition(), 0.5f);
        }

        if (_currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_currentTarget.GetWorldPosition(), 0.5f);

#if UNITY_EDITOR
            var distanceWorld = Vector3.Distance(transform.position, _currentTarget.WorldCenter);
            Handles.Label(_currentTarget.WorldCenter, $"Distance: {distanceWorld}");
#endif
        }
    }
}