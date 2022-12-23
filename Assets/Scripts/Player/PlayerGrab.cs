using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    [Header("Grab Settings")]
    [Range(0f, 10f)]
    public float AutoGrabRadius = 1f;

    [Header("Air Movement Settings")]
    public float FloatAmount = 3f;

    [Range(0f, 1000f)]
    public float Acceleration = 500f;

    [Range(0f, 10f)]
    public float DecelerationMultiplier = 3.5f;

    [Header("Visualization Settings")]
    public Material OutlineMaterial;

    [HideInInspector]
    public GameObject GrabbedObject;

    GameObject _lastClosestObject = null;
    PlayerLocomotion _locomotion;

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
        GameObject closestObject = ScanForGrabbables(AutoGrabRadius);

        // Remove outline from the previous closest object
        if (closestObject != null && closestObject != _lastClosestObject)
        {
            MatSystem.AddMaterial(closestObject, OutlineMaterial);
            if (_lastClosestObject != null)
                MatSystem.RemoveMaterial(_lastClosestObject, OutlineMaterial);

            _lastClosestObject = closestObject;
        }
        else if (closestObject == null && _lastClosestObject != null)
        {
            MatSystem.RemoveMaterial(_lastClosestObject, OutlineMaterial);
            _lastClosestObject = null;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (closestObject != null && GrabbedObject == null)
            {
                GrabbedObject = closestObject;
                GrabbedObject.transform.SetParent(transform);

                // Set direction to object
                _locomotion.Direction = GrabbedObject.transform.position - transform.position;

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

    GameObject ScanForGrabbables(float radius)
    {
        // Get a list of all colliders in the radius
        List<Collider> colliders = new List<Collider>(Physics.OverlapSphere(transform.position, radius));

        if (colliders.Count == 0)
            return null;

        float closestDistance = -1f;
        GameObject closestObject = null;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag != "Grabbable")
                continue;

            float distance = Vector3.Distance(transform.position, collider.transform.position);

            // Apply weight according to _locomotion.MouseVector
            Vector3 direction = _locomotion.MouseVector;

            // Get the angle between the player's direction and the object's direction
            float angle = Vector3.Angle(direction, collider.transform.position - transform.position);

            // Apply weight according to the angle
            distance *= 1f + angle / 180f;

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
        Gizmos.DrawWireSphere(transform.position, AutoGrabRadius);
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
