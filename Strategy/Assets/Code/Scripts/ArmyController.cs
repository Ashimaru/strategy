using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IOrder
{
    abstract string GetOrderDescription();
    abstract void Cancel();
}

public class MoveOrder: IOrder
{
    Vector3Int originPosition;
    Vector3Int targetPosition;
    Timer movementTimer;
    ArmyController armyToMove;
    Action onOrderDone;


    public MoveOrder(ArmyController armyToMove, Vector3Int targetPosition, Action onOrderDone)
    {
        originPosition = armyToMove.CurrentPosition;
        this.targetPosition = targetPosition;
        this.armyToMove = armyToMove;
        this.onOrderDone = onOrderDone;
        armyToMove.army.CurrentAssigmentDescription = GetOrderDescription();
        movementTimer = Utils.CreateTimer(armyToMove.gameObject, 1f, MoveAndStartTimer);
    }

    void MoveAndStartTimer()
    {
        var moves = Systems.Get<INavigation>().NavigateTowards(armyToMove.CurrentPosition, targetPosition);
        armyToMove.CurrentPosition = moves[0];
        if (moves[0] == targetPosition)
        {
            armyToMove.army.CurrentAssigmentDescription = "Standby";
            onOrderDone();
            return;
        }
        Utils.CreateTimer(armyToMove.gameObject, 1f, MoveAndStartTimer);
    }

    public void Cancel()
    {
        movementTimer.Cancel();
    }

    public string GetOrderDescription()
    {
        return string.Format("Moving from {0} to {1}.", originPosition.ToString(), targetPosition.ToString());
    }
}

public class ArmyController : MonoBehaviour,
                              IFocusableTile,
                              ITileSelectedListener
{
    public Army army;
    public ArmyViewController armyViewController;
    public Tilemap unitTilemap;
    public TileBase armyTile;

    [SerializeField]
    private Vector3Int currentPosition;
    private IOrder currentOrder;

    public Vector3Int CurrentPosition { 
        get { return currentPosition;  }
        set {
            Systems.Get<IClickableTile>().DeregisterClickableTile(currentPosition, this);
            unitTilemap.SetTile(currentPosition, null);
            currentPosition = value;
            gameObject.transform.position = Systems.Get<IGrid>().GridToWorld(value);
            unitTilemap.SetTile(currentPosition, armyTile);
            Systems.Get<IClickableTile>().RegisterClickableTile(currentPosition, TileType.Unit, this);
            Systems.Get<ITileEnterListenerManager>().OnTileEnter(currentPosition, this);
        }
    }

    public void MoveTo(Vector3Int targetPosition)
    {
        if(targetPosition == CurrentPosition)
        {
            return;
        }

        currentOrder = new MoveOrder(this, targetPosition, () => {
            Debug.Log("Movement done");
        });
    }

    public void OnFocusAcquired()
    {
        armyViewController.LoadArmyInfo(army);
        Systems.Get<ITileSelector>().RegisterForTileSelect(this);
    }

    public void OnFocusLost()
    {
        armyViewController.ClearArmyInfo();
        Systems.Get<ITileSelector>().ClearListener();
    }

    public void TileSelected(Vector3Int coordinates)
    {
        MoveTo(coordinates);
    }

    public void Despawn()
    {
        Systems.Get<IClickableTile>().DeregisterClickableTile(currentPosition, this);
        unitTilemap.SetTile(currentPosition, null);
        if (armyViewController.CurrentlyShownArmy == army)
        {
            armyViewController.ClearArmyInfo();
        }
        Destroy(gameObject);
    }
}
