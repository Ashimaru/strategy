using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyCreatorViewController : MonoBehaviour
{
    [SerializeField]
    private ArmyViewController garrisonViewController;
    [SerializeField]
    private NewArmyViewController newArmyController;

    private Action<Army> onSplitDone;
    private Army garrison;
    private Army newArmy;

    void Start()
    {
        gameObject.SetActive(false);
    }

    private void ClearViews()
    {
        newArmyController.ClearArmyInfo();
        garrisonViewController.ClearArmyInfo();
    }

    private void ReloadViews()
    {
        ClearViews();
        LoadArmies();
    }

    private Army GetArmyWithSoldierGroup(SoldierGroup soldierGroup)
    {
        if (garrison.soldiers.Contains(soldierGroup))
        {
            return garrison;
        }

        if(newArmy.soldiers.Contains(soldierGroup))
        {
            return newArmy;
        }
        return null;
    }

    private void LoadArmies()
    {
        garrisonViewController.LoadArmyInfo(garrison);
        newArmyController.LoadArmyInfo(newArmy);
    }

    public void LoadArmies(Army garrison, Army newArmy, Action<Army> onSplitDone)
    {
        this.garrison = garrison;
        this.newArmy = newArmy;
        gameObject.SetActive(true);
        this.onSplitDone = onSplitDone;
        LoadArmies();
    }

    public void MoveGroupToOtherArmy(SoldierGroup soldierGroup)
    {
        var sourceArmy = GetArmyWithSoldierGroup(soldierGroup);
        Debug.Assert(sourceArmy != null);

        if(sourceArmy == garrison)
        {
            newArmy.soldiers.Add(soldierGroup);
            garrison.soldiers.Remove(soldierGroup);
        }
        else
        {
            garrison.soldiers.Add(soldierGroup);
            newArmy.soldiers.Remove(soldierGroup);
        }
        ReloadViews();
    }

    public void CompleteCreation()
    {
        onSplitDone(newArmy);
        ClearViews();
        gameObject.SetActive(false);
    }

    public void CancelCreation()
    {
        garrison.soldiers.AddRange(newArmy.soldiers);
        onSplitDone(null);
        ClearViews();
        gameObject.SetActive(false);
    }
}
