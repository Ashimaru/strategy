using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Location", menuName = "Game/Location")]
public class LocationData : ScriptableObject
{
    public string LocationName = "";
    public Alignment alignment = Alignment.Human;
    public Army Garrison;

    public List<Army> VisitingArmies = new();
}
