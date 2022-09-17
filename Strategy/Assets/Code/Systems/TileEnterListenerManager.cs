using System.Collections.Generic;
using UnityEngine;

public interface ITileEnterListenerManager
{
    void DeregisterForTileEnter(Vector3Int tileCoords, IEnterableTile listener);
    void RegisterForTileEnter(Vector3Int tileCoords, IEnterableTile listener);
    void OnTileEnter(Vector3Int coords, ArmyController army);
}

public class TileEnterListenerManager : MonoBehaviour, ITileEnterListenerManager
{
    private Dictionary<Vector3Int, IEnterableTile> _listeners = new();

    private void Awake()
    {
        Systems.RegisterSystem<ITileEnterListenerManager>(this);
    }

    private void OnDestroy()
    {
        Systems.DeregisterSystem<ITileEnterListenerManager>(this);
    }

    public void RegisterForTileEnter(Vector3Int tileCoords, IEnterableTile listener)
    {
        _listeners.Add(tileCoords, listener);
    }

    public void DeregisterForTileEnter(Vector3Int tileCoords, IEnterableTile listener)
    {
        _listeners.Remove(tileCoords);
    }

    public void OnTileEnter(Vector3Int coords, ArmyController army)
    {
        var listener = _listeners.GetValueOrDefault(coords);
        if(listener != null)
        {
            listener.OnArmyEnter(army);
        }
    }
}
