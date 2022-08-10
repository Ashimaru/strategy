using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ArmyController : MonoBehaviour,
                              IFocusableTile,
                              ITileSelectedListener
{
    public Army army;
    public ArmyViewController armyViewController;
    public Tilemap unitTilemap;
    public TileBase armyTile;

    [SerializeField]
    private Vector3Int currentPosition;
    private IOrder currentOrder;

    public Vector3Int CurrentPosition { 
        get { return currentPosition;  }
        set {
            Systems.Get<IClickableTile>().DeregisterClickableTile(currentPosition, this);
            unitTilemap.SetTile(currentPosition, null);
            currentPosition = value;
            gameObject.transform.position = Systems.Get<IGrid>().GridToWorld(value);
            unitTilemap.SetTile(currentPosition, armyTile);
            Systems.Get<IClickableTile>().RegisterClickableTile(currentPosition, TileType.Unit, this);
            Systems.Get<ITileEnterListenerManager>().OnTileEnter(currentPosition, this);
        }
    }

    public void MoveTo(Vector3Int targetPosition)
    {
        MoveTo(targetPosition, () => {
            Debug.Log("Movement done");
        });
    }

    public void MoveTo(Vector3Int targetPosition, Action onMoveDone)
    {
        if (targetPosition == CurrentPosition)
        {
            return;
        }

        currentOrder = new MoveOrder(this, targetPosition, onMoveDone);
    }

    public void Wait(float waitingTime)
    {
        Wait(waitingTime, () => { Debug.Log("Waiting done"); });
    }

    public void Wait(float waitingTime, Action onWaitDone)
    {
        currentOrder = new WaitOrder(this, waitingTime, onWaitDone);
    }

    public void OnFocusAcquired()
    {
        armyViewController.LoadArmyInfo(army);
        Systems.Get<ITileSelector>().RegisterForTileSelect(this);
    }

    public void OnFocusLost()
    {
        armyViewController.ClearArmyInfo();
        Systems.Get<ITileSelector>().ClearListener();
    }

    public void TileSelected(Vector3Int coordinates)
    {
        MoveTo(coordinates);
    }

    public void Despawn()
    {
        Systems.Get<IClickableTile>().DeregisterClickableTile(currentPosition, this);
        unitTilemap.SetTile(currentPosition, null);
        if (armyViewController.CurrentlyShownArmy == army)
        {
            armyViewController.ClearArmyInfo();
        }
        Destroy(gameObject);
    }
}
