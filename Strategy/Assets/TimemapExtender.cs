using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Grid))]
public class TimemapExtender : MonoBehaviour
{

    void Awake()
    {
        var allTilemaps = GetComponentsInChildren<Tilemap>();
        var minPosition = new Vector3Int(
            allTilemaps.Select(x => x.cellBounds.xMin).Min(),
            allTilemaps.Select(x => x.cellBounds.yMin).Min(),
            0);

        var maxPosition = new Vector3Int(
        allTilemaps.Select(x => x.cellBounds.xMax).Max(),
        allTilemaps.Select(x => x.cellBounds.yMax).Max(),
        0);

        foreach (var tileMap in allTilemaps)
        {
            tileMap.cellBounds.SetMinMax(minPosition, maxPosition);
        }
    }

}
