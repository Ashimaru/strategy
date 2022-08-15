using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IGrid
{
    Vector3 GridToWorld(Vector3Int position);
    Vector3Int WorldToGrid(Vector3 position);
    int CalculateDistance(Vector3Int from, Vector3Int to);
}

[RequireComponent(typeof(Grid))]
public class GridInfo : MonoBehaviour, IGrid
{
    private Grid grid;
    void Awake()
    {
        grid = GetComponent<Grid>();
        Systems.RegisterSystem<IGrid>(this);
    }

    public Vector3Int WorldToGrid(Vector3 position)
    {
        var result = grid.WorldToCell(position);
        result.z = 0;
        return result;
    }

    public Vector3 GridToWorld(Vector3Int position)
    {
        var result = grid.CellToWorld(position);
        result.z = 0;
        return result;
    }

    public int CalculateDistance(Vector3Int origin, Vector3Int target)
    {
        return Mathf.Max(Mathf.Abs(origin.z - target.z), Mathf.Max(Mathf.Abs(origin.x - target.x), Mathf.Abs(origin.y - target.y)));
    }
}
