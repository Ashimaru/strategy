using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface INavigation
{
    List<Vector3Int> NavigateTowards(Vector3Int currentPosition, Vector3Int targetPosition);
}

internal class PathNode : IEquatable<PathNode>
{
    public readonly Vector3Int position;

    public PathNode parentNode;
    public float gCost;
    public float hCost;
    public float fCost;

    public PathNode(PathNode _parentNode, Vector3Int _position, float _gCost, float _hCost)
    {
        parentNode = _parentNode;
        position = _position;
        gCost = _gCost;
        hCost = _hCost;
        fCost = gCost + hCost;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        PathNode otherTile = obj as PathNode;
        return otherTile != null ? Equals(otherTile) : false; 
    }

    public override int GetHashCode()
    {
        return position.GetHashCode();
    }

    public bool Equals(PathNode other)
    {
        return position == other.position;
    }

}
public class Navigation : MonoBehaviour, INavigation
{
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private Tilemap baseTilemap;
    [SerializeField]
    private Tilemap roads;
    [SerializeField]
    private List<TileProperties> tileDatas;

    private Dictionary<TileBase, TileProperties> dataFromTiles = new();

    private Dictionary<Vector3Int, Vector3Int> gridToNavigation = new Dictionary<Vector3Int, Vector3Int>();
    private Dictionary<Vector3Int, Vector3Int> navigationToGrid = new Dictionary<Vector3Int, Vector3Int>();


    private List<Vector3Int> coloredTiles = new();
    private List<TextMesh> textMeshes = new();

    readonly private List<Vector3Int> POSSIBLE_MOVES = new()
    {
        new Vector3Int(1, 0, -1),
        new Vector3Int(1, -1, 0),
        new Vector3Int(0, -1, 1),
        new Vector3Int(-1, 0, 1),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(0, 1, -1)
    };


    private readonly Vector3Int YZ_INCREMENT = new Vector3Int(0, 1, -1);
    private readonly Vector3Int XZ_INCREMENT = new Vector3Int(1, 0, -1);
    private readonly Vector3Int XY_INCREMENT = new Vector3Int(1, -1, 0);

    public List<Vector3Int> NavigateTowards(Vector3Int currentPosition, Vector3Int targetPosition)
    {
        Debug.Assert(currentPosition != targetPosition);
        return AStar(currentPosition, targetPosition);
    }

    private List<Vector3Int> GetNeighbours(Vector3Int position)
    {
        var possibleMoves = new List<Vector3Int>(POSSIBLE_MOVES);
        for (var index = 0; index < possibleMoves.Count; ++index)
        {
            possibleMoves[index] += position;
        }
        return possibleMoves;
    }

    private void Awake()
    {
        Systems.RegisterSystem<INavigation>(this);

        foreach (var tileProperties in tileDatas)
        {
            foreach (var tile in tileProperties.tiles)
            {
                dataFromTiles.Add(tile, tileProperties);
            }
        }
    }

    private void Start()
    {
        var xMin = baseTilemap.cellBounds.xMin;
        var xMax = baseTilemap.cellBounds.xMax;
        var yMin = baseTilemap.cellBounds.yMin;
        var yMax = baseTilemap.cellBounds.yMax;
        Vector3Int xNumerationOffset = Vector3Int.zero;
        for (int x = xMin; x < xMax; ++x)
        {
            var yNumerationOffset = Vector3Int.zero;
            for (int y = yMin; y < yMax; ++y)
            {
                gridToNavigation[new Vector3Int(x, y, 0)] = (xNumerationOffset + yNumerationOffset);
                yNumerationOffset += y % 2 != 0 ? XY_INCREMENT : XZ_INCREMENT;
            }
            xNumerationOffset += YZ_INCREMENT;
        }

        navigationToGrid = gridToNavigation.Reverse();
    }

    private void OnDrawGizmos()
    {
        Vector3 size = new(0.2F, 0.2F, 0.2F);
        foreach(var entry in gridToNavigation.Keys)
        {
            Gizmos.DrawCube(grid.CellToWorld(entry), size);
        }
    }

    private List<Vector3Int> AStar(Vector3Int from, Vector3Int targetPosition)
    {
        List<Vector3Int> result = new();

        var openPathTiles = new List<PathNode>();
        var closedPathTiles = new List<PathNode>();

        var navFrom = gridToNavigation[from];
        var navTarget = gridToNavigation[targetPosition];

        PathNode currentTile = new PathNode(null, navFrom, 0, CalculateDistance(navFrom, navTarget));

        openPathTiles.Add(currentTile);

        while (openPathTiles.Count != 0)
        {
            openPathTiles = openPathTiles.OrderBy(x => x.fCost).ThenBy(x => x.gCost).ToList();
            currentTile = openPathTiles[0];

            openPathTiles.Remove(currentTile);
            closedPathTiles.Add(currentTile);

            if (currentTile.position == navTarget)
            {
                break;
            }

            var neighbours = GetNeighbours(currentTile.position);
            foreach (var neighbour in neighbours)
            {
                var walkingCost = GetTileWalkingModifier(neighbour);
                // Skip when out of the grid or tile is not walkable
                if (walkingCost == null)
                {
                    continue;
                }

                float g = currentTile.gCost + walkingCost.Value;
                PathNode neighbourTile = new PathNode(currentTile, neighbour, g, CalculateDistance(neighbour, navTarget));

                if (closedPathTiles.Contains(neighbourTile))
                {
                    continue;
                }

                if (!openPathTiles.Contains(neighbourTile))
                {
                    openPathTiles.Add(neighbourTile);
                }
                else if (neighbourTile.fCost > g + neighbourTile.hCost)
                {
                    neighbourTile.gCost = g;
                }
            }
        }
        List<Vector3Int> finalPathTiles = new();

        currentTile = closedPathTiles.First(x => x.position == navTarget);

        return RetracePath(currentTile);
    }
    private List<Vector3Int> RetracePath(PathNode currentTile)
    {
        var result = new List<Vector3Int>();
        if(currentTile == null)
        {
            return result;
        }
        do
        {
            result.Add(navigationToGrid[currentTile.position]);
            currentTile = currentTile.parentNode;
        } while (currentTile.parentNode != null);

        result.Reverse();
        return result;
    }

    private float? GetTileWalkingModifier(Vector3Int position)
    {
        var gridPosition = navigationToGrid[position];
        var roadTile = roads.GetTile(gridPosition);
        if (roadTile != null)
        {
            var roadProperties = dataFromTiles[roadTile];
            return roadProperties.walkingCost;
        }
        var tileBase = baseTilemap.GetTile(gridPosition);
        if (tileBase == null)
        {
            return null;
        }
        var tileProperties = dataFromTiles[tileBase];
        if (!tileProperties.isWalkable)
        {
            return null;
        }
        return tileProperties.walkingCost;
    }

    private int CalculateDistance(Vector3Int origin, Vector3Int target)
    {
        return Mathf.Max(Mathf.Abs(origin.z - target.z), Mathf.Max(Mathf.Abs(origin.x - target.x), Mathf.Abs(origin.y - target.y)));
    }


    //-------------- Left for debug purposes ------------------//
    void ClearTiles()
    {
        foreach(var tile in coloredTiles)
        {
            baseTilemap.SetTileFlags(tile, TileFlags.None);
            baseTilemap.SetColor(tile, Color.white);
        }
    }

    void ColorTile(Vector3Int coords, Color color)
    {
        baseTilemap.SetTileFlags(coords, TileFlags.None);
        baseTilemap.SetColor(coords, color);
        coloredTiles.Add(coords);
    }
}
