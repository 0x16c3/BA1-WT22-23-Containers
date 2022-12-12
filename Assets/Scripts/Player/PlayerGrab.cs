using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerGrab : MonoBehaviour
{
    [Header("Auto Grab Settings")]

    [Tooltip("Scans for grabbable objects in a radius around the player and grabs the closest one")]
    public bool AutoGrab = true;

    [Range(0f, 10f)]
    public float AutoGrabRadius = 1f;

    [Tooltip("Ray cast distance to scan for grabbable objects in player's forward direction, backup if Auto Grab is disabled")]
    [Range(0f, 10f)]
    public float ScanDistance = 2f;

    [Range(0, 360)]
    public int ScanAngle = 30;

    public int ScanDivisions = 15;

    [Header("Air Movement Settings")]

    public float FloatAmount = 3f;

    [Range(0f, 1000f)]
    public float Acceleration = 500f;

    [Range(0f, 10f)]
    public float DecelerationMultiplier = 3.5f;

    string GRABBABLE_TAG = "Grabbable";

    GameObject _grabbedObject;
    PlayerLocomotion _locomotion;

    float _lastScanTime = 0f;

    void Start()
    {
        _locomotion = GetComponent<PlayerLocomotion>();
    }

    void Update()
    {
        GameObject closestObject = AutoGrab ? ScanForGrabbables(AutoGrabRadius) : null;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // If no closes object + auto grab is disabled
            // Scan for grabbable objects in front of the player
            if (closestObject == null && !AutoGrab)
                closestObject = ScanForGrabbable(ScanDistance);

            if (closestObject != null && _grabbedObject == null)
            {
                _grabbedObject = closestObject;
                _grabbedObject.transform.SetParent(transform);

                // Set direction to object
                _locomotion.ForceDirection(_grabbedObject.transform.position - transform.position);

                // Disable collisions only with the player and disable gravity
                Physics.IgnoreCollision(_grabbedObject.GetComponent<Collider>(), GetComponent<Collider>());
                _grabbedObject.GetComponent<Rigidbody>().useGravity = false;
            }
            else if (_grabbedObject != null)
            {
                // Enable collisions with the player and gravity
                Physics.IgnoreCollision(_grabbedObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
                _grabbedObject.GetComponent<Rigidbody>().useGravity = true;

                _grabbedObject.transform.SetParent(null);
                _grabbedObject = null;
            }
        }
    }

    void FixedUpdate()
    {
        if (_grabbedObject != null)
            MoveObject();
    }

    void OnDrawGizmos()
    {
        DebugVisuals();
    }

    IEnumerable<Vector3[]> ScanRays(int angle, int divisions)
    {
        Vector3 direction = _locomotion.Direction;

        for (int i = 0; i < divisions; i++)
        {
            yield return new Vector3[]
            {
                transform.position,
                Quaternion.AngleAxis(-angle / 2f + i * angle / divisions, Vector3.up) * direction
            };
        }
    }

    GameObject ScanForGrabbable(float range)
    {
        _lastScanTime = Time.time;

        Vector3 direction = _locomotion.Direction;

        // Check for 15 ray casts in a 15 degree angle
        foreach (Vector3[] ray in ScanRays(ScanAngle, ScanDivisions))
        {
            RaycastHit hit;
            if (Physics.Raycast(ray[0], ray[1], out hit, range))
            {
                if (hit.collider.gameObject.tag == GRABBABLE_TAG)
                    return hit.collider.gameObject;
            }
        }

        return null;
    }

    GameObject ScanForGrabbables(float radius)
    {
        if (!AutoGrab)
            return null;

        // Get a list of all colliders in the radius
        List<Collider> colliders = new List<Collider>(Physics.OverlapSphere(transform.position, radius));

        if (colliders.Count == 0)
            return null;

        if (!colliders.Any(c => c.gameObject.tag == GRABBABLE_TAG))
            return null;

        float closestDistance = -1f;
        GameObject closestObject = null;

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag != GRABBABLE_TAG)
                continue;

            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (closestDistance == -1f || distance < closestDistance)
            {
                closestObject = collider.gameObject;
                closestDistance = distance;
            }
        }

        return closestObject;
    }

    void MoveObject()
    {
        // Get last direction from PlayerLocomotion script
        Vector3 direction = _locomotion.Direction;

        // Calculate goal position and distance to it
        Vector3 goalPosition = transform.position + (direction * 2f) + Vector3.up * FloatAmount;
        Vector3 goalDir = goalPosition - _grabbedObject.transform.position;
        float distance = Vector3.Distance(goalPosition, _grabbedObject.transform.position);

        // Add acceleration towards goal position, but only if the object is not already there, decrease speed while getting closer
        _grabbedObject.GetComponent<Rigidbody>().AddForce(goalDir * Acceleration * (distance * 0.5f) * Time.fixedDeltaTime, ForceMode.Acceleration); // TEST: GetComponent performance hit?

        // Apply negative force to keep the object in place
        _grabbedObject.GetComponent<Rigidbody>().AddForce(-_grabbedObject.GetComponent<Rigidbody>().velocity * DecelerationMultiplier, ForceMode.Acceleration);
    }

    void DebugVisuals()
    {
        if (AutoGrab)
            Gizmos.DrawWireSphere(transform.position, AutoGrabRadius);
        else
        {
            if (_lastScanTime + 0.1f < Time.time)
                return;

            // Check for 15 ray casts in a 15 degree angle
            foreach (Vector3[] ray in ScanRays(ScanAngle, ScanDivisions))
            {
                Gizmos.DrawRay(ray[0], ray[1] * ScanDistance);
            }
        }
    }
}
