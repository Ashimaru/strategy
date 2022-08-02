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

    public void ShowArmyView()
    {
        if(_activeView != null)
        {
            _activeView.gameObject.SetActive(false);
        }

        armyView.gameObject.SetActive(true);
        _activeView = armyView; 
    }
    public void ShowJobsView()
    {
        if (_activeView != null)
        {
            _activeView.gameObject.SetActive(false);
        }

        jobsView.gameObject.SetActive(true);
        _activeView=jobsView;
    }


    public void CreateArmyFromGarrison()
    {
        _startArmyCreation();
    }

    public void LoadLocation(Location location, Action createArmyCallback, JobQueue jobQueue)
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

        if (AlwaysLoadAllData || location.LocationData.alignment == Alignment.Necro)
        {
            LoadArmyData(locationData.Garrison, createArmyCallback);
            LoadJobsData(jobQueue);
        }

        ShowArmyView();
    }

    public void LoadArmyData(Army army, Action createArmyCallback)
    {
        _startArmyCreation = createArmyCallback;
        armyView.ClearArmyInfo();
        armyView.LoadArmyInfo(army);
    }

    private void LoadJobsData(JobQueue jobQueue)
    {
        jobsView.LoadQueue(jobQueue);
        
    }



    private void Start()
    {
        gameObject.SetActive(false);
    }


}
