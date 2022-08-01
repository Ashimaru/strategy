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

    private MonoBehaviour _activeView;
    public void LoadLocation(Location location, Action createArmyCallback)
    {
        if (location == null)
        {
            gameObject.SetActive(false);
            _startArmyCreation = null;
            return;
        }

        gameObject.SetActive(true);
        var locationData = location.LocationData;
        if (AlwaysLoadAllData || location.LocationData.alignment == Alignment.Human)
        {
            LoadArmyView
            armyView.LoadArmyInfo(locationData.Garrison);
        }


    }

    public void LoadArmyView( Action createArmyCallback, )
    {
        _activeView = armyView;

        _startArmyCreation = createArmyCallback;


        var locationData = location.LocationData;
        armyView.ClearArmyInfo();
        locationName.text = locationData.LocationName;

    }

    public void LoadJobsView()
    {

    }
    public void CreateArmyFromGarrison()
    {
        _startArmyCreation();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }


}
