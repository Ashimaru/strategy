using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGlobalHeatManager
{
    void IncreaseHeat();
}

public class GlobalHeatManager : MonoBehaviour, IGlobalHeatManager
{
    static readonly int MAX_HEAT = 5;
    int _currentHeat = 0;

    void Awake()
    {
        Systems.RegisterSystem<IGlobalHeatManager>(this);
    }

    private void OnDestroy()
    {
        Systems.DeregisterSystem<IGlobalHeatManager>(this);
    }

    public void IncreaseHeat()
    {
        if (_currentHeat >= MAX_HEAT)
        {
            return;
        }

        ++_currentHeat;
        if (_currentHeat == MAX_HEAT)
        {
            Debug.Log("MAX HEAT REACHED - THIS MEANS WAR.");
            StartWar();
            return;
        }

        Debug.Log($"Global heat got increased to {_currentHeat}/{MAX_HEAT}");
    }

    private void StartWar()
    {
        Debug.Log("WAR NOT IMPLEMENTED YET");
    }
}
