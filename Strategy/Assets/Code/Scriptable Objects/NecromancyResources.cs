using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Resource
{
    [SerializeField]
    private int _amount;
    public int Amount { get => _amount; set
        {
            Debug.Log("SET " + _amount);
            OnAmountChanged.Invoke(_amount, value);
            _amount = value;
        } }

    public delegate void ValueChanged(int oldValue, int newValue);
    public event ValueChanged OnAmountChanged;
}


[CreateAssetMenu(fileName = "NecroResources", menuName = "Game/NecromancyResources")]
public class NecromancyResources : ScriptableObject
{
    [SerializeField]
    Resource _bodies;
    [SerializeField]
    Resource _bones;
    [SerializeField]
    Resource _mana;
    [SerializeField]
    Resource _gold;

    public Resource Bodies { get => _bodies;}
    public Resource Bones { get => _bones;}
    public Resource Mana { get => _mana;}
    public Resource Gold { get => _gold;}
}
