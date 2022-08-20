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

    public int CalculateNumberOfSoldiers()
    {
        return soldiers.Sum(group =>  group.NumberOfMembers);
    }
}
