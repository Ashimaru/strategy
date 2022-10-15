using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public enum TileType
{
    Unit,
    Location
}

public interface IClickableTile
{
    void RegisterClickableTile(Vector3 position, TileType tileType, IFocusableTile tile);
    void RegisterClickableTile(Vector3Int position, TileType tileType, IFocusableTile tile);
    void DeregisterClickableTile(Vector3Int position, IFocusableTile tile);
}

public enum Scenario
{
    TestNav,
    FocusOnTiles,
    ShowCoordinates
}

public class ClickableTile : MonoBehaviour, IClickableTile
{
    public struct TileTypeFocusableTile
    {
        public TileType tileType;
        public IFocusableTile tile;

        public TileTypeFocusableTile(TileType tileType, IFocusableTile tile)
        {
            this.tileType = tileType;
            this.tile = tile;
        }
    }

    [SerializeField] private Navigation navigation;
    [SerializeField] private Tilemap tiles;
    [SerializeField] private Scenario selectedScenario;

    private List<Vector3Int> path = new();
    private List<Vector3Int> navPoints = new();


    private Dictionary<Vector3Int, List<TileTypeFocusableTile>> clickableTiles = new();

    TileTypeFocusableTile? CurrentFocus;



    public void RegisterClickableTile(Vector3Int position, TileType tileType, IFocusableTile tile)
    {
        if(!clickableTiles.ContainsKey(position))
        {
            var tiles = new List<TileTypeFocusableTile>() { new TileTypeFocusableTile(tileType, tile) };
            clickableTiles.Add(position, tiles);
        }
        else
        {
            clickableTiles[position].Add(new TileTypeFocusableTile(tileType, tile));
            clickableTiles[position].OrderBy(x => x.tileType);
        }
    }

    public void RegisterClickableTile(Vector3 position, TileType tileType, IFocusableTile tile)
    {
        RegisterClickableTile(Systems.Get<IGrid>().WorldToGrid(position), tileType, tile);
    }

    public void DeregisterClickableTile(Vector3Int position, IFocusableTile tile)
    {
        if(! clickableTiles.ContainsKey(position))
        {
            return;
        }

        clickableTiles[position].RemoveAll(entry => entry.tile == tile);
        if(clickableTiles[position].Count == 0)
        {
            clickableTiles.Remove(position);
        }
    }

    void Awake()
    {
        Systems.RegisterSystem<IClickableTile>(this);
    }

    private void OnDestroy()
    {
        Systems.DeregisterSystem<IClickableTile>(this);
    }

    void Update()
    {
        if (Debug.isDebugBuild && Input.GetKeyUp(KeyCode.RightBracket))
        {
            var modes = Enum.GetValues(typeof(Scenario)).Cast<int>();
            var newSelection = (int)selectedScenario + 1;
            selectedScenario = modes.Contains(newSelection) ? (Scenario)newSelection : (Scenario)modes.First();
        }

        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log("Over gameobject.");
            return;
        }


        switch (selectedScenario)
        {
            case Scenario.TestNav:
                TestNav();
                break;
            case Scenario.FocusOnTiles:
                FocusOnTiles();
                break;
            case Scenario.ShowCoordinates:
                Debug.Log(GetTilePostionOnGrid());
                break;
            default:
                Debug.Assert(false);
                break;
        }
    }

    private void FocusOnTiles()
    {
        var position = GetTilePostionOnGrid();
        //Debug.Log(position);
        //Debug.Log(clickableTiles.Stringify());

        var possibleTiles = clickableTiles.GetValueOrDefault(position);
        if (possibleTiles == null)
        {
            ClearCurrentFocus();
            return;
        }

        var nextTile = GetNextTile(possibleTiles);
        SetCurrentFocus(nextTile);
    }

    private void TestNav()
    {
        var position = GetTilePostionOnGrid();

        navPoints.Add(position);
        if (navPoints.Count == 1)
        {
            return;
        }

        if (navPoints.Count == 2)
        {
            ColorTiles(path, Color.white);
            path = navigation.NavigateTowards(navPoints[0], navPoints[1]);
            ColorTiles(path, Color.red);
        }

        navPoints.Clear();
    }

    Vector3Int GetTilePostionOnGrid()
    {
        return Systems.Get<IGrid>().WorldToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    void ColorTile(Vector3Int coords, Color color)
    {
        tiles.SetTileFlags(coords, TileFlags.None);
        tiles.SetColor(coords, color);
    }

    TileTypeFocusableTile GetNextTile(List<TileTypeFocusableTile> tiles)
    {
        if(!CurrentFocus.HasValue)
        {
            return tiles[0];
        }

        var currentIndex = tiles.IndexOf(CurrentFocus.Value);
        if(currentIndex == tiles.Count - 1)
        {
            return tiles[0];
        }

        return tiles[++currentIndex];


    }
    void SetCurrentFocus(TileTypeFocusableTile value)
    {
        if(!CurrentFocus.HasValue)
        {
            CurrentFocus = value;
            value.tile.OnFocusAcquired();
        }

        if(CurrentFocus.Value.tile == value.tile)
        {
            return;
        }

        CurrentFocus.Value.tile.OnFocusLost();
        CurrentFocus = value;
        value.tile.OnFocusAcquired();
    }

    void ClearCurrentFocus()
    {
        if(CurrentFocus.HasValue)
        {
            CurrentFocus.Value.tile.OnFocusLost();
        }
        CurrentFocus = null;
    }


    void ColorTiles(IEnumerable<Vector3Int> tilesToChange, Color color)
    {
        foreach (var tile in tilesToChange)
        {
            ColorTile(tile, color);
        }
    }
}
