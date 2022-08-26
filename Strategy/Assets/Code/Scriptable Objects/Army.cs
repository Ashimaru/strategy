using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum Alignment
{
    Necro,
    Human,
    Neutral
}

[Serializable]
public class SoldierGroup
{
    [SerializeField]
    public int NumberOfMembers;
    [SerializeField]
    public UnitData unitData;
}

[CreateAssetMenu(fileName = "Army", menuName = "Game/Army")]
public class Army : ScriptableObject
{
    public string ArmyName = "";
    public string CurrentAssigmentDescription = "Standby";
    public Alignment Aligment;
    public List<SoldierGroup> soldiers = new();

    public int Power
    {
        get { return CalculatePower(); }
    }

    public int Size
    {
        get { return CalculateNumberOfSoldiers(); }
    }

    public static Army CreateGroupFromArmy(Army sourceArmy, int minimumNumberOfMembers, int minimumPower)
    {
        Debug.Assert(sourceArmy.Size >= minimumNumberOfMembers &&
                     sourceArmy.Power >= minimumPower);

        var subgroup = CreateInstance<Army>();
        Action addRandomUnitToDeliveryGroup = () =>
        {
            var soldierType = sourceArmy.soldiers.RandomElement();
            subgroup.AddSoldiers(soldierType.unitData, 1);
            soldierType.NumberOfMembers -= 1;
            if (soldierType.NumberOfMembers <= 0)
            {
                subgroup.soldiers.Remove(soldierType);
            }
        };
        for (int i = 0; i < minimumNumberOfMembers; ++i)
        {
            addRandomUnitToDeliveryGroup();
        }

        while (subgroup.Power < minimumPower)
        {
            //Debug.Log($"Delivery group is too weak ({Utils.CalculateArmyPower(deliveryGroup)}) - adding another unit");
            addRandomUnitToDeliveryGroup();
        }

        return subgroup;
    }


    public void AddSoldiers(List<SoldierGroup> newSoldiers)
    {
        foreach (var newSoldierGroup in newSoldiers)
        {
            AddSoldiers(newSoldierGroup.unitData, newSoldierGroup.NumberOfMembers);
        }
    }

    public void AddSoldiers(UnitData unitData, int amount)
    {
        var soldierGroupInGarrison = soldiers.FirstOrDefault(currentArmy => currentArmy.unitData.UnitTypeName == unitData.UnitTypeName);
        if (soldierGroupInGarrison == null)
        {
            soldiers.Add(new SoldierGroup { 
                NumberOfMembers = amount,
                unitData =  unitData 
            });
            return;
        }

        soldierGroupInGarrison.NumberOfMembers += amount;
    }

    private int CalculateNumberOfSoldiers()
    {
        return soldiers.Sum(group =>  group.NumberOfMembers);
    }

    private int CalculatePower()
    {
        Func<SoldierGroup, int> calculateUnitsPower = soldierGroup =>
        {
            int power = soldierGroup.unitData.MaxHP / 2;
            power += soldierGroup.unitData.UnitType == UnitType.Melee ? soldierGroup.unitData.MeeleAttack : soldierGroup.unitData.RangedAttack;
            return power * soldierGroup.NumberOfMembers;
        };

        return soldiers.Sum(calculateUnitsPower);
    }
}
