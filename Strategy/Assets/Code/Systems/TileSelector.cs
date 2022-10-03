using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ITileSelectedListener
{
    abstract void TileSelected(Vector3Int coordinates);
}

public interface ITileSelector
{
    void ClearListener();
    void RegisterForTileSelect(ITileSelectedListener listener);
}

public class TileSelector : MonoBehaviour, ITileSelector
{
    private ITileSelectedListener listener;
    public void RegisterForTileSelect(ITileSelectedListener listener)
    {
        this.listener = listener;
    }

    public void ClearListener()
    {
        listener = null;
    }

    void Awake()
    {
        Systems.RegisterSystem<ITileSelector>(this);
    }

    private void OnDestroy()
    {
        Systems.DeregisterSystem<ITileSelector>(this);
    }
    void Update()
    {
        if (!Input.GetMouseButtonUp(1))
        {
            return;
        }

        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (listener == null)
        {
            return;
        }

        listener.TileSelected(GetMousePosition());
    }

    Vector3Int GetMousePosition()
    {
        return Systems.Get<IGrid>().WorldToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}