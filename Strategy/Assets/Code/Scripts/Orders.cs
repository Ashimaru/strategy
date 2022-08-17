using System;
using System.Collections;
using UnityEngine;

public interface IOrder
{
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
        Debug.Log($"Current position:{armyToMove.CurrentPosition} target position:{targetPosition}");
        if (armyToMove.CurrentPosition == targetPosition)
        {
            Debug.Log("Done with movement");
            armyToMove.army.CurrentAssigmentDescription = "Standby";
            onOrderDone();
            return;
        }
        movementTimer = Utils.CreateTimer(armyToMove.gameObject, 1f, MoveAndStartTimer);
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

public class WaitOrder : IOrder
{
    private ArmyController _army;
    private Timer _timer;
    public WaitOrder(ArmyController army, float timeToWait, Action onOrderCompleted)
    {
        _army = army;
        _timer = Utils.CreateTimer(army.gameObject, timeToWait, onOrderCompleted);
    }

    public void Cancel()
    {
        _timer.Cancel();
    }

    public string GetOrderDescription()
    {
        return "Waiting in location.";
    }
}
