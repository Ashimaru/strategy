using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

enum ResourceType
{
    Bodies,
    Bones,
    Mana,
    Gold
}



public class ResourceViewController : MonoBehaviour
{
    [SerializeField]
    private ResourceType _resourceType;
    [SerializeField]
    private TextMeshProUGUI _resourcesAmountText;
    [SerializeField]
    private PlayerResources _playerResources;

    void Start()
    {
        Resource resourceToObserve = null;
        switch(_resourceType)
        {
            case ResourceType.Bodies:
                resourceToObserve = _playerResources.Resources.Bodies;
                break;
            case ResourceType.Bones:
                resourceToObserve = _playerResources.Resources.Bones;
                break;
            case ResourceType.Mana:
                resourceToObserve = _playerResources.Resources.Mana;
                break;
            case ResourceType.Gold:
                resourceToObserve = _playerResources.Resources.Gold;
                break;
        }

        Debug.Assert(resourceToObserve != null);
        resourceToObserve.OnAmountChanged += UpdateValue;
        UpdateValue(0, resourceToObserve.Amount);
        
    }

    void UpdateValue(int oldValue, int newValue)
    {
        _resourcesAmountText.text = newValue.ToString();
    }
}
