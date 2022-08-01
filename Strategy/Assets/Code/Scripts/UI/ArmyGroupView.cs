using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArmyGroupView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI armyMemberName;
    [SerializeField]
    private TextMeshProUGUI armyMemberCount;
    
    public virtual void LoadSoldierGroup(SoldierGroup soldierGroup)
    {
        armyMemberName.text = soldierGroup.unitData.UnitTypeName;
        armyMemberCount.text = Convert.ToString(soldierGroup.NumberOfMembers);
    }
}
