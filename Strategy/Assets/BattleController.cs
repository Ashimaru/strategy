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

        AttackWithMeleeUnits(_attackingArmyData, _defendingArmyData);
        AttackWithMeleeUnits(_defendingArmyData, _attackingArmyData);
        _attackingArmyAttackTimer = Utils.CreateRepeatingTimer(gameObject, 1, () => AttackWithMeleeUnits(_attackingArmyData, _defendingArmyData));
        _defendingArmyAttackTimer = Utils.CreateRepeatingTimer(gameObject, 1.1F, () => AttackWithMeleeUnits(_defendingArmyData, _attackingArmyData));
    }

    public void StartBattle(ArmyController attackingArmy, ArmyController defendingArmy)
    {
        _attackingArmy = attackingArmy;
        _defendingArmy = defendingArmy;

        _attackingArmyData = new ArmyData(_attackingArmy.army);
        _defendingArmyData = new ArmyData(_defendingArmy.army);
    }

    private void AttackWithMeleeUnits(ArmyData attackingArmy, ArmyData defendingArmy)
    {
        Debug.Log($"{attackingArmy.Army.ArmyName} attacks {defendingArmy.Army.ArmyName} for {++attackingArmy.AttackTurn} time.");
        var meeleGroupsInAttackingArmy = GetGroupsOfType(UnitType.Melee, attackingArmy.Groups);
        var meeleGroupsInDefendingArmy = GetGroupsOfType(UnitType.Melee, defendingArmy.Groups);

        var numberOfMeeleUnitsInAttackingArmy = meeleGroupsInAttackingArmy.Sum(group => group.Group.NumberOfMembers);
        var numberOfMeeleUnitsInDefendingArmy = meeleGroupsInDefendingArmy.Sum(group => group.Group.NumberOfMembers);

        Debug.LogFormat("Number of attacking units: {0}, number of defending units: {1}", numberOfMeeleUnitsInAttackingArmy, numberOfMeeleUnitsInDefendingArmy);
        if (numberOfMeeleUnitsInAttackingArmy > 2 * numberOfMeeleUnitsInDefendingArmy)
        {
            Debug.Log("OVERFLOW - implement it!");
        }

        var totalDamage = meeleGroupsInAttackingArmy.Sum(group => group.Group.unitData.MeeleAttack * group.Group.NumberOfMembers);
        Debug.LogFormat("Total damage dealt {0}", totalDamage);
        var damageForEachDefendingGroup = Mathf.CeilToInt((float)totalDamage / meeleGroupsInDefendingArmy.Count());
        Debug.LogFormat("Damage for singular group: {0}", damageForEachDefendingGroup);

        List<BattleGroup> deadGroups = new();

        foreach (var defendingGroup in meeleGroupsInDefendingArmy)
        {
            var unitData = defendingGroup.Group.unitData;
            if (defendingGroup.RemainingHealth > damageForEachDefendingGroup)
            {
                defendingGroup.RemainingHealth -= damageForEachDefendingGroup;
                Debug.Log($"Net enough damage to kill {unitData.UnitTypeName}, remaining health={defendingGroup.RemainingHealth}");
                continue;

            }

            var howManyUnitsDie = Mathf.Min(unitData.MaxHP / damageForEachDefendingGroup, defendingGroup.Group.NumberOfMembers);
            var overflowDamage = damageForEachDefendingGroup - (unitData.MaxHP * howManyUnitsDie);

            if(overflowDamage >= defendingGroup.RemainingHealth)
            {
                ++howManyUnitsDie;
                defendingGroup.RemainingHealth = unitData.MaxHP - (overflowDamage - defendingGroup.RemainingHealth);
            }

            defendingGroup.Group.NumberOfMembers -= howManyUnitsDie;
            Debug.Log($"{howManyUnitsDie} {unitData.UnitTypeName} die after an attack, remaining health: {defendingGroup.RemainingHealth}. Units left in group: {defendingGroup.Group.NumberOfMembers}");

            if(defendingGroup.Group.NumberOfMembers <=0)
            {
                Debug.Log($"{unitData.UnitTypeName}s are all dead.");
                deadGroups.Add(defendingGroup);
            }
        }

        foreach (var group in deadGroups)
        {
            defendingArmy.Groups.Remove(group);
            defendingArmy.Army.soldiers.Remove(group.Group);
        }

        if(defendingArmy.Army.soldiers.Count == 0)
        {
            Debug.LogFormat("{0} won!", attackingArmy.Army.ArmyName);
            _attackingArmyAttackTimer.Cancel();
            _defendingArmyAttackTimer.Cancel();
        }

    }

    private IEnumerable<BattleGroup> GetGroupsOfType(UnitType unitType, List<BattleGroup> armyGroups)
    {
        return armyGroups.Where(group => group.Group.unitData.UnitType == unitType);
    }
}
