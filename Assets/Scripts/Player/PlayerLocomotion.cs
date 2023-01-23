using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class PlayerLocomotion : MonoBehaviour
{
    [Header("Movement Settings")]

    [Range(0f, 25f)]
    public float MovementSpeed = 9f;

    [Range(0f, 25f)]
    public float AirStrafeSpeed = 3f;

    [Range(0f, 1000f)]
    public float Acceleration = 200f;

    [Range(0f, 1000f)]
    public float MaxAccelerationForce = 150f;

    [HideInInspector]
    public float InitialSpeed;

    public bool LockCursor = false;

    [HideInInspector]
    public bool IsInAir = false;
    [HideInInspector]
    public bool IsRunning = false;
    [HideInInspector]
    public bool IsDebuffed = false;


    Rigidbody _rb;
    Vector3 _direction = Vector3.zero;
    Vector3 _inputVector = Vector3.zero;
    Vector3 _mouseVector = Vector3.zero;
    GameObject _mouseHover;
    PlayerGrab _grab;

    bool _switchedCells = false;

    public Vector3 Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            _direction.y = 0;

            if (_direction.magnitude > 1)
                _direction.Normalize();
        }
    }

    public Vector3 Velocity
    {
        get => _rb.velocity;
        set => _rb.velocity = value;
    }

    public Vector3 InputVector => _inputVector;
    public Vector3 MouseVector => _mouseVector;
    public GameObject MouseHover => _mouseHover;

    void Start()
    {
        InitialSpeed = MovementSpeed;

        _rb = GetComponent<Rigidbody>();
        _grab = GetComponent<PlayerGrab>();

        if (_rb == null)
            Debug.LogError("Rigidbody not found on object");

        if (_grab == null)
            Debug.LogError("PlayerGrab not found on object");
    }

    void Update()
    {
        _inputVector = InputToVector();
        _mouseVector = MousePosToVector();
        IsRunning = Input.GetKey(KeyCode.LeftShift);

        SaveDirection();
    }

    void FixedUpdate()
    {
        // Calculate goal velocity
        Vector3 goalVelocity = _inputVector * (IsInAir ? AirStrafeSpeed : MovementSpeed);

        // Calculate acceleration
        Vector3 acceleration = goalVelocity - _rb.velocity;
        acceleration = Vector3.ClampMagnitude(acceleration, MaxAccelerationForce);

        // Apply acceleration
        _rb.AddForce(acceleration * Acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    void OnDrawGizmos()
    {
        if (Direction != Vector3.zero)
        {
            Handles.color = Color.gray;
            Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(Direction), 1f, EventType.Repaint);
        }
        if (_inputVector != Vector3.zero)
        {
            Handles.color = Color.green;
            Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(_inputVector), 1f, EventType.Repaint);
        }
        if (_mouseVector != Vector3.zero)
        {
            Handles.color = Color.red;
            Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(_mouseVector), 1f, EventType.Repaint);
        }
        if (_mouseHover != null)
        {
            Handles.color = Color.yellow;
            Handles.DrawWireCube(_mouseHover.transform.position, _mouseHover.transform.localScale);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == ("Ground"))
            IsInAir = false;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == ("Ground"))
            IsInAir = true;
    }

    Vector3 InputToVector()
    {
        // Get input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Calculate force vector
        var forceVector = new Vector3(h, 0, v);

        if (forceVector.magnitude > 1)
            forceVector.Normalize();

        return forceVector;
    }

    Vector3 MousePosToVector()
    {
        // Get mouse position
        Vector3 mousePos = Input.mousePosition;

        // Raycast to mouse position
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        // Trace to ground layer
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 direction = hit.point - transform.position;
            direction.y = 0;

            if (hit.collider && hit.collider.gameObject.tag == "Grabbable")
                _mouseHover = hit.collider.gameObject;
            else
                _mouseHover = null;

            return direction.normalized;
        }

        return Vector3.zero;
    }

    void SaveDirection()
    {
        if (_inputVector.magnitude == 0)
            _switchedCells = false;

        if (_grab && _grab.GrabbableHasGridEffect())
        {
            if (_inputVector.magnitude == 0)
                return;

            if (_switchedCells)
                return;

            ContainerGridCell curCell = _grab.GetActiveContainer().CurrentCell;
            ContainerGridCell nextCell = curCell.Tile.GetNext<ContainerGridCell>(curCell.Tile.GridPosition, _inputVector);

            if (nextCell != null)
            {
                Vector3 direction = nextCell.Tile.WorldCenter - transform.position;
                direction.y = 0;

                if (direction.magnitude > curCell.Tilemap.CellSize.x * (2f - 0.125f)) // allows approx. 3 tiles in radius
                    return;

                if (direction.magnitude > 1)
                    direction.Normalize();

                Direction = direction;

                nextCell.AdoptGrabbable(_grab.GetActiveContainer());
                _switchedCells = true;
            }
            return;
        }

        if (_mouseVector.magnitude > 0 && !LockCursor)
            Direction = _mouseVector;
        if (LockCursor && _inputVector.magnitude > 0)
            Direction = _inputVector;
    }
}
