using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerLocomotion : MonoBehaviour
{
    [Header("Movement Settings")]

    public float TpLen = 0.1f;

    [Range(0f, 25f)]
    public float MovementSpeed = 9f;

    [Range(0f, 25f)]
    public float SlowSpeed = 9f;

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
    public bool OnRamp = false;
    [HideInInspector]
    public bool HasSlowEffect = false;

    Rigidbody _rb;
    Vector3 _direction = Vector3.zero;
    Vector3 _inputVector = Vector3.zero;
    Vector3 _mouseVector = Vector3.zero;
    Vector3 _mousePos = Vector3.zero;
    GameObject _mouseHover;
    PlayerGrab _grab;

    // For Animation
    Transform _playerModel;
    Animator _playerAnimator;

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
    public Vector3 MousePos => _mousePos;
    public GameObject MouseHover => _mouseHover;

    void Start()
    {
        InitialSpeed = MovementSpeed;

        _playerModel = transform.Find("Jeffrey");
        _playerAnimator = _playerModel.GetComponent<Animator>();
        _playerAnimator.SetFloat("Speed", InitialSpeed);

        if (_playerModel == null || _playerAnimator == null)
        {
            Debug.LogError("No player model found or player animator");
        }
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

        SaveDirection();

        if (_inputVector != Vector3.zero)
        {
            _playerAnimator.SetInteger("AnimationTypes", 1);
        }
        else
        {
            _playerAnimator.SetInteger("AnimationTypes", 0);
        }

    }

    void FixedUpdate()
    {
        // Calculate goal velocity
        Vector3 goalVelocity = _inputVector * (IsInAir ? AirStrafeSpeed : HasSlowEffect ? SlowSpeed : MovementSpeed);

        // Calculate acceleration
        Vector3 acceleration = goalVelocity - _rb.velocity;
        acceleration = Vector3.ClampMagnitude(acceleration, MaxAccelerationForce);
        acceleration = acceleration * Acceleration;

        if (IsInAir)
        {
            // Apply extra downwards force if in air
            acceleration += new Vector3(0, -Acceleration * 10f, 0);
        }

        // Apply acceleration
        _rb.AddForce(acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);

        if (_inputVector != Vector3.zero)
        {
            _playerModel.rotation = Quaternion.RotateTowards(
                _playerModel.rotation, Quaternion.LookRotation(_inputVector), 500 * Time.fixedDeltaTime);
        }

    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
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
#endif
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == ("Ground Plane"))
            IsInAir = false;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == ("Ground Plane") && !OnRamp)
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

            _mousePos = hit.point;

            if (hit.collider && hit.collider.gameObject.tag == "Grabbable")
                _mouseHover = hit.collider.gameObject;
            else
                _mouseHover = null;

            return direction.normalized;
        }

        _mouseHover = null;
        _mousePos = Vector3.zero;

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

            TileGridCell curCell = _grab.GetActiveContainer().CurrentCell;
            TileGridCell nextCell = curCell.Tile.GetNext<TileGridCell>(curCell.Tile.GridPosition, _inputVector);

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
