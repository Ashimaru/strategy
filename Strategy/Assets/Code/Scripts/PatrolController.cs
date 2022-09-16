using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
struct PathCheckpoint
{
    public Vector3Int position;
    public float timeToWait;
}

[Serializable]
struct PatrolPath
{
    public string patrolName;
    public List<PathCheckpoint> checkpoints;
}

[RequireComponent(typeof(Location))]
public class PatrolController : MonoBehaviour
{
    private static readonly int minimumPatrolSize = 5;
    private static readonly int minimumPatrolPower = 20;

    private Location _myLocation;
    [SerializeField]
    private List<PatrolPath> _possiblePatrols;
    [SerializeField]
    private float _timeForNextPatrolToBeSent = 10f;
    [SerializeField]
    private RegionHeatManager _regionHeat;
    [SerializeField]
    private HeatSettings _heatSettings;

    private Timer _nextPatrolTimer;
    private Timer _waitForPatrolToComeback;

    private ArmyController _currentPatrol;

    // Start is called before the first frame update
    void Start()
    {
        _myLocation = GetComponent<Location>();
        RestartSendingPatrolTimer();
    }

    void SendPatrol()
    {
        if(_currentPatrol != null)
        {
            return;
        }

        if(!CanCreatePatrol())
        {
            Debug.Log($"{_myLocation.LocationData.LocationName}: cannot create patrol - Power:{_myLocation.LocationData.Garrison.Power}/{minimumPatrolPower} Size:{_myLocation.LocationData.Garrison.Size}/{minimumPatrolSize}");
            return;
        }
        _nextPatrolTimer.Cancel();
        _nextPatrolTimer = null;
        var patrol = Army.CreateGroupFromArmy(_myLocation.LocationData.Garrison,
                                              _heatSettings.GetHeatSettings(_regionHeat.GetHeatLevel()).VillageSettings.PatrolRequirements);
        patrol.ArmyName = $"{_myLocation.LocationData.LocationName}'s patrol";
        var armyGo = Systems.Get<IArmyFactory>().CreateAIArmy(patrol, _myLocation.Position);
        _currentPatrol = armyGo.GetComponent<ArmyController>();
        _currentPatrol.OnArmyDestroyedInBattleCallback += OnPatrolDestroyed;

        var armyAi = armyGo.GetComponent<AIArmyController>();
        armyAi.ExecutePatrol(_myLocation, _possiblePatrols.RandomElement(), RestartSendingPatrolTimer);
    }

    private void RestartSendingPatrolTimer()
    {
        _nextPatrolTimer = Utils.CreateRepeatingTimer(gameObject, _timeForNextPatrolToBeSent, SendPatrol, "Patrol sending timer");
    }

    private bool CanCreatePatrol()
    {
        var garr = _myLocation.LocationData.Garrison;
        var garrSize = garr.Size;
        if (garrSize < minimumPatrolSize)
        {
            Debug.Log($"{_myLocation.LocationData.LocationName} cannot send patrol - not enough garrison power: {garrSize}/{minimumPatrolSize}");
            return false;
        }

        var garrPower = garr.Power;
        if (garrPower < minimumPatrolPower)
        {
            Debug.Log($"{_myLocation.LocationData.LocationName} cannot send patrol - not enough garrison power: {garrPower}/{minimumPatrolPower}");
            return false;
        }
        return true;
    }

    private void OnPatrolDestroyed()
    {
        Debug.Log($"{_currentPatrol.army.ArmyName} got destroyed");
        _waitForPatrolToComeback = Utils.CreateTimer(gameObject, 30F, () =>
        {
            _regionHeat.IncreaseHeat(IncreaseAmount.Small);
            Debug.Assert(_nextPatrolTimer == null);
            RestartSendingPatrolTimer();
        },
        $"Raise Heat after {_currentPatrol.army.ArmyName} got destroyed");
        _currentPatrol = null;
    }
}
