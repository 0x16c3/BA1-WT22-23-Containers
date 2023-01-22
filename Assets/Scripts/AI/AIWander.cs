using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class AIWander : MonoBehaviour
{
    [Header("Wander Settings")]
    public float IntervalMin = 1.0f;
    public float IntervalMax = 3.0f;
    public float MovementRadius = 6.0f;
    public float MinMovementRadius = 3.0f;

    public bool Suicidal = true;

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
    TileGrid _tilemap;
    TileGeneric _tile;
    ContainerGeneric _container;

    PathFinder _pathFinder;
    List<PathTile> _paths = new List<PathTile>();

    Vector3 _lastRandPoint;
    PathTile _currentTarget;

    bool _reachedTarget => _paths.Count > 0 && _tile.GridPosition == _paths[_paths.Count - 1].GridPosition;

    bool _correctedRotation = false;
    bool _containerGrabbed => _container != null && _container.IsGrabbed;
    bool _containerWasGrabbed = false;
    bool _containerDropped = true;
    bool _grounded => !_containerGrabbed && Mathf.Abs(_rb.velocity.y) < 0.1f;

    void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();

        _tilemap = TileGrid.FindTilemap();
        _tile = _tilemap.GetTile(transform.position);

        _container = GetComponent<ContainerGeneric>();

        _pathFinder = new PathFinder(_tilemap, this.gameObject);
        _pathFinder.OnPathReset += ResetPathData;

        Wander();
    }

    void OnDisable()
    {
        _pathFinder.OnPathReset -= ResetPathData;
    }

    void Wander()
    {
        Vector3 randomPoint = GetRandomPoint();
        while (Vector3.Distance(randomPoint, _lastRandPoint) < MinMovementRadius || Vector3.Distance(randomPoint, transform.position) < MinMovementRadius)
        {
            randomPoint = GetRandomPoint();
        }

        _lastRandPoint = randomPoint;

        TileGeneric randomTile = _tilemap.GetTile(randomPoint);

        _pathFinder.SetStart(_tile.GridPosition);
        _pathFinder.SetTarget(randomTile.GridPosition);
        _pathFinder.InitPath();
    }

    void ResetPathData()
    {
        // todo: reset path data after it's been grabbed

        _lastRandPoint = Vector3.zero;
        _paths = new List<PathTile>();
        _currentTarget = null;
        _correctedRotation = false;
    }

    void Update()
    {
        if (!_tilemap)
            return;

        UpdateContainerStates();

        if (Input.GetKeyDown(KeyCode.Space))
            Wander();

        _pathFinder.FindPath();
        _paths = _pathFinder.Path;
        _tile = _tilemap.GetTile(transform.position);
    }

    void FixedUpdate()
    {
        if (_paths == null || _paths.Count == 0)
            return;

        if (_currentTarget == null)
            _currentTarget = _pathFinder.GetNextInPath(_tile.GridPosition);

        if (!_grounded)
            return;

        SwitchTarget();
        FaceDirection(_pathFinder.GetNextInPath(_tile.GridPosition)); // Next tile in path independent of current target
        MoveTowards(_currentTarget);
    }

    Vector3 GetRandomPoint()
    {
        Vector3 randomPoint = Random.insideUnitSphere * MovementRadius;
        randomPoint += transform.position;
        randomPoint.y = 0;

        if (Suicidal)
            return randomPoint;

        var tile = _tilemap.GetTile(randomPoint);

        // Check if there is a ContainerGridCell at that point
        if (!tile.GetInstantiatedObject<ContainerGridCell>())
            return GetRandomPoint();

        return randomPoint;
    }

    void FaceDirection(PathTile tile)
    {
        if (tile == null)
            return;

        // Calculate direction towards target
        Vector3 direction = tile.WorldCenter - transform.position;
        direction.y = 0;
        direction.Normalize();

        // Rotate towards target
        ApplyTorque(direction);

        // If direction is low enough, mark it as corrected
        if (Vector3.Angle(transform.forward, direction) < 1f)
            _correctedRotation = true;
    }

    void MoveTowards(PathTile tile)
    {
        if (tile == null)
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
        if (_currentTarget == null)
            return;

        if (_currentTarget == _paths[_paths.Count - 1])
            return;

        // If we're not facing the right direction, don't switch
        if (!_correctedRotation)
            return;

        var distanceWorld = Vector3.Distance(transform.position, _currentTarget.WorldCenter + Vector3.up * _tilemap.CellSize.x);
        if (distanceWorld < _tilemap.cellSize.x / 2)
        {
            _currentTarget = _pathFinder.GetNextInPath(_tile.GridPosition);
            _correctedRotation = false;
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

    void UpdateContainerStates()
    {
        if (_containerGrabbed)
            _containerWasGrabbed = true;

        if (_containerWasGrabbed && !_containerGrabbed)
        {
            _containerWasGrabbed = false;
            _containerDropped = true;
        }

        // if not falling & dropped
        if (_containerDropped && _grounded)
        {
            _containerDropped = false;
            _pathFinder.SetStart(_tile.GridPosition);
            _pathFinder.InitPath();
        }
    }

    void OnDrawGizmos()
    {
        if (_tilemap == null)
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

            var distanceWorld = Vector3.Distance(transform.position, _currentTarget.WorldCenter);
            Handles.Label(_currentTarget.WorldCenter, $"Distance: {distanceWorld}");
        }
    }
}