using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    private ArmyController _attackingArmy;
    private ArmyController _defendingArmy;

    private Timer _attackingArmyAttackTimer;
    private Timer _defendingArmyAttackTimer;

    private ArmyData _attackingArmyData;
    private ArmyData _defendingArmyData;

    private bool _isBattleFinished = false;

    [SerializeField]
    private Army _testAttackingArmy;
    [SerializeField]
    private Army _testDefendingArmy;



    void Start()
    {
        var attackingArmyCopy = Instantiate(_testAttackingArmy);
        var defendingArmyCopy = Instantiate(_testDefendingArmy);

        _attackingArmyData = new ArmyData(attackingArmyCopy);
        _defendingArmyData = new ArmyData(defendingArmyCopy);

        Attack(_attackingArmyData, _defendingArmyData);
        Attack(_defendingArmyData, _attackingArmyData);

        _attackingArmyAttackTimer = Utils.CreateRepeatingTimer(gameObject, 1, () => Attack(_attackingArmyData, _defendingArmyData));
        _defendingArmyAttackTimer = Utils.CreateRepeatingTimer(gameObject, 1.1F, () => Attack(_defendingArmyData, _attackingArmyData));
    }

    public void StartBattle(ArmyController attackingArmy, ArmyController defendingArmy)
    {
        _attackingArmy = attackingArmy;
        _defendingArmy = defendingArmy;

        _attackingArmyData = new ArmyData(_attackingArmy.army);
        _defendingArmyData = new ArmyData(_defendingArmy.army);
    }

    private void Attack(ArmyData attackingArmy, ArmyData defendingArmy)
    {
        Debug.Log($"{attackingArmy.Army.ArmyName} attacks {defendingArmy.Army.ArmyName} for {++attackingArmy.AttackTurn} time.");
        AttackWithMeleeUnits(attackingArmy, defendingArmy);

        if(!_isBattleFinished)
        {
            AttackWithRangedUnits(attackingArmy, defendingArmy);
        }
    }

    private void AttackWithMeleeUnits(ArmyData attackingArmy, ArmyData defendingArmy)
    {
        Debug.Log($"{attackingArmy.Army.ArmyName} attacks {defendingArmy.Army.ArmyName} with melee.");
        var meeleGroupsInAttackingArmy = GetGroupsOfType(UnitType.Melee, attackingArmy.Groups);
        var meeleGroupsInDefendingArmy = GetGroupsOfType(UnitType.Melee, defendingArmy.Groups);

        var numberOfMeeleUnitsInAttackingArmy = meeleGroupsInAttackingArmy.Sum(group => group.Group.NumberOfMembers);
        var numberOfMeeleUnitsInDefendingArmy = meeleGroupsInDefendingArmy.Sum(group => group.Group.NumberOfMembers);

        Debug.LogFormat("Number of attacking units: {0}, number of defending units: {1}", numberOfMeeleUnitsInAttackingArmy, numberOfMeeleUnitsInDefendingArmy);
        if (numberOfMeeleUnitsInAttackingArmy > 2 * numberOfMeeleUnitsInDefendingArmy)
        {
            Debug.Log("OVERFLOW - implement it!");
        }

        var totalDamage = CalculateDamage(meeleGroupsInAttackingArmy, group => group.Group.unitData.MeeleAttack);
        Debug.LogFormat("Total damage dealt {0}", totalDamage);
        var damageForEachDefendingGroup = Mathf.CeilToInt((float)totalDamage / meeleGroupsInDefendingArmy.Count());
        Debug.LogFormat("Damage for singular group: {0}", damageForEachDefendingGroup);

        var deadGroups = ApplyDamage(meeleGroupsInDefendingArmy, damageForEachDefendingGroup);

        foreach (var group in deadGroups)
        {
            defendingArmy.Groups.Remove(group);
            defendingArmy.Army.soldiers.Remove(group.Group);
        }

        if (defendingArmy.Army.soldiers.Count == 0)
        {
            Debug.LogFormat("{0} won!", attackingArmy.Army.ArmyName);
            _attackingArmyAttackTimer.Cancel();
            _defendingArmyAttackTimer.Cancel();
            _isBattleFinished = true;
        }
    }

    private void AttackWithRangedUnits(ArmyData attackingArmy, ArmyData defendingArmy)
    {
        Debug.Log($"{attackingArmy.Army.ArmyName} attacks {defendingArmy.Army.ArmyName} with ranged.");
        var rangedUnitsInAttackingArmy = GetGroupsOfType(UnitType.Ranged, attackingArmy.Groups);
        
        if(rangedUnitsInAttackingArmy.Count() == 0)
        {
            Debug.Log($"{attackingArmy.Army.ArmyName} has no ranged units.");
            return;
        }

        var totalDamage = CalculateDamage(rangedUnitsInAttackingArmy, group => group.Group.unitData.RangedAttack);
        Debug.LogFormat("Total damage dealt {0}", totalDamage);
        var damageForEachDefendingGroup = Mathf.CeilToInt((float)totalDamage / defendingArmy.Groups.Count());
        Debug.LogFormat("Damage for singular group: {0}", damageForEachDefendingGroup);

        var deadGroups = ApplyDamage(defendingArmy.Groups, damageForEachDefendingGroup);

        foreach (var group in deadGroups)
        {
            defendingArmy.Groups.Remove(group);
            defendingArmy.Army.soldiers.Remove(group.Group);
        }

        if (defendingArmy.Army.soldiers.Count == 0)
        {
            Debug.LogFormat("{0} won!", attackingArmy.Army.ArmyName);
            _attackingArmyAttackTimer.Cancel();
            _defendingArmyAttackTimer.Cancel();
            _isBattleFinished = true;
        }
    }


    private IEnumerable<BattleGroup> GetGroupsOfType(UnitType unitType, List<BattleGroup> armyGroups)
    {
        return armyGroups.Where(group => group.Group.unitData.UnitType == unitType);
    }

    private int CalculateDamage(IEnumerable<BattleGroup> battleGroups, Func<BattleGroup, int> attackStatAccessor)
    {
       return battleGroups.Sum(group => attackStatAccessor(group) * group.Group.NumberOfMembers);
    }

    private List<BattleGroup> ApplyDamage(IEnumerable<BattleGroup> damageReceivingGroups, int amountOfDamage)
    {
        List<BattleGroup> deadGroups = new();

        foreach (var defendingGroup in damageReceivingGroups)
        {
            var unitData = defendingGroup.Group.unitData;
            if (defendingGroup.RemainingHealth > amountOfDamage)
            {
                defendingGroup.RemainingHealth -= amountOfDamage;
                Debug.Log($"Net enough damage to kill {unitData.UnitTypeName}, remaining health={defendingGroup.RemainingHealth}");
                continue;

            }

            var howManyUnitsDie = Mathf.Min(Mathf.FloorToInt(amountOfDamage / unitData.MaxHP), defendingGroup.Group.NumberOfMembers);
            var overflowDamage = amountOfDamage - (unitData.MaxHP * howManyUnitsDie);

            Debug.Log($"Remaining health for {defendingGroup.Group.unitData.UnitTypeName}={defendingGroup.RemainingHealth}/{defendingGroup.Group.unitData.MaxHP}");
            if (overflowDamage >= defendingGroup.RemainingHealth)
            {
                ++howManyUnitsDie;
                defendingGroup.RemainingHealth = unitData.MaxHP - (overflowDamage - defendingGroup.RemainingHealth);
            }

            defendingGroup.Group.NumberOfMembers -= howManyUnitsDie;
            Debug.Log($"{howManyUnitsDie} {unitData.UnitTypeName} die after an attack, remaining health: {defendingGroup.RemainingHealth}. Units left in group: {defendingGroup.Group.NumberOfMembers}");

            if (defendingGroup.Group.NumberOfMembers <= 0)
            {
                Debug.Log($"{unitData.UnitTypeName}s are all dead.");
                deadGroups.Add(defendingGroup);
            }
        }

        return deadGroups;
    }
}
