using UnityEngine;
using UnityEditor;

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
    public Vector3 Direction;
    [HideInInspector]
    public bool IsInAir = false;
    [HideInInspector]
    public bool IsRunning = false;

    public bool LockCursor = false;

    Rigidbody _rb;
    Vector3 _inputVector;
    Vector3 _mouseVector;
    PlayerGrab _grab;

    bool _switchedCells = false;

    public Vector3 Velocity
    {
        get { return _rb.velocity; }
        set { _rb.velocity = value; }
    }

    public Vector3 InputVector
    {
        get { return _inputVector; }
    }

    public Vector3 MouseVector
    {
        get { return _mouseVector; }
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _grab = GetComponent<PlayerGrab>();
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
        if (Direction == Vector3.zero)
            return;

        Handles.color = Color.gray;
        Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(Direction), 1f, EventType.Repaint);
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
        Vector3 forceVector = new Vector3(h, 0, v);

        if (forceVector.magnitude > 1)
            forceVector.Normalize();

        return forceVector;
    }

    Vector3 MousePosToVector()
    {
        // Get mouse position
        Vector3 mousePos = Input.mousePosition;

        // raycast to mouse position
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        // TODO: Ignore grabbable layer

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 direction;

            // Calculate direction from player to mouse position
            if (hit.collider.gameObject.tag == "Grabbable")
            {
                // Return a direction to the grabbable
                direction = hit.collider.transform.position - transform.position;
                direction.y = 0;

                if (direction.magnitude > 1)
                    direction.Normalize();

                return direction;
            }

            direction = hit.point - transform.position;
            direction.y = 0;

            if (direction.magnitude > 1)
                direction.Normalize();

            return direction;
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

            ContainerGrid grid = _grab.GetActiveGrid();
            if (grid == null)
                return;

            if (_switchedCells)
                return;

            ContainerGridCell curCell = _grab.GetActiveContainer().CurrentCell;
            ContainerGridCell nextCell = grid.GetCellInDirection(curCell, _inputVector);

            if (nextCell != null)
            {
                Vector3 direction = nextCell.transform.position - transform.position;
                direction.y = 0;

                if (direction.magnitude > 1)
                    direction.Normalize();

                Direction = direction;

                nextCell.AdoptGrabbable(_grab.GetActiveContainer());
                _switchedCells = true;
            }
            return;
        }

        if (_mouseVector.magnitude > 0 && !LockCursor)
        {
            Direction = _mouseVector;
            Direction.y = 0;

            if (Direction.magnitude > 1)
                Direction.Normalize();
        }

        if (!LockCursor) return;

        Direction = _inputVector;
        Direction.y = 0;

        if (Direction.magnitude > 1)
            Direction.Normalize();
    }

    public void ForceDirection(Vector3 direction)
    {
        Direction = direction;
        Direction.y = 0;

        if (Direction.magnitude > 1)
            Direction.Normalize();
    }
}
