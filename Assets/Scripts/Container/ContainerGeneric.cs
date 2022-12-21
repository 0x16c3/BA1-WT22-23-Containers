using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ContainerGeneric : MonoBehaviour
{
    [Header("Grid Snap - Air Movement Settings")]

    [Range(0f, 1000f)]
    public float Acceleration = 150f;

    [Range(0f, 10f)]
    public float DecelerationMultiplier = 3.5f;

    [HideInInspector]
    public ContainerGridCell ParentCell = null; // Realitme parent cell - can be null

    [HideInInspector]
    public ContainerGridCell CurrentCell = null; // This will always be set to an object while the object is targeting a cell

    [HideInInspector]
    public bool HasGridEffect = false;

    float _lastHadGridEffect = 0f;

    bool _disableEffect = false;

    float COYOTE_TIME = 0.3f;

    void Update()
    {
        PushTowardsParentCell();
        ProcessCoyoteTime();
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
        Vector3 textPosition = transform.position + new Vector3(0, 0.5f, 0);

        if (ParentCell != null)
        {
            Handles.Label(textPosition, "Parent: " + ParentCell.GetPos2D());
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

        Vector3 direction = Vector2D((ParentCell.transform.position - new Vector3(0, ParentCell.CellSize / 2, 0)) - transform.position).normalized;

        // Distance to parent cell ignoring y axis
        float distance = Vector3.Distance(Vector2D(transform.position), Vector2D(ParentCell.transform.position));

        if (distance > 0.1f)
        {
            GetComponent<Rigidbody>().AddForce(direction * Acceleration * Time.deltaTime);
        }
        else
        {
            // Apply negative force to keep the object in place
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddForce(-Vector2D(rigidbody.velocity) * DecelerationMultiplier, ForceMode.Acceleration);
        }
    }
}
