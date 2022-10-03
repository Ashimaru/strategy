using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionData : MonoBehaviour
{
    public RegionHeatManager HeatManager => _regionHeatManager;
    public CityController Capital => _capital;
    public List<VillageController> Villages => _villages;

    [SerializeField]
    private RegionHeatManager _regionHeatManager;
    [SerializeField]
    private CityController _capital;
    [SerializeField]
    private List<VillageController> _villages;


}
