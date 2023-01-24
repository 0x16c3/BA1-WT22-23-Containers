using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ContainerGeneric : MonoBehaviour, IDamageable
{
    int _health = 6;
    public int Health
    {
        get => _health;
        private set => _health = value;
    }

    [Header("Grid Snap - Air Movement Settings")]
    [Range(0f, 1000f)]
    public float Acceleration = 150f;

    [Range(0f, 10f)]
    public float DecelerationMultiplier = 3.5f;

    [Tooltip("Maximum velocity of the player when grabbing an object")]
    public int MaxPlayerVelocity = 5;

    [HideInInspector]
    public ContainerGridCell ParentCell = null; // Realitme parent cell - can be null

    [HideInInspector]
    public ContainerGridCell CurrentCell = null; // This will always be set to an object while the object is targeting a cell

    [HideInInspector]
    public bool HasGridEffect = false;

    float _lastHadGridEffect = 0f;

    bool _disableEffect = false;

    float COYOTE_TIME = 0.3f;

    Rigidbody _rb;
    TileGrid _tilemap;

    int _maxHealth = -1;

    void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
            Debug.LogError("Rigidbody not found on object");

        _tilemap = TileGrid.FindTilemap();

        _maxHealth = Health;
    }

    void Update()
    {
        PushTowardsParentCell();
        ProcessCoyoteTime();
        CorrectRotation();
    }

    void ProcessCoyoteTime()
    {
        if (!HasGridEffect && ParentCell != null)
        {
            HasGridEffect = true;
            CurrentCell = ParentCell;
        }

        // Reset current cell if it's been changed externally
        if (ParentCell != null && CurrentCell != null && CurrentCell != ParentCell)
        {
            CurrentCell = ParentCell;
            _disableEffect = false;
            _lastHadGridEffect = 0f;
        }

        // Disable grid effect if the object has been in the air for a while
        if (_disableEffect && _lastHadGridEffect + COYOTE_TIME < Time.time && ParentCell == null)
        {
            HasGridEffect = false;
            CurrentCell = null;
            _disableEffect = false;
        }

        if (HasGridEffect && ParentCell == null)
            _disableEffect = true;

        if (ParentCell != null)
            _lastHadGridEffect = Time.time;
    }

    void OnDrawGizmos()
    {
        var textPosition = transform.position + new Vector3(0, 0.5f, 0);

        if (ParentCell != null)
        {
            Handles.Label(textPosition, "Parent: " + ParentCell.Tile.GridPosition);
            textPosition += new Vector3(0, 0.2f, 0);
        }
    }

    Vector3 Vector2D(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    void PushTowardsParentCell()
    {
        if (ParentCell == null)
            return;

        // Don't apply forces if the player is running faster than the max velocity
        if (transform.parent)
        {
            var locomotion = transform.parent.GetComponent<PlayerLocomotion>();

            if (locomotion != null && locomotion.Velocity.magnitude > MaxPlayerVelocity)
                return;
        }

        Vector3 direction = Vector2D((ParentCell.transform.position - new Vector3(0, _tilemap.CellSize.x / 2, 0)) - transform.position).normalized;

        // Distance to parent cell ignoring y axis
        float distance = Vector3.Distance(Vector2D(transform.position), Vector2D(ParentCell.transform.position));

        // Limit the force so it is always less than gravity
        Vector3 force = direction * Acceleration * Time.deltaTime;

        if (force.magnitude > Physics.gravity.magnitude)
            force = force.normalized * Physics.gravity.magnitude;

        if (distance > 0.1f)
        {
            GetComponent<Rigidbody>().AddForce(force);
        }
        else
        {
            // Apply negative force to keep the object in place
            var rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddForce(-Vector2D(rigidbody.velocity) * DecelerationMultiplier, ForceMode.Acceleration);
        }
    }

    void CorrectRotation()
    {
        if (ParentCell == null)
            return;

        if (_rb.velocity.magnitude < 0.1f)
            return;

        if (transform.parent)
        {
            var playerGrab = transform.parent ? null : transform.parent.GetComponent<PlayerGrab>();

            if (playerGrab == null)
                return;

            if (playerGrab.GrabbedObject == gameObject)
                return;
        }

        Quaternion targetRotation = Quaternion.Euler(0, ParentCell.transform.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    public void Damage(int damage) => Health = Mathf.Clamp(Health - damage, 0, _maxHealth);
    public void Heal(int heal) => Health = Mathf.Clamp(Health + heal, 0, _maxHealth);
}
