using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Location), typeof(JobQueue))]
public class VillageController : MonoBehaviour
{
    private static readonly int CRITICAL_GARRISON_POWER = 20;

    [SerializeField]
    private List<UnitData> _possibleUnits;
    [SerializeField]
    private List<JobData> _possibleJobs;
    [SerializeField]
    private HeatSettings heatSettings;
    [SerializeField]
    private int _resourcesGeneratedPerJob = 20;
    [SerializeField]
    private int _resourcesNeededToSendToCity = 100;


    private RegionData _region;
    private Location _myLocation;
    private JobQueue _jobQueue;
    private int _resources = 0;

    private bool shouldSendResourcesToCity = true;

    private void Start()
    {
        _myLocation = GetComponent<Location>();
        _jobQueue = GetComponent<JobQueue>();
        _region = GetComponentInParent<RegionData>();
        Debug.Assert(_region != null, "Village's parent must be a region");
        DecideNextStep();
    }

    void DecideNextStep()
    {
        if (IsGarrisonCritical())
        {
            FillupGarrison();
            return;
        }

        SendResourcesToCity();
    }

    private void GenerateResources(int resources)
    {
        _resources += resources;
        DecideNextStep();
    }

    private void AddUnitToGarrison(UnitData unit)
    {
        _myLocation.LocationData.Garrison.AddSoldiers(unit, 1);
        DecideNextStep();

    }

    private void StartResourceProductionJob()
    {
        //Debug.Log($"{_myLocation.LocationData.LocationName} is creating resources");
        _jobQueue.AddToQueue(new Job(GetJobData("VillageGenerateResources"), () => { GenerateResources(_resourcesGeneratedPerJob); }));
    }

    private void StartUnitCreationJob(UnitData unitToCreate)
    {
        Debug.Assert(_resources >= unitToCreate.CreationJob.JobCost);
        //Debug.Log($"Creating Unit {unitToCreate.UnitTypeName}");
        _resources -= unitToCreate.CreationJob.JobCost;
        _jobQueue.AddToQueue(new Job(unitToCreate.CreationJob, () => AddUnitToGarrison(unitToCreate)));
    }

    private void SendResourcesToCity()
    {
        if (!shouldSendResourcesToCity)
        {
            StartResourceProductionJob();
            return;
        }

        if (_resources < _resourcesNeededToSendToCity)
        {
            StartResourceProductionJob();
            return;
        }

        if (!HasEnoughGarrisonToSendResourcesToCity())
        {
            FillupGarrison();
            return;
        }

        //Debug.Log("Sending resources to city.");
        var army = CreateDeliveryGroup();


        var armyGo = Systems.Get<IArmyFactory>().CreateAIArmy(army, _myLocation.Position);
        var armyAi = armyGo.GetComponent<AIArmyController>();
        armyAi.DeliverResourcesToTheCity(_resources, _region.Capital.Location, _myLocation);

        shouldSendResourcesToCity = false;
        Utils.CreateTimer(gameObject, 30f, () => { shouldSendResourcesToCity = true; }, "Send resources to city");
        StartResourceProductionJob();
    }

    private Army CreateDeliveryGroup()
    {
        VillageHeatSettings villageHeatSettings = GetHeatSettings();
        var deliveryGroup = Army.CreateGroupFromArmy(_myLocation.LocationData.Garrison,
                                            villageHeatSettings.ResourceGroupRequirements);
        deliveryGroup.ArmyName = _myLocation.LocationData.LocationName + "'s transport";
        deliveryGroup.Aligment = _myLocation.LocationData.alignment;
        return deliveryGroup;
    }

    private JobData GetJobData(string jobName)
    {
        var result = _possibleJobs.Find(job => job.name == jobName);
        Debug.Assert(result != null, $"Failed to find job with name {jobName}. Possible jobs:{_possibleJobs.Stringify()}");

        return result;
    }

    private bool HasEnoughGarrisonToSendResourcesToCity()
    {
        var garrisonRequirements = GetHeatSettings().MinimumGarrisonToSendResources;
        int numberOfSoldiers = _myLocation.LocationData.Garrison.Size;
        if (numberOfSoldiers < garrisonRequirements.Size)
        {
            //Debug.Log($"{_myLocation.LocationData.LocationName}: not enough soldiers to send resources:{numberOfSoldiers}/{requiredNumberOfSoldiersToSendResources}");
            return false;
        }

        int garrisonPower = _myLocation.LocationData.Garrison.Power;

        if (garrisonPower < garrisonRequirements.Power)
        {
            Debug.Log($"{_myLocation.LocationData.LocationName}: garrison is too weak:{garrisonPower}/{garrisonRequirements.Power}");
            return false;
        }

        return true;
    }

    private bool IsGarrisonCritical()
    {
        int power = _myLocation.LocationData.Garrison.Power;
        if (power < CRITICAL_GARRISON_POWER)
        {
            //Debug.Log($"{_myLocation.LocationData.LocationName}: garrison is critical ({power}).");
            return true;
        }
        return false;
    }

    private void FillupGarrison()
    {
        var possibleUnitsToCreate = new List<UnitData>(_possibleUnits);
        possibleUnitsToCreate.RemoveAll(unitData => unitData.CreationJob.JobCost > _resources);

        if (possibleUnitsToCreate.Count == 0)
        {
            //Debug.Log($"Not enough resources({_resources}) to create new unit");
            StartResourceProductionJob();
            return;
        }

        var unitToCreate = possibleUnitsToCreate.RandomElement();
        StartUnitCreationJob(unitToCreate);
        return;
    }

    private VillageHeatSettings GetHeatSettings()
    {
        return heatSettings.GetHeatSettings(_region.HeatManager.GetHeatLevel()).VillageSettings;
    }
}
