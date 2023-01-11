using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AIWander : MonoBehaviour
{
    public float IntervalMin = 1.0f;
    public float IntervalMax = 3.0f;
    public float MovementRadius = 6.0f;
    public float MinMovementRadius = 3.0f;

    public bool Suicidal = true;

    [Range(0f, 25f)]
    public float MovementSpeed = 9f;

    [Range(0f, 1000f)]
    public float Acceleration = 200f;

    [Range(0f, 1000f)]
    public float MaxAccelerationForce = 150f;


    public LayerMask obstacles;

    Rigidbody _rb;
    PathFinder _pathFinder;
    TileGrid _tilemap;
    TileGeneric _tile;
    List<PathTile> _paths = new List<PathTile>();
    Vector3 _lastRandPoint;
    PathTile _currentTarget;

    bool _markClearTarget = false;

    bool Ready => _tilemap != null;

    void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();

        Collider collider = GetComponent<Collider>();
        Vector3 size = collider.bounds.extents;

        _tilemap = TileGrid.FindTilemap();
        _tile = _tilemap.GetTile(transform.position);

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
        _tile = _tilemap.GetTile(transform.position);
        _lastRandPoint = Vector3.zero;
        _paths = new List<PathTile>();
        _markClearTarget = true;
    }

    void Update()
    {
        if (!_tilemap)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Wander();
        }

        _pathFinder.SetStart(_tile.GridPosition);

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

        if (_currentTarget == null)
            return;

        var distanceWorld = Vector3.Distance(transform.position, _currentTarget.WorldCenter);
        if (distanceWorld < 0.1f)
            _currentTarget = _pathFinder.GetNextInPath(_tile.GridPosition);

        MoveTowards(_currentTarget);

        if (_markClearTarget)
        {
            if (_currentTarget != _pathFinder.EndTile)
                _currentTarget = null;

            _markClearTarget = false;
        }
    }

    void MoveTowards(PathTile tile)
    {
        if (tile == null)
            return;

        Vector3 goalVelocity = (tile.WorldCenter - transform.position).normalized * MovementSpeed;
        Vector3 acceleration = goalVelocity - _rb.velocity;
        acceleration = Vector3.ClampMagnitude(acceleration, MaxAccelerationForce);

        _rb.AddForce(acceleration * Acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    void Fidget()
    {
        // Fidgeting animation with rotation and small movement using physics

        // Store current position and rotation
        Vector3 oldPosition = transform.position;
        Quaternion oldRotation = transform.rotation;

        var strength = Random.Range(0.1f, 0.5f);
        var direction = Random.insideUnitSphere;
        direction.y = 0;

        _rb.AddForce(direction * strength, ForceMode.Impulse);

        var rotation = Random.Range(-90f, 90f);
        Vector3 torque = new Vector3(0, rotation, 0);

        _rb.AddTorque(torque, ForceMode.Impulse);
    }

    Vector3 GetRandomPoint()
    {
        Vector3 randomPoint = Random.insideUnitSphere * MovementRadius;
        randomPoint += transform.position;
        randomPoint.y = 0;

        if (Suicidal)
            return randomPoint;

        // Check if there is a ContainerGridCell at that point
        var tile = _tilemap.GetTile(randomPoint);

        if (!tile.GetInstantiatedObject<ContainerGridCell>())
            return GetRandomPoint();

        return randomPoint;
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
        }
    }
}