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
            var soldierGroupInGarrison = soldiers.FirstOrDefault(currentArmy => currentArmy.unitData.UnitTypeName == newSoldierGroup.unitData.UnitTypeName);
            if(soldierGroupInGarrison == null)
            {
                soldiers.Add(newSoldierGroup);
                continue;
            }

            soldierGroupInGarrison.NumberOfMembers += newSoldierGroup.NumberOfMembers;
        }
    }
}
