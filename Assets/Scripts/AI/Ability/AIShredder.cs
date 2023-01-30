using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using GD.MinMaxSlider;

public class AIShredder : MonoBehaviour, ICustomBehavior
{
    [MinMaxSlider(0f, 10f)]
    public Vector2 SearchDelay = new Vector2(3f, 5f);
    [MinMaxSlider(0f, 10f)]
    public Vector2 EatDelay = new Vector2(3f, 5f);
    public int Damage = 4;

    bool _enableWander = false;

    public bool WanderWhileUsingAbility => _enableWander;

    public bool Enabled
    {
        get => enabled;
        set => enabled = value;
    }

    TileGrid _grid;
    ContainerGeneric _targetContainer;
    TileGeneric _targetContainerTile;
    TileGeneric _targetTile;

    AIWander _wander;

    float _nextSearch = -1f;
    float _nextEat = -1f;
    bool _onLastTarget = false, _reachedTarget = false;

    void Awake()
    {
        _grid = FindObjectOfType<TileGrid>();
        _wander = GetComponent<AIWander>();
        if (_wander == null)
            Debug.LogError("No AIWander found on object!");

        _wander.RandomWander = false;
        _wander.OnLastTarget += OnTargetReached;
    }

    void OnDisable()
    {
        _wander.RandomWander = true;
        _wander.OnLastTarget -= OnTargetReached;
    }

    public void Ability()
    {
        if (_grid == null)
            return;

        SearchForTarget();
        SetAIWanderTarget();

        // Check if target tile is still walkable
        if (_targetContainerTile != null && _targetTile != null && !IsWalkable(_targetTile) && !_onLastTarget)
            FindNewNeigbor();

        if (!_reachedTarget)
            return;

        // If current tile changed
        var currentTile = _grid.GetTile(transform.position);
        if (currentTile.GridPosition != _targetTile.GridPosition)
            FindNewNeigbor();

        // Face the target
        LookAtTarget(_targetContainerTile);

        // If not close enough to target, move towards it
        float distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), _targetTile.WorldCenter);
        if (distance > 0.1f)
            MoveTowardsTarget(_targetTile);

        EatContainer();
    }

    bool IsWalkable(TileGeneric tile)
    {
        if (tile == null)
            return false;

        List<GameObject> objects = tile.GetObjects();

        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] == gameObject)
                continue;

            // Ignore if ground layer
            if (objects[i].layer == 3)
                continue;

            // Ignore if clip brush
            if (objects[i].layer == 4)
                continue;

            var gridCell = objects[i].GetComponent<TileGridCell>();
            if (gridCell != null && gridCell.Broken)
                return false;

            var damageable = objects[i].GetComponent<TileDamageable>();
            if (damageable != null && (damageable.OnFire || damageable.Health <= 0))
                continue;

            // Ignore if current tile is the object
            if (objects[i].transform.position == tile.WorldCenter)
                continue;

            var collider = objects[i].GetComponent<Collider>();
            if (collider != null && !collider.isTrigger)
                return false;
        }
        return true;
    }

    void OnLastTarget()
    {
        _onLastTarget = true;
    }

    void OnTargetReached()
    {
        // Stop setting a target
        _reachedTarget = true;
        _enableWander = false;
    }

    void EatContainer()
    {
        if (_nextEat <= 0f)
        {
            _nextEat = Time.time + Random.Range(EatDelay.x, EatDelay.y);
        }
        else if (Time.time > _nextEat)
        {
            if (_targetContainer != null)
                _targetContainer.Damage(Damage);

            _nextEat = -1f;

            // If container health is 0, reset path
            if (_targetContainer != null && _targetContainer.Health <= 0)
                ResetPathVars();
        }
    }

    GameObject ClosestObject(List<GameObject> objects)
    {
        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] == gameObject)
                continue;

            var distance = Vector3.Distance(objects[i].transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = objects[i];
            }
        }

        return closestObject;
    }

    void SearchForTarget()
    {
        if (_nextSearch <= 0f && _targetContainer == null)
        {
            _nextSearch = Time.time + Random.Range(SearchDelay.x, SearchDelay.y);
        }
        else if (Time.time > _nextSearch && _targetContainer == null)
        {
            // Target random Grabbable tag that is not this object
            var targets = GameObject.FindGameObjectsWithTag("Grabbable").Where(x => x != gameObject && x.GetComponent<AIBehavior>() == null);
            if (targets.Count() > 0)
            {
                var target = ClosestObject(targets.ToList());

                // Get the tile the target is on
                var tile = _grid.GetTile(target.transform.position);

                // Get the lowest object on the tile
                var objects = tile.GetObjects();

                GameObject lowestObject = null;
                TileGeneric lowestTile = null;
                float lowestY = Mathf.Infinity;

                for (int i = 0; i < objects.Count; i++)
                {
                    // If not grabbable, ignore
                    if (!objects[i].CompareTag("Grabbable"))
                        continue;

                    // Get tile the object is on
                    var objectTile = _grid.GetTile(objects[i].transform.position);

                    if (objectTile == null)
                        continue;

                    // Check if any of the tiles around the object are empty, if none of them are, ignore
                    if (GetEmptyNeighbors(objectTile).Count == 0)
                        continue;

                    // Get object collider
                    var collider = objects[i].GetComponent<Collider>();
                    if (collider == null)
                        continue;

                    if (collider.bounds.min.y < lowestY)
                    {
                        lowestY = collider.bounds.min.y;
                        lowestObject = objects[i];
                        lowestTile = objectTile;
                    }
                }

                // If we found a target, set it
                if (lowestObject != null)
                {
                    _targetContainer = lowestObject.GetComponent<ContainerGeneric>();
                    _targetContainerTile = lowestTile;
                }
            }

            _nextSearch = -1f;
        }
    }

    void SetAIWanderTarget()
    {
        if (_targetContainerTile != null && _targetTile == null && !_onLastTarget)
        {
            // Select random walkable tile around the target
            var emptyNeighbors = GetEmptyNeighbors(_targetContainerTile);
            if (emptyNeighbors.Count == 0)
                return;

            _targetTile = emptyNeighbors[Random.Range(0, emptyNeighbors.Count)];
        }

        if (_targetTile != null && !_onLastTarget)
        {
            _enableWander = true;
            _wander.SetTarget(_targetTile.WorldCenter);
        }
    }

    void LookAtTarget(TileGeneric target)
    {
        Vector3 direction = target.WorldCenter - transform.position;
        direction.y = 0;
        direction.Normalize();

        float angle = Vector3.Angle(transform.forward, direction);

        // Rotate towards target
        _wander.ApplyTorque(direction);
    }

    void MoveTowardsTarget(TileGeneric target)
    {
        Vector3 direction = target.WorldCenter - transform.position;
        direction.y = 0;
        direction.Normalize();

        _wander.ApplyForce(direction, _wander.MovementSpeed);
    }

    List<TileGeneric> GetEmptyNeighbors(TileGeneric tile)
    {
        var neighbors = tile.GetNeighbors();

        List<TileGeneric> emptyNeighbors = new List<TileGeneric>();
        for (int j = 0; j < neighbors.Count; j++)
        {
            var objectsOnNeighbor = neighbors[j].GetObjects().Where(x => x.CompareTag("Grabbable")).ToList();
            if (objectsOnNeighbor.Count == 0)
            {
                emptyNeighbors.Add(neighbors[j]);
            }
        }

        return emptyNeighbors;
    }

    void ResetPathVars()
    {
        _reachedTarget = false;
        _onLastTarget = false;
        _targetContainer = null;
        _targetContainerTile = null;
        _targetTile = null;
    }

    void FindNewNeigbor()
    {
        // If no neighbors are walkable, set target to null
        if (GetEmptyNeighbors(_targetContainerTile).Count == 0)
        {
            ResetPathVars();
        }
        else
        {
            // If there are walkable neighbors, set target to one of them
            var neighbors = GetEmptyNeighbors(_targetContainerTile);
            _targetTile = neighbors[Random.Range(0, neighbors.Count)];
            _reachedTarget = false;
            _onLastTarget = false;
        }
    }

    void OnDrawGizmos()
    {
        if (_targetContainer != null)
        {
            // Draw box around target
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_targetContainer.transform.position, _targetContainer.transform.localScale);
        }
    }
}
