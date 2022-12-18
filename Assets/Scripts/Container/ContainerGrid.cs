using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ContainerGrid : MonoBehaviour
{
    public int Width = 6;
    public int Length = 3;
    public float CellHeight = 3f;
    public float CellSize = 1f;

    [ReadOnly]
    public List<GameObject> Cells = new List<GameObject>();

    void Start()
    {
        List<Vector3> cellLocations = CalcCellLocations();

        // Create cells
        foreach (Vector3 cellLocation in cellLocations)
        {
            GameObject cell = new GameObject("Cell");
            cell.transform.position = cellLocation + Vector3.up * CellHeight / 2;
            cell.transform.parent = transform;
            Cells.Add(cell);

            ContainerGridCell containerGridCell = cell.GetComponent<ContainerGridCell>();

            if (containerGridCell == null)
            {
                containerGridCell = cell.AddComponent<ContainerGridCell>();
                containerGridCell.CellSize = CellSize;
                containerGridCell.CellHeight = CellHeight;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        List<Vector3> cellLocations = CalcCellLocations();

        foreach (Vector3 cellLocation in cellLocations)
        {
            Gizmos.DrawWireCube(cellLocation + Vector3.up * CellHeight / 2, new Vector3(CellSize, CellHeight, CellSize));
        }
    }

    List<Vector3> CalcCellLocations()
    {
        List<Vector3> cellLocations = new List<Vector3>();

        for (int y = 0; y < Length; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Vector3 cellLocation = transform.position + new Vector3(x * CellSize, 0, y * CellSize);
                cellLocations.Add(cellLocation);
            }
        }

        return cellLocations;
    }

    public Vector2 GetCellPos2D(ContainerGridCell cell)
    {
        // Check in the list of cells
        int index = Cells.IndexOf(cell.gameObject);

        if (index == -1)
            return new Vector2(-1, -1);

        int x = index % Width;
        int y = index / Width;

        return new Vector2(x, y);
    }

    public ContainerGridCell GetCellAtPos(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Length)
            return null;

        int index = x + y * Width;
        return Cells[index].GetComponent<ContainerGridCell>();
    }

    public ContainerGridCell GetCellInDirection(ContainerGridCell cell, Vector3 direction)
    {
        Vector2 cellPos = GetCellPos2D(cell);
        Vector2 newPos = cellPos + new Vector2(direction.x, direction.z);

        return GetCellAtPos((int)newPos.x, (int)newPos.y);
    }
}
