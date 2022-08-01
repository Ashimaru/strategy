using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ArmyViewController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI armyName;
    [SerializeField]
    private TextMeshProUGUI currentAssignmentDescription;
    [SerializeField]
    private ScrollView armyScrollView;
    [SerializeField]
    private GameObject armyElelmentsViewParent;
    [SerializeField]
    private GameObject armyEntryPrefab;

    public Army CurrentlyShownArmy { get => army; }

    private Army army;
    readonly private List<GameObject> createdElements = new();

    private void Start()
    {
        gameObject.SetActive(army != null);
    }
    public void LoadArmyInfo(Army army)
    {
        this.army = army;
        gameObject.SetActive(true);
        armyName.text = army.ArmyName;
        if (currentAssignmentDescription)
        {
            currentAssignmentDescription.text = army.CurrentAssigmentDescription;
        }

        foreach (var soldierGroup in army.soldiers)
        {
            var listEntry = Instantiate(armyEntryPrefab, armyElelmentsViewParent.transform, false);
            listEntry.name = soldierGroup.unitData.UnitTypeName;
            createdElements.Add(listEntry);
            var loaderScript = listEntry.GetComponent<ArmyGroupView>();
            loaderScript.LoadSoldierGroup(soldierGroup);
        }
    }

    private void Update()
    {
        if (army != null && currentAssignmentDescription != null)
        {
            currentAssignmentDescription.text = army.CurrentAssigmentDescription;
        }
    }


    public void ClearArmyInfo()
    {
        gameObject.SetActive(false);
        foreach (var element in createdElements)
        {
            Destroy(element);
        }
        createdElements.Clear();
        army = null;
    }
}
