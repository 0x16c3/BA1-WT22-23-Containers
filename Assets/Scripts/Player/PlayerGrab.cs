using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerGrab : MonoBehaviour
{
    [Header("Grab Settings")]

    [Tooltip("Scans for grabbable objects in a radius around the player and grabs the closest one"), ReadOnly]
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

    [HideInInspector]
    public GameObject GrabbedObject;

    PlayerLocomotion _locomotion;

    float _lastScanTime = 0f;

    void Start()
    {
        _locomotion = GetComponent<PlayerLocomotion>();
        if (_locomotion == null)
            Debug.LogError("PlayerLocomotion script not found on player");
    }

    void OnValidate()
    {
        _locomotion = GetComponent<PlayerLocomotion>();
        if (_locomotion == null)
            Debug.LogError("PlayerLocomotion script not found on player");
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

            if (closestObject != null && GrabbedObject == null)
            {
                GrabbedObject = closestObject;
                GrabbedObject.transform.SetParent(transform);

                // Set direction to object
                _locomotion.ForceDirection(GrabbedObject.transform.position - transform.position);

                // Disable collisions only with the player and disable gravity
                Physics.IgnoreCollision(GrabbedObject.GetComponent<Collider>(), GetComponent<Collider>());
                GrabbedObject.GetComponent<Rigidbody>().useGravity = false;
            }
            else if (GrabbedObject != null)
            {
                // Enable collisions with the player and gravity
                Physics.IgnoreCollision(GrabbedObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
                GrabbedObject.GetComponent<Rigidbody>().useGravity = true;

                GrabbedObject.transform.SetParent(null);
                GrabbedObject = null;
            }
        }
    }

    void FixedUpdate()
    {
        if (GrabbedObject != null)
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
                if (hit.collider.gameObject.tag == "Grabbable")
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

        // THERE IS A BETTER WAY TO DO THIS - emir
        float closestDistance = -1f;
        GameObject closestObject = null;

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag != "Grabbable")
                continue;

            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (closestDistance == -1f || distance < closestDistance)
            {
                closestObject = collider.gameObject;
                closestDistance = distance;
            }
        }

        if (closestObject == null)
            return null;

        float ceilingDistance = 3f;
        Collider[] aboveColliders = Physics.OverlapBox(closestObject.transform.position + Vector3.up * ceilingDistance / 2f, new Vector3(0.5f, ceilingDistance / 2f, 0.5f));

        // Check if there is a grabbable object above the closest one and keep getting the closest one
        if (aboveColliders.Length > 0)
        {
            // Get the highest object
            GameObject highestObject = null;

            foreach (Collider collider in aboveColliders)
            {
                if (collider.gameObject.tag != "Grabbable")
                    continue;

                if (highestObject == null || collider.transform.position.y > highestObject.transform.position.y)
                    highestObject = closestObject = collider.gameObject;
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
        Vector3 goalDir = goalPosition - GrabbedObject.transform.position;
        float distance = Vector3.Distance(goalPosition, GrabbedObject.transform.position);

        // Add acceleration towards goal position, but only if the object is not already there, decrease speed while getting closer
        GrabbedObject.GetComponent<Rigidbody>().AddForce(goalDir * Acceleration * (distance * 0.5f) * Time.fixedDeltaTime, ForceMode.Acceleration); // TEST: GetComponent performance hit?

        // Apply negative force to keep the object in place
        GrabbedObject.GetComponent<Rigidbody>().AddForce(-GrabbedObject.GetComponent<Rigidbody>().velocity * DecelerationMultiplier, ForceMode.Acceleration);
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

    public ContainerGrid GetActiveGrid()
    {
        if (GrabbedObject == null) return null;

        ContainerGeneric container = GrabbedObject.GetComponent<ContainerGeneric>();
        return container == null ? null : container.CurrentCell.transform.parent.GetComponent<ContainerGrid>();
    }

    public ContainerGeneric GetActiveContainer()
    {
        if (GrabbedObject == null) return null;

        ContainerGeneric container = GrabbedObject.GetComponent<ContainerGeneric>();
        return container == null ? null : container;
    }

    public bool GrabbableHasGridEffect()
    {
        if (GrabbedObject == null) return false;

        ContainerGeneric container = GrabbedObject.GetComponent<ContainerGeneric>();
        return container == null ? false : container.HasGridEffect;
    }
}
