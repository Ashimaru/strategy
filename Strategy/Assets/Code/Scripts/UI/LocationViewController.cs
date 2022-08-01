using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using System;

public class LocationViewController : MonoBehaviour
{
    [SerializeField]
    private ArmyViewController armyView;
    [SerializeField]
    private JobsViewController jobsView;

    [SerializeField]
    private TextMeshProUGUI locationName;
    [SerializeField]
    private bool AlwaysLoadAllData;

    private Action _startArmyCreation;
    private Location _currentLocation;
    private MonoBehaviour _activeView;

    public void CreateArmyFromGarrison()
    {
        _startArmyCreation();
    }

    public void LoadLocation(Location location, Action createArmyCallback)
    {
        if (location == null)
        {
            gameObject.SetActive(false);
            _startArmyCreation = null;
            _currentLocation = null;
            return;
        }

        _currentLocation = location;
        gameObject.SetActive(true);
        var locationData = location.LocationData;
        locationName.text = locationData.LocationName;

        if (AlwaysLoadAllData || location.LocationData.alignment == Alignment.Human)
        {
            LoadArmyView(locationData.Garrison, createArmyCallback);            
        }
    }

    private void LoadArmyView(Army army, Action createArmyCallback)
    {
        _activeView = armyView;

        _startArmyCreation = createArmyCallback;
        armyView.ClearArmyInfo();
        armyView.LoadArmyInfo(army);
    }



    private void Start()
    {
        gameObject.SetActive(false);
    }


}
