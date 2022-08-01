using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class NewArmyViewController : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField armyName;
    [SerializeField]
    private ScrollView armyScrollView;
    [SerializeField]
    private GameObject armyElelmentsViewParent;
    [SerializeField]
    private GameObject armyEntryPrefab;

    private Army army;
    private List<GameObject> createdElements = new();


    private void Start()
    {
        gameObject.SetActive(army != null);
        armyName.onValueChanged.AddListener(value => army.ArmyName = value);
    }
    public void LoadArmyInfo(Army army)
    {
        this.army = army;
        gameObject.SetActive(true);
        armyName.text = army.ArmyName;

        foreach (var soldierGroup in army.soldiers)
        {
            var listEntry = Instantiate(armyEntryPrefab, armyElelmentsViewParent.transform, false);
            listEntry.name = soldierGroup.unitData.UnitTypeName;
            createdElements.Add(listEntry);
            var loaderScript = listEntry.GetComponent<ArmyGroupView>();
            loaderScript.LoadSoldierGroup(soldierGroup);
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
    }
}
