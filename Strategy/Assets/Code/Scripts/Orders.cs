using System;
using System.Collections;
using UnityEngine;

public interface IOrder
{
    abstract void Execute();
    abstract string GetOrderDescription();
    abstract void Cancel();
}

public class MoveOrder : IOrder
{
    Vector3Int originPosition;
    Vector3Int targetPosition;
    Timer movementTimer;
    ArmyController armyToMove;
    Action onOrderDone;

    public MoveOrder(ArmyController armyToMove, Vector3Int targetPosition, Action onOrderDone)
    {
        this.targetPosition = targetPosition;
        this.armyToMove = armyToMove;
        this.onOrderDone = onOrderDone;
    }

    void MoveAndStartTimer()
    {
        var moves = Systems.Get<INavigation>().NavigateTowards(armyToMove.CurrentPosition, targetPosition);
        armyToMove.CurrentPosition = moves[0];
        //Debug.Log($"Current position:{armyToMove.CurrentPosition} target position:{targetPosition}");
        if (armyToMove.CurrentPosition == targetPosition)
        {
            //Debug.Log($"{armyToMove.name}: Done with movement");
            armyToMove.army.CurrentAssigmentDescription = "Standby";
            onOrderDone();
            return;
        }
        movementTimer = Utils.CreateTimer(armyToMove.gameObject, 1f, MoveAndStartTimer, "Next move timer");
    }

    public void Cancel()
    {
        movementTimer.Cancel();
    }

    public string GetOrderDescription()
    {
        return string.Format("Moving from {0} to {1}.", originPosition.ToString(), targetPosition.ToString());
    }

    public void Execute()
    {

        originPosition = armyToMove.CurrentPosition;
        if (targetPosition == originPosition)
        {
            //Debug.Log("Done with movement");
            armyToMove.army.CurrentAssigmentDescription = "Standby";
            onOrderDone();
            return;
        }


        armyToMove.army.CurrentAssigmentDescription = GetOrderDescription();
        movementTimer = Utils.CreateTimer(armyToMove.gameObject, 1f, MoveAndStartTimer, "Next move timer");
    }
}

public class WaitOrder : IOrder
{
    private ArmyController _army;
    private Timer _timer;
    private float _timeToWait;
    private Action _onOrderCompleted;
    public WaitOrder(ArmyController army, float timeToWait, Action onOrderCompleted)
    {
        _army = army;
       _timeToWait = timeToWait;
        _onOrderCompleted = onOrderCompleted;   
    }

    public void Execute()
    {
        _timer = Utils.CreateTimer(_army.gameObject, _timeToWait, OnWaitDone, "Waiting at location");
    }

    public void Cancel()
    {
        _timer.Cancel();
    }

    public string GetOrderDescription()
    {
        return "Waiting in location.";
    }

    private void OnWaitDone()
    {
        //Debug.Log($"{_army.name}: Waiting done");
        _onOrderCompleted();
    }
}

