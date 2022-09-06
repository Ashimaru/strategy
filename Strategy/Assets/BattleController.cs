using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

class BattleEntry
{
    public BattleEntry()
    {

    }
    public BattleEntry(string armyName)
    {
        ArmyName = armyName;
    }

    public string ArmyName { get; set; }
    public int TotalDamage { get; set; }

    public Dictionary<string, int> EnemyLosses { get; set; } = new();

    public void AddDeadUnits(string unitName, int numberOfDeadUnits)
    {
        if (EnemyLosses.ContainsKey(unitName))
        {
            EnemyLosses[unitName] += numberOfDeadUnits;
            return;
        }
        EnemyLosses.Add(unitName, numberOfDeadUnits);
    }
}

class BattleHistory
{
    public List<BattleEntry> Entries { get; set; } = new();
}

class BattleGroup
{
    public BattleGroup(SoldierGroup group)
    {
        RemainingHealth = group.unitData.MaxHP;
        Group = group;
    }
    public int RemainingHealth { get; set; }
    public SoldierGroup Group { get; private set; }
}

class ArmyData
{
    public ArmyData(Army army)
    {
        Army = army;
        foreach (var armyGroup in army.soldiers)
        {
            Groups.Add(new BattleGroup(armyGroup));
        }
    }

    public List<BattleGroup> Groups = new();
    public Army Army { get; private set; }

    public int AttackTurn { get; set; } = 0;
}

public class BattleController : MonoBehaviour
{
    //First arg is winning army, second is losing army
    public Action<ArmyController, ArmyController> OnBattleFinished;
    public Tilemap UnitsTilemap;
    public Vector3Int Position;

    private ArmyController _attackingArmy;
    private ArmyController _defendingArmy;

    private Timer _attackingArmyAttackTimer;
    private Timer _defendingArmyAttackTimer;

    private ArmyData _attackingArmyData;
    private ArmyData _defendingArmyData;

    private bool _isBattleFinished = false;

    private IGrid _grid;

    [SerializeField]
    private Army _testAttackingArmy;
    [SerializeField]
    private Army _testDefendingArmy;
    [SerializeField]
    private TileBase _battleTile;
    [SerializeField]
    private TileBase _battlefieldTile;

    private BattleHistory _history = new();


    private void Awake()
    {
        _grid = Systems.Get<IGrid>();
    }

    void Start()
    {
        //if(_attackingArmyData != null)
        //{
        //    return;
        //}

        //var attackingArmyCopy = Instantiate(_testAttackingArmy);
        //var defendingArmyCopy = Instantiate(_testDefendingArmy);

        //_attackingArmyData = new ArmyData(attackingArmyCopy);
        //_defendingArmyData = new ArmyData(defendingArmyCopy);

        //StartBattle();
    }

    public void StartBattle(ArmyController attackingArmy, ArmyController defendingArmy)
    {
        _attackingArmy = attackingArmy;
        _defendingArmy = defendingArmy;

        _attackingArmyData = new ArmyData(_attackingArmy.army);
        _defendingArmyData = new ArmyData(_defendingArmy.army);

        StartBattle();

    }

    private void StartBattle()
    {
        UnitsTilemap.SetTile(Position, _battleTile);

        Attack(_attackingArmyData, _defendingArmyData);
        if (_isBattleFinished)
        {
            return;
        }
        Attack(_defendingArmyData, _attackingArmyData);

        _attackingArmyAttackTimer = Utils.CreateRepeatingTimer(gameObject, 1, () => Attack(_attackingArmyData, _defendingArmyData), $"{_attackingArmyData.Army.ArmyName} attack timer");
        _defendingArmyAttackTimer = Utils.CreateRepeatingTimer(gameObject, 1.1F, () => Attack(_defendingArmyData, _attackingArmyData), $"{_defendingArmyData.Army.ArmyName} attack timer");
    }

    private void Attack(ArmyData attackingArmy, ArmyData defendingArmy)
    {
        if(_isBattleFinished)
        {
            //Debug.Log("Battle is already completed.");
            return;
        }

        BattleEntry battleEntry = new(attackingArmy.Army.ArmyName);
        attackingArmy.AttackTurn += 1;
        Debug.Log($"{attackingArmy.Army.ArmyName} attacks {defendingArmy.Army.ArmyName} for {attackingArmy.AttackTurn} time.");
        AttackWithMeleeUnits(attackingArmy, defendingArmy, battleEntry);

        if (!_isBattleFinished)
        {
            AttackWithRangedUnits(attackingArmy, defendingArmy, battleEntry);
        }
        _history.Entries.Add(battleEntry);
    }

    private void AttackWithMeleeUnits(ArmyData attackingArmy, ArmyData defendingArmy, BattleEntry battleEntry)
    {
        //Debug.Log($"{attackingArmy.Army.ArmyName} attacks {defendingArmy.Army.ArmyName} with melee.");
        var meeleGroupsInAttackingArmy = GetGroupsOfType(UnitType.Melee, attackingArmy.Groups);
        var meeleGroupsInDefendingArmy = GetGroupsOfType(UnitType.Melee, defendingArmy.Groups);
        var rangedGroupsInDefendingArmy = GetGroupsOfType(UnitType.Ranged, defendingArmy.Groups);

        var numberOfMeeleUnitsInAttackingArmy = meeleGroupsInAttackingArmy.Sum(group => group.Group.NumberOfMembers);
        var numberOfMeeleUnitsInDefendingArmy = meeleGroupsInDefendingArmy.Sum(group => group.Group.NumberOfMembers);
        var numberOfRangedUnitsInDefendingArmy = rangedGroupsInDefendingArmy.Sum(group => group.Group.NumberOfMembers);

        var totalDamage = CalculateDamage(meeleGroupsInAttackingArmy);
        //Debug.LogFormat("Total damage dealt {0}", totalDamage);
        battleEntry.TotalDamage += totalDamage;

        (int damageToMeele, int damageToRanged) = CalculateDamageMeleeAndRanged(attackingArmy, defendingArmy, numberOfMeeleUnitsInAttackingArmy, numberOfMeeleUnitsInDefendingArmy, totalDamage);

        var damageForEachMeleeDefendingGroup = Mathf.CeilToInt((float)damageToMeele / meeleGroupsInDefendingArmy.Count());
        var damageForEachRangedDefendingGroup = Mathf.CeilToInt((float)damageToRanged / meeleGroupsInDefendingArmy.Count());
        //Debug.Log($"Damage for each melee group: {damageForEachMeleeDefendingGroup}, damage for each ranged group {damageForEachRangedDefendingGroup}");

        var deadGroups = ApplyDamage(meeleGroupsInDefendingArmy, damageForEachMeleeDefendingGroup, battleEntry);
        deadGroups.AddRange(ApplyDamage(rangedGroupsInDefendingArmy, damageForEachRangedDefendingGroup, battleEntry));

        foreach (var group in deadGroups)
        {
            defendingArmy.Groups.Remove(group);
            defendingArmy.Army.soldiers.Remove(group.Group);
        }

        if (defendingArmy.Army.soldiers.Count == 0)
        {
            FinishBattle(attackingArmy);
        }
    }

    private (int, int) CalculateDamageMeleeAndRanged(ArmyData attackingArmy, ArmyData defendingArmy, int numberOfMeeleUnitsInAttackingArmy, int numberOfMeeleUnitsInDefendingArmy, int totalDamage)
    {
        //Debug.LogFormat("Number of attacking units: {0}, number of defending units: {1}", numberOfMeeleUnitsInAttackingArmy, numberOfMeeleUnitsInDefendingArmy);
        var numberofUnitsToOverflow = numberOfMeeleUnitsInAttackingArmy - 2 * numberOfMeeleUnitsInDefendingArmy;
        if (numberofUnitsToOverflow <= 0)
        {
            //Debug.Log($"{attackingArmy.Army.ArmyName} has not enough meele units to overflow, all damage is dealt to meele units");
            return (totalDamage, 0);
        }

        var rangedGroupsInDefendingArmy = GetGroupsOfType(UnitType.Ranged, defendingArmy.Groups);
        if (rangedGroupsInDefendingArmy.Count() == 0)
        {
            //Debug.Log($"{defendingArmy.Army.ArmyName} has not range units, all damage is dealt to meele units");
            return (totalDamage, 0);
        }

        float percentageOfDamageDealtToRangedUnits = (float)numberofUnitsToOverflow / numberOfMeeleUnitsInAttackingArmy;
        //Debug.Log($"{percentageOfDamageDealtToRangedUnits * 100f}% of damage is dealt to ranged units");

        int damageToRanged = Mathf.CeilToInt(percentageOfDamageDealtToRangedUnits * totalDamage);
        int damageToMeele = totalDamage - damageToRanged;

        return (damageToMeele, damageToRanged);
    }

    private void AttackWithRangedUnits(ArmyData attackingArmy, ArmyData defendingArmy, BattleEntry battleEntry)
    {
        //Debug.Log($"{attackingArmy.Army.ArmyName} attacks {defendingArmy.Army.ArmyName} with ranged.");
        var rangedUnitsInAttackingArmy = GetGroupsOfType(UnitType.Ranged, attackingArmy.Groups);

        if (rangedUnitsInAttackingArmy.Count() == 0)
        {
            //Debug.Log($"{attackingArmy.Army.ArmyName} has no ranged units.");
            return;
        }

        var totalDamage = CalculateDamage(rangedUnitsInAttackingArmy);
        //Debug.LogFormat("Total damage dealt {0}", totalDamage);
        battleEntry.TotalDamage += totalDamage;
        var damageForEachDefendingGroup = Mathf.CeilToInt((float)totalDamage / defendingArmy.Groups.Count());
        //Debug.LogFormat("Damage for singular group: {0}", damageForEachDefendingGroup);

        var deadGroups = ApplyDamage(defendingArmy.Groups, damageForEachDefendingGroup, battleEntry);

        foreach (var group in deadGroups)
        {
            defendingArmy.Groups.Remove(group);
            defendingArmy.Army.soldiers.Remove(group.Group);
        }

        if (defendingArmy.Army.soldiers.Count == 0)
        {
            FinishBattle(attackingArmy);
        }
    }


    private IEnumerable<BattleGroup> GetGroupsOfType(UnitType unitType, List<BattleGroup> armyGroups)
    {
        return armyGroups.Where(group => group.Group.unitData.UnitType == unitType);
    }

    private int CalculateDamage(IEnumerable<BattleGroup> battleGroups)
    {
        return battleGroups.Sum(group => group.Group.unitData.Attack * group.Group.NumberOfMembers);
    }

    private List<BattleGroup> ApplyDamage(IEnumerable<BattleGroup> damageReceivingGroups, int amountOfDamage, BattleEntry battleEntry)
    {
        List<BattleGroup> deadGroups = new();

        foreach (var defendingGroup in damageReceivingGroups)
        {
            var unitData = defendingGroup.Group.unitData;
            if (defendingGroup.RemainingHealth > amountOfDamage)
            {
                defendingGroup.RemainingHealth -= amountOfDamage;
                //Debug.Log($"0 {unitData.UnitTypeName} die after an attack, remaining health: {defendingGroup.RemainingHealth}. Units left in group: {defendingGroup.Group.NumberOfMembers}");
                battleEntry.AddDeadUnits(unitData.UnitTypeName, 0);
                continue;

            }

            var howManyUnitsDie = Mathf.Min(Mathf.FloorToInt(amountOfDamage / unitData.MaxHP), defendingGroup.Group.NumberOfMembers);
            var overflowDamage = amountOfDamage - (unitData.MaxHP * howManyUnitsDie);

            //Debug.Log($"Current health for {defendingGroup.Group.unitData.UnitTypeName}={defendingGroup.RemainingHealth}/{defendingGroup.Group.unitData.MaxHP}");
            if (overflowDamage >= defendingGroup.RemainingHealth)
            {
                ++howManyUnitsDie;
                defendingGroup.RemainingHealth = unitData.MaxHP - (overflowDamage - defendingGroup.RemainingHealth);
            }

            howManyUnitsDie = Mathf.Min(howManyUnitsDie, defendingGroup.Group.NumberOfMembers);
            defendingGroup.Group.NumberOfMembers -= howManyUnitsDie;
            //Debug.Log($"{howManyUnitsDie} {unitData.UnitTypeName} die after an attack, remaining health: {defendingGroup.RemainingHealth}/{unitData.MaxHP}. Units left in group: {defendingGroup.Group.NumberOfMembers}");
            battleEntry.AddDeadUnits(unitData.UnitTypeName, howManyUnitsDie);

            if (defendingGroup.Group.NumberOfMembers <= 0)
            {
                //Debug.Log($"{unitData.UnitTypeName}s are all dead.");
                deadGroups.Add(defendingGroup);
            }
        }

        return deadGroups;
    }

    private void FinishBattle(ArmyData winner)
    {
        Debug.LogFormat("{0} won!", winner.Army.ArmyName);
        UnitsTilemap.SetTile(Position, null);
        SpawnBattlefield();

        if(_attackingArmyAttackTimer != null)
        {
            _attackingArmyAttackTimer.Cancel();
        }

        if (_defendingArmyAttackTimer != null)
        {
            _defendingArmyAttackTimer.Cancel();
        }
        _isBattleFinished = true;

        var winningController = _attackingArmy;
        var losingController = _defendingArmy;

        if(winningController.army != winner.Army)
        {
            (winningController, losingController) = (losingController, winningController);
        }
        OnBattleFinished(winningController, losingController);
    }

    private void SpawnBattlefield()
    {
        Debug.Log("SpawnBattlefield() NOT IMPLEMENTED YET!");
    }
}
