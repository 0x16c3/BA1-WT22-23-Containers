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

    Rigidbody _rb;
    Vector3 _inputVector;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _inputVector = InputToVector();
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

    void SaveDirection()
    {
        if (_inputVector.magnitude == 0)
            return;

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
