using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Location : MonoBehaviour, IEnterableTile
{

    [SerializeField]
    private LocationData _locationData;
    public LocationData LocationData { get; private set; }

    public Vector3Int Position { get; private set; }

    public bool ShouldSkipNextArmyEnter { get; set; }

    void Start()
    {
        LocationData = Instantiate(_locationData);
        LocationData.Garrison = Instantiate(_locationData.Garrison);
        Position = Systems.Get<IGrid>().WorldToGrid(transform.position);
        Systems.Get<ITileEnterListenerManager>().RegisterForTileEnter(Position, this);
    }
    public void OnArmyEnter(ArmyController army)
    {
        //Debug.Log($"Testing if {army.army.ArmyName} can enter {LocationData.LocationName} army alingment={army.army.Aligment} city alingment={LocationData.alignment}");
        if(army.army.Aligment == LocationData.alignment)
        {
            if (!ShouldSkipNextArmyEnter)
            {
                LocationData.Garrison.AddSoldiers(army.army.soldiers);
                army.Despawn();
                ShouldSkipNextArmyEnter = false;
            }
        }
    }
}
