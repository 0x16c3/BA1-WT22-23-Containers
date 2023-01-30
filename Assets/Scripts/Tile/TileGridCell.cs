using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class TileGridCell : MonoBehaviour
{
    public float CellHeightMax = 3f;
    public bool Broken = false;

    public List<ContainerGeneric> Grabbables = new List<ContainerGeneric>();

    Collider _collider = null;
    Collider[] _breakColliders = null;
    public TileGrid Tilemap;
    public TileGeneric Tile;

    bool _initialized = false;

    MeshRenderer _groundPlane;

    void Initialize()
    {
        _collider = GetComponent<Collider>();

        Tilemap = TileGrid.FindTileGrid();
        if (!Tilemap.Initialized)
            return;

        Tile = Tilemap.GetTile(transform.position);

        // Set current size
        transform.localScale = new Vector3(Tilemap.CellSize.x, CellHeightMax, Tilemap.CellSize.y);

        // If no collider, add one
        if (_collider == null)
        {
            _collider = gameObject.AddComponent<BoxCollider>();
            _collider.transform.position = transform.position + Vector3.up * CellHeightMax / 2;
            _collider.transform.localScale = new Vector3(Tilemap.cellSize.x / 2, CellHeightMax, Tilemap.cellSize.y / 2);
        }

        // Get all the child objects with layer "BreakCollider"
        _breakColliders = GetComponentsInChildren<Collider>().Where(c => c.gameObject.name == "BreakCollider").ToArray();
        if (_breakColliders.Length == 0)
            Debug.LogError("TileGridCell: No break colliders found on " + name);

        _groundPlane = GetComponentInChildren<MeshRenderer>();

        _collider.isTrigger = true;
        _initialized = true;
    }



    void OnDisable()
    {
        if (_groundPlane != null)
            _groundPlane.enabled = true;
    }

    void Update()
    {
        if (!_initialized)
            Initialize();
    }

    void OnDrawGizmos()
    {
        if (Tilemap == null)
        {
            Tilemap = (TileGrid)GetComponentInChildren<Tilemap>();
            return;
        }

        Gizmos.DrawWireCube(transform.position, new Vector3(Tilemap.cellSize.x, CellHeightMax, Tilemap.cellSize.y));
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag != "Grabbable")
            return;

        var container = collider.GetComponent<ContainerGeneric>();

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

        var container = collider.GetComponent<ContainerGeneric>();

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

        if (!Grabbables.Contains(container))
            Grabbables.Add(container);

        container.ParentCell = this;
    }

    public void DisownGrabbable(ContainerGeneric container, bool force = false)
    {
        // If no grabbable or grabbable is not ours, return (unless force is true)
        if (container == null || (!force && container.ParentCell != this))
            return;

        if (Grabbables.Contains(container))
            Grabbables.Remove(container);

        container.ParentCell = null;
    }

    public void Break()
    {
        Broken = true;
        // Enable break colliders
        for (int i = 0; i < _breakColliders.Length; i++)
            _breakColliders[i].enabled = true;

        _groundPlane.enabled = false;

        // Remove all grabbables
        for (int i = 0; i < Grabbables.Count; i++)
        {
            DisownGrabbable(Grabbables[i], true);
            // Maybe destroy the grabbables?
        }
    }

    public void Repair()
    {
        Broken = false;
        for (int i = 0; i < _breakColliders.Length; i++)
            _breakColliders[i].enabled = false;

        _groundPlane.enabled = true;
    }
}
