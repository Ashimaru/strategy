using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    public List<IOrder> OrderQueue { get; private set; } = new();

    public Vector3Int CurrentPosition
    {
        get { return currentPosition; }
    }

    void Start()
    {
        ChangePositionTo(Systems.Get<IGrid>().WorldToGrid(transform.position));
        Systems.Get<IRepository<ArmyController>>().Add(this);
    }

    public bool ChangePositionTo(Vector3Int newLocation)
    {
        if(currentPosition == newLocation)
        {
            Debug.Log($"{army.ArmyName} cannot move to {newLocation} - its already there.");
            return false;
        }

        var armiesAtPosition = Systems.Get<IRepository<ArmyController>>().Find(army => army.CurrentPosition == newLocation);
        if (armiesAtPosition.Count != 0)
        {
            var defendingArmy = armiesAtPosition[0];
            Utils.StartBattle(this, defendingArmy, newLocation, () => { });
            return false;
        }

        Systems.Get<IClickableTile>().DeregisterClickableTile(currentPosition, this);
        unitTilemap.SetTile(currentPosition, null);
        currentPosition = newLocation;
        gameObject.transform.position = Systems.Get<IGrid>().GridToWorld(newLocation);
        unitTilemap.SetTile(currentPosition, armyTile);
        Systems.Get<IClickableTile>().RegisterClickableTile(currentPosition, TileType.Unit, this);
        Systems.Get<ITileEnterListenerManager>().OnTileEnter(currentPosition, this);
        return true;
    }

    public void MoveTo(Vector3Int targetPosition)
    {
        MoveTo(targetPosition, () =>
        {
        });
    }

    public void MoveTo(Vector3Int targetPosition, Action onMoveDone)
    {
        AddOrder(new MoveOrder(this, targetPosition, BuildOrderDoneCallback(onMoveDone)));
    }

    public void Wait(float waitingTime)
    {
        Wait(waitingTime, () => { });
    }

    public void Wait(float waitingTime, Action onWaitDone)
    {
        AddOrder(new WaitOrder(this, waitingTime, BuildOrderDoneCallback(onWaitDone)));
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
        Debug.Log($"Despawning {army.ArmyName}");
        HideArmy();
        Systems.Get<IRepository<ArmyController>>().Remove(this);
        Destroy(gameObject);
    }

    public void HideArmy()
    {
        Systems.Get<IClickableTile>().DeregisterClickableTile(currentPosition, this);
        unitTilemap.SetTile(currentPosition, null);
        if (armyViewController.CurrentlyShownArmy == army)
        {
            armyViewController.ClearArmyInfo();
        }
    }

    public void StopCurrentOrder()
    {
        if (currentOrder != null)
        {
            currentOrder.Cancel();
        }
    }

    public void ResumeCurrentOrder()
    {
        if (currentOrder != null)
        {
            currentOrder.Execute();
        }
    }

    public void RestoreArmy()
    {
        Systems.Get<IClickableTile>().RegisterClickableTile(currentPosition, TileType.Unit, this);
        unitTilemap.SetTile(currentPosition, armyTile);
    }

    private void AddOrder(IOrder order)
    {
        if (currentOrder == null)
        {
            //Debug.Log("Starting action since queue is empty");
            currentOrder = order;
            currentOrder.Execute();
            return;
        }
        //Debug.Log("Queueing action since queue is not empty");
        OrderQueue.Add(order);
    }

    private void CompleteCurrentOrder()
    {
        currentOrder = null;
    }

    private void StartNextOrder()
    {
        if (OrderQueue.Count == 0)
        {
            //Debug.Log($"{army.ArmyName}: All orders complete nothing to do");
            return;
        }

        currentOrder = OrderQueue[0];
        OrderQueue.RemoveAt(0);
        currentOrder.Execute();
    }

    private Action BuildOrderDoneCallback(Action originalCallback)
    {
        return () =>
        {
            CompleteCurrentOrder();
            originalCallback();
            StartNextOrder();
        };
    }
}
