using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Location), typeof(SelectableLocation))]
public class NecropolyController : MonoBehaviour 
{
    [SerializeField] ArmyCreatorViewController _armyCreatorView;
    [SerializeField] LocationViewController _locationView;

    private Location _location;
    private SelectableLocation _selectableLocation;

    private void Start()
    {
        _location = GetComponent<Location>();
        _selectableLocation = GetComponent<SelectableLocation>();
        _selectableLocation.CreateArmyDelegate = CreateArmyFromGarrison;
    }

    public void CreateArmyFromGarrison()
    {
        var newArmy = ScriptableObject.CreateInstance<Army>();
        newArmy.ArmyName = _location.LocationData.LocationName + "'s Army";
        _armyCreatorView.LoadArmies(_location.LocationData.Garrison, newArmy, OnArmyCreated);
    }

    private void OnArmyCreated(Army newArmy)
    {
        if (newArmy == null)
        {
            return;
        }
        _location.ShouldSkipNextArmyEnter = true;
        Systems.Get<IArmyFactory>().CreateArmy(newArmy, _location.Position);
        _locationView.LoadArmyData(_location.LocationData.Garrison, CreateArmyFromGarrison);
    }


}
