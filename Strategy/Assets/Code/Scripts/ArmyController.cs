using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ArmyController : MonoBehaviour,
                              IFocusableTile,
                              ITileSelectedListener
{
    public Army army;
    public ArmyViewController armyViewController;
    public ArmyOrderSelector orderSelector;
    public Tilemap unitTilemap;
    public TileBase armyTile;

    public delegate void OnArmyDestroyedInBattle();
    public OnArmyDestroyedInBattle OnArmyDestroyedInBattleCallback { get; set; } = delegate { };

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
        if (currentPosition == newLocation)
        {
            //Debug.Log($"{army.ArmyName} cannot move to {newLocation} - its already there.");
            return false;
        }

        var armiesAtPosition = Systems.Get<IRepository<ArmyController>>().Find(army => army.CurrentPosition == newLocation);
        var armyToFight = SelectArmyToFight(armiesAtPosition);
        if (armyToFight != null)
        {
            Utils.StartBattle(this, armyToFight, newLocation, () => { });
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

    #region Orders
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

    public void JoinGarrison(Location location)
    {
        JoinGarrison(location, () => { });
    }

    public void JoinGarrison(Location location, Action onGarrionJoined)
    {
        AddOrder(new JoinGarrisonOrder(location, this, onGarrionJoined));
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
        //Debug.Log($"Selected tile {coordinates}");

        //var worldCoord = Systems.Get<IGrid>().GridToWorld(coordinates);
        //List<Operation> possibleOps = new()
        //{
        //    new Operation("Move To", () => MoveTo(coordinates)),
        //    new Operation("Other", () => Debug.Log("Hello!"))
        //};

        //orderSelector.ShowOptions(possibleOps, worldCoord);

        MoveTo(coordinates);
    }

    public void DestroyAfterBattle()
    {
        OnArmyDestroyedInBattleCallback.Invoke();
        Despawn();
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


    #endregion

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
    private ArmyController SelectArmyToFight(List<ArmyController> armiesAtTile)
    {
        armiesAtTile.RemoveAll(army => army.army.Aligment == this.army.Aligment);
        return armiesAtTile.Count != 0 ? armiesAtTile[0] : null;
    }
}
