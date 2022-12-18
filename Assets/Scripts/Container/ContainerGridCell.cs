using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ContainerGridCell : MonoBehaviour
{
    public float CellSize = 1f;
    public float CellHeight = 3f;

    public List<ContainerGeneric> Grabbables = new List<ContainerGeneric>();

    Collider _collider = null;

    void Start()
    {
        _collider = GetComponent<Collider>();

        // If no collider, add one
        if (_collider == null)
        {
            _collider = gameObject.AddComponent<BoxCollider>();
            _collider.transform.localScale = new Vector3(CellSize / 2, CellHeight, CellSize / 2);
        }

        _collider.isTrigger = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(CellSize, CellHeight, CellSize));
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag != "Grabbable")
            return;

        ContainerGeneric container = collider.GetComponent<ContainerGeneric>();

        if (container == null)
        {
            Debug.LogError("ContainerGridCell: OnTriggerEnter: ContainerGeneric component not found on grabbable");
            return;
        }

        if (container.ParentCell == null || container.ParentCell == this)
            AdoptGrabbable(container);
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag != "Grabbable")
            return;

        ContainerGeneric container = collider.GetComponent<ContainerGeneric>();

        if (container == null)
        {
            Debug.LogError("ContainerGridCell: OnTriggerExit: ContainerGeneric component not found on grabbable");
            return;
        }

        if (container.ParentCell != this)
            return;

        DisownGrabbable(container);
    }

    public void AdoptGrabbable(ContainerGeneric container)
    {
        if (container == null)
            return;

        // Add to list
        if (!Grabbables.Contains(container))
            Grabbables.Add(container);

        container.ParentCell = this;
    }

    public void DisownGrabbable(ContainerGeneric container, bool force = false)
    {
        // If no grabbable or grabbable is not ours, return (unless force is true)
        if (container == null || (!force && container.ParentCell != this))
            return;

        // remove from list
        if (Grabbables.Contains(container))
            Grabbables.Remove(container);

        container.ParentCell = null;
    }

    public Vector2 GetPos2D()
    {
        return transform.parent.GetComponent<ContainerGrid>().GetCellPos2D(this);
    }
}
