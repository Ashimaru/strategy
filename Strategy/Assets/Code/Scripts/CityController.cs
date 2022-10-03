using UnityEngine;

public class CityController : MonoBehaviour
{
    public Location Location => _location;
    private Location _location;


    void Start()
    {
        _location = GetComponent<Location>();
    }
}
