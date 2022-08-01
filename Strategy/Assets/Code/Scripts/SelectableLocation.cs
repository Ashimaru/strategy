using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Location))]
public class SelectableLocation : MonoBehaviour, IFocusableTile
{
    [SerializeField] private LocationViewController _locationView;
    private Location _location;

    public Action CreateArmyDelegate;

    // Start is called before the first frame update
    void Start()
    {
        _location = GetComponent<Location>();
        Systems.Get<IClickableTile>().RegisterClickableTile(_location.Position, TileType.Location, this);
    }

    public void OnFocusAcquired()
    {
        _locationView.LoadLocation(_location, CreateArmyDelegate);
    }

    public void OnFocusLost()
    {
        _locationView.LoadLocation(null, null);
    }
}
