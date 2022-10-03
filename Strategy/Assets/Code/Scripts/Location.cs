using UnityEngine;
public class Location : MonoBehaviour
{

    [SerializeField]
    private LocationData _locationData;
    public LocationData LocationData { get; private set; }

    public Vector3Int Position { get; private set; }

    void OnDestroy()
    {
        Systems.Get<IRepository<Location>>().Remove(this);
    }

    void Start()
    {
        LocationData = Instantiate(_locationData);
        LocationData.Garrison = Instantiate(_locationData.Garrison);
        Position = Systems.Get<IGrid>().WorldToGrid(transform.position);
        Systems.Get<IRepository<Location>>().Add(this);
    }
}
