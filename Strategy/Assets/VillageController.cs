using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Location))]
public class VillageController : MonoBehaviour
{
    public Location ParentCity;
    [SerializeField]
    private List<UnitData> _possibleUnits;

    private Timer _resourceProductionTimer;
    private Location _myLocation;
    private int _resources = 0;

    private void Start()
    {
        _myLocation = GetComponent<Location>();
        _resourceProductionTimer = Utils.CreateRepeatingTimer(gameObject, 5F, GenerateResources);
    }

    private void GenerateResources()
    {
        _resources += 10;

        if(_resources > 20)
        {
            SendResourcesToCity();
            _resources = 0;
        }

    }


    private void SendResourcesToCity()
    {
        Debug.Log("Sending resources to city.");

        _myLocation.ShouldSkipNextArmyEnter = true;

        var army = ScriptableObject.CreateInstance<Army>();
        army.ArmyName = _myLocation.LocationData.LocationName + "'s transport.";
        army.Aligment = _myLocation.LocationData.alignment;
        army.AddSoldiers(new List<SoldierGroup>() {
           new SoldierGroup{
               NumberOfMembers = Random.Range(5, 10),
               unitData = _possibleUnits[0]
        }});


        var armyGo = Systems.Get<IArmyFactory>().CreateArmy(army, _myLocation.Position);
        var armyController = armyGo.GetComponent<ArmyController>();
        armyController.MoveTo(ParentCity.Position, () => armyController.Wait(3F, () => armyController.MoveTo(_myLocation.Position)));

    }
}
