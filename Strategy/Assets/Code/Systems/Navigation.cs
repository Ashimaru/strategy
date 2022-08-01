using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface INavigation
{
    List<Vector3Int> GetNeighbours(Vector3Int position);
    List<Vector3Int> NavigateTowards(Vector3Int currentPosition, Vector3Int targetPosition);
}

internal class TileWalkCost
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
}

public class Navigation : MonoBehaviour, INavigation
{
    private IGrid worldGrid;
    [SerializeField]
    private Tilemap tilemap;

    readonly private List<Vector3Int> POSSIBLE_MOVES_ON_ODD_COLUMN = new()
    {
        new Vector3Int(+1, +0, 0),
        new Vector3Int(+1, +1, 0),
        new Vector3Int(+0, +1, 0),
        new Vector3Int(-1, +0, 0),
        new Vector3Int(+0, -1, 0),
        new Vector3Int(+1, -1, 0)
    };
    readonly private List<Vector3Int> POSSIBLE_MOVES_ON_EVEN_COLUMN = new()
    {
        new Vector3Int(+1, +0, 0),
        new Vector3Int(+0, +1, 0),
        new Vector3Int(-1, +1, 0),
        new Vector3Int(-1, +0, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(+0, -1, 0)
    };
    public List<Vector3Int> NavigateTowards(Vector3Int currentPosition, Vector3Int targetPosition)
    {
        //Debug.Log("Navigating from " + currentPosition + " to " + targetPosition + " distance=" + worldGrid.CalculateDistance(currentPosition, targetPosition));
        //List<Vector3Int> result = new();
        //var nextStep = CalculateNexStep(currentPosition, targetPosition);
        //while (nextStep.HasValue)
        //{
        //    result.Add(nextStep.Value);
        //    nextStep = CalculateNexStep(nextStep.Value, targetPosition);
        //}
     
        return AStar(currentPosition, targetPosition);
    }

    public List<Vector3Int> GetNeighbours(Vector3Int position)
    {

        var possibleMoves = position.y % 2 == 0 ? new List<Vector3Int>(POSSIBLE_MOVES_ON_EVEN_COLUMN) : new List<Vector3Int>(POSSIBLE_MOVES_ON_ODD_COLUMN);
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
    }

    private List<Vector3Int> AStar(Vector3Int from, Vector3Int targetPosition)
    {
        List<Vector3Int> result = new();

        var tilesToCheck = new List<TileWalkCost>();
        var alreadyCheckTiles = new HashSet<TileWalkCost>();

        tilesToCheck.Add(new TileWalkCost(null, from, 0, worldGrid.CalculateDistance(from, targetPosition)));

        while(tilesToCheck.Count != 0)
        {
            var currentTile = tilesToCheck[0];
            foreach (var tile in tilesToCheck)
            {
                if (tile.total < currentTile.total)
                    currentTile = tile;
            }
            tilesToCheck.Remove(currentTile);

            alreadyCheckTiles.Add(currentTile);

            if(currentTile.tilePosition == targetPosition)
            {
                return RetracePath(currentTile);
            }

            foreach (var neighbour in GetNeighbours(currentTile.tilePosition))
            {
                if(alreadyCheckTiles.Where(x => x.tilePosition == neighbour).Count() > 0)
                {
                    continue;
                }

                var costToNeighbour = worldGrid.CalculateDistance(currentTile.tilePosition, neighbour);
                var elementToCheck = tilesToCheck.FirstOrDefault(x => x.tilePosition == neighbour);
                if(elementToCheck == null)
                {
                    elementToCheck = new TileWalkCost(currentTile, neighbour, costToNeighbour, worldGrid.CalculateDistance(neighbour, targetPosition));
                    tilesToCheck.Add(elementToCheck);
                    continue;
                }

                if(elementToCheck.costFromOrigin > costToNeighbour)
                {
                    elementToCheck.costFromOrigin = costToNeighbour;
                    elementToCheck.total = costToNeighbour + elementToCheck.costToTarget;
                    elementToCheck.parentNode = currentTile;
                }
            }

        }

        return result;
    }

    private List<Vector3Int> RetracePath(TileWalkCost currentTile)
    {
        var result = new List<Vector3Int>();
        do
        {
            result.Add(currentTile.tilePosition);
            currentTile = currentTile.parentNode;
        }
        while (currentTile.parentNode != null);
        result.Reverse();
        return result;
    }


    private Tile GetTile(Vector3Int position)
    {
        return tilemap.GetTile<Tile>(position);
    }

    private Vector3Int? CalculateNexStep(Vector3Int currentPosition, Vector3Int targetPosition)
    {
        if (currentPosition == targetPosition)
        {
            return null;
        }

        var possibleMoves = GetNeighbours(currentPosition);

        var movesWithDistance = possibleMoves
            .Select(move => new { Move = move, Distance = worldGrid.CalculateDistance(move, targetPosition) })
            .OrderBy(x => x.Distance);

        var move = movesWithDistance.ElementAt(0);
        //Debug.Log("Selected " + move.Move + "with distance " + move.Distance + " from possible moves:\n" + possibleMoves.Stringify());
        return move.Move;
    }


}
