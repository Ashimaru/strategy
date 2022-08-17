using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface INavigation
{
    List<Vector3Int> GetNeighbours(Vector3Int position);
    List<Vector3Int> NavigateTowards(Vector3Int currentPosition, Vector3Int targetPosition);
}

internal class TileWalkCost : IEquatable<TileWalkCost>
{
    public readonly Vector3Int tilePosition;

    public TileWalkCost parentNode;
    public int costFromOrigin; //gCost
    public int costToTarget; //hCost
    public int total; //fCost

    public TileWalkCost(TileWalkCost _parentNode, Vector3Int _tilePosition, int _costFromOrigin, int _costToTarget)
    {
        parentNode = _parentNode;
        tilePosition = _tilePosition;
        costFromOrigin = _costFromOrigin;
        costToTarget = _costToTarget;
        total = costFromOrigin + costToTarget;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        TileWalkCost otherTile = obj as TileWalkCost;
        return otherTile != null ? Equals(otherTile) : false; 
    }
    public bool Equals(TileWalkCost other)
    {
        return tilePosition == other.tilePosition;
    }

}

public class Navigation : MonoBehaviour, INavigation
{
    private IGrid worldGrid;
    [SerializeField]
    private Tilemap tilemap;
    private System.Action<Vector3Int, Color> colorTileMethod;

    private Dictionary<Vector3Int, Vector3Int> gridToNavigation = new Dictionary<Vector3Int, Vector3Int>();
    private Dictionary<Vector3Int, Vector3Int> navigationToGrid = new Dictionary<Vector3Int, Vector3Int>();

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
        return AStar(currentPosition, targetPosition);
    }

    public void NavigateTowards(Vector3Int currentPosition, Vector3Int targetPosition, System.Action<Vector3Int, Color> colorTile)
    {
        colorTileMethod = colorTile;
        StartCoroutine(AStarDebug(currentPosition, targetPosition));
    }

    public List<Vector3Int> GetNeighbours(Vector3Int position)
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
    }

    private void Start()
    {
        worldGrid = Systems.Get<IGrid>();

        var xMin = tilemap.cellBounds.xMin;
        var xMax = tilemap.cellBounds.xMax;
        var yMin = tilemap.cellBounds.yMin;
        var yMax = tilemap.cellBounds.yMax;
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

    IEnumerator AStarDebug(Vector3Int from, Vector3Int targetPosition)
    {
        List<Vector3Int> result = new();
        HashSet<Vector3Int> allColoredTiles = new();

        var openPathTiles = new List<TileWalkCost>();
        var closedPathTiles = new List<TileWalkCost>();

        var navFrom = gridToNavigation[from];
        var navTarget = gridToNavigation[targetPosition];

        TileWalkCost currentTile = new TileWalkCost(null, navFrom, 0, worldGrid.CalculateDistance(navFrom, navTarget));

        openPathTiles.Add(currentTile);

        while (openPathTiles.Count != 0)
        {
            openPathTiles = openPathTiles.OrderBy(x => x.total).ThenByDescending(x => x.costFromOrigin).ToList();
            currentTile = openPathTiles[0];

            openPathTiles.Remove(currentTile);
            closedPathTiles.Add(currentTile);

            yield return new WaitForSeconds(0.1f);
            allColoredTiles.Add(currentTile.tilePosition);
            colorTileMethod(navigationToGrid[currentTile.tilePosition], Color.red);

            int g = currentTile.costFromOrigin + 1;

            if (currentTile.tilePosition == navTarget)
            {
                break;
            }

            var neighbours = GetNeighbours(currentTile.tilePosition);
            foreach (var neighbour in neighbours)
            {
                yield return new WaitForSeconds(0.1f);
                TileWalkCost neighbourTile = new TileWalkCost(currentTile, neighbour, g, worldGrid.CalculateDistance(neighbour, navTarget));

                allColoredTiles.Add(neighbour);

                if (closedPathTiles.Contains(neighbourTile))
                {
                    continue;
                }
                colorTileMethod(navigationToGrid[neighbour], Color.green);

                if (!openPathTiles.Contains(neighbourTile))
                {
                    openPathTiles.Add(neighbourTile);
                }
                else if(neighbourTile.total > g + neighbourTile.costToTarget)
                {
                    neighbourTile.costFromOrigin = g;
                }
            }
        }
        yield return new WaitForSeconds(1f);

        List<TileWalkCost> finalPathTiles = new List<TileWalkCost>();

        var targetTiles = closedPathTiles.Where(x => x.tilePosition == navTarget).ToList();

        if(targetTiles.Count == 0)
        {
            yield break;
        }

        currentTile = targetTiles[0];
        colorTileMethod(navigationToGrid[currentTile.tilePosition], Color.blue);

        finalPathTiles.Add(currentTile);

        for (int i = currentTile.costFromOrigin - 1; i >= 0; i--)
        {
            var neighbours = GetNeighbours(currentTile.tilePosition);
            currentTile = closedPathTiles.Find(x => x.costFromOrigin == i && neighbours.Contains(x.tilePosition));
            finalPathTiles.Add(currentTile);
            colorTileMethod(navigationToGrid[currentTile.tilePosition], Color.blue);
            yield return new WaitForSeconds(0.1f);
        }

        finalPathTiles.Reverse();

        yield return new WaitForSeconds(1f);

        foreach (var tile in allColoredTiles)
        {
            colorTileMethod(tile, Color.white);
        }
    }

    private List<Vector3Int> AStar(Vector3Int from, Vector3Int targetPosition)
    {
        List<Vector3Int> result = new();

        var openPathTiles = new List<TileWalkCost>();
        var closedPathTiles = new List<TileWalkCost>();

        var navFrom = gridToNavigation[from];
        var navTarget = gridToNavigation[targetPosition];

        TileWalkCost currentTile = new TileWalkCost(null, navFrom, 0, worldGrid.CalculateDistance(navFrom, navTarget));

        openPathTiles.Add(currentTile);

        while (openPathTiles.Count != 0)
        {
            openPathTiles = openPathTiles.OrderBy(x => x.total).ThenByDescending(x => x.costFromOrigin).ToList();
            currentTile = openPathTiles[0];

            openPathTiles.Remove(currentTile);
            closedPathTiles.Add(currentTile);

            int g = currentTile.costFromOrigin + 1;

            if (currentTile.tilePosition == navTarget)
            {
                break;
            }

            var neighbours = GetNeighbours(currentTile.tilePosition);
            foreach (var neighbour in neighbours)
            {
                TileWalkCost neighbourTile = new TileWalkCost(currentTile, neighbour, g, worldGrid.CalculateDistance(neighbour, navTarget));

                if (closedPathTiles.Contains(neighbourTile))
                {
                    continue;
                }

                if (!openPathTiles.Contains(neighbourTile))
                {
                    openPathTiles.Add(neighbourTile);
                }
                else if (neighbourTile.total > g + neighbourTile.costToTarget)
                {
                    neighbourTile.costFromOrigin = g;
                }
            }
        }
        List<Vector3Int> finalPathTiles = new();

        var targetTiles = closedPathTiles.Where(x => x.tilePosition == navTarget).ToList();

        if (targetTiles.Count == 0)
        {
            return finalPathTiles;
        }

        currentTile = targetTiles[0];
        finalPathTiles.Add(currentTile.tilePosition);

        for (int i = currentTile.costFromOrigin - 1; i >= 0; i--)
        {
            var neighbours = GetNeighbours(currentTile.tilePosition);
            currentTile = closedPathTiles.Find(x => x.costFromOrigin == i && neighbours.Contains(x.tilePosition));
            finalPathTiles.Add(navigationToGrid[currentTile.tilePosition]);
        }

        finalPathTiles.Reverse();
        return finalPathTiles;
    }

}
