using System;
using UnityEngine;

[RequireComponent(typeof(ArmyController))]
public class AIArmyController : MonoBehaviour
{
    private ArmyController armyController;
    void Awake()
    {
        armyController = GetComponent<ArmyController>();
        Debug.Assert(armyController != null);
    }

    public void DeliverResourcesToTheCity(int resources, Location cityLocation, Location villageLocation)
    {
        armyController.MoveTo(cityLocation.Position, () => { VisitLocation(cityLocation); });
        armyController.Wait(3F, () => LeaveLocation(cityLocation));
        armyController.MoveTo(villageLocation.Position, () => JoinGarrison(villageLocation));
    }
    private void VisitLocation(Location location)
    {
        location.LocationData.VisitingArmies.Add(armyController.army);
        armyController.HideArmy();
    }

    private void LeaveLocation(Location location)
    {
        location.LocationData.VisitingArmies.Add(armyController.army);
        armyController.HideArmy();
    }

    private void JoinGarrison(Location location)
    {
        location.LocationData.Garrison.AddSoldiers(armyController.army.soldiers);
        armyController.Despawn();
    }

    internal void ExecutePatrol(Location sourceLocation, PatrolPath path, Action OnPatrolCompleted)
    {
        //Debug.Log($"{armyController.army.ArmyName} starts patrol route {path.patrolName}");
        Debug.Assert(OnPatrolCompleted != null);
        foreach(var checkpoint in path.checkpoints)
        {
            armyController.MoveTo(checkpoint.position);
            armyController.Wait(checkpoint.timeToWait);
        }

        armyController.MoveTo(sourceLocation.Position, () =>
        {
            JoinGarrison(sourceLocation);
            OnPatrolCompleted();
        });
     }
}
