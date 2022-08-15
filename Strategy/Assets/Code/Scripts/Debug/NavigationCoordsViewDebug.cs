using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NavigationCoordsViewDebug : MonoBehaviour
{
    public Grid grid;
    public Canvas textCanvas;
    public Tilemap tilemap;

    private List<Canvas> coordinateView = new List<Canvas>();

    private readonly Vector3Int YZ_INCREMENT = new Vector3Int(0,1,-1);
    private readonly Vector3Int XZ_INCREMENT = new Vector3Int(1,0,-1);
    private readonly Vector3Int XY_INCREMENT = new Vector3Int(1,-1,0);

    private void Start()
    {
        ShowCoordView();
    }

    private void ShowCoordView()
    {
        var xMin = tilemap.cellBounds.xMin;
        var xMax = tilemap.cellBounds.xMax;
        var yMin = tilemap.cellBounds.yMin;
        var yMax = tilemap.cellBounds.yMax;
        Vector3Int xNumerationOffset = Vector3Int.zero;
        for (int x = xMin; x < xMax; ++x)
        {
            var yNumerationOffset = Vector3Int.zero;
            for (int y = yMin; y < yMax; ++y)
            {
                var worldPos = grid.CellToWorld(new Vector3Int(x, y, 0));
                var canvas = Instantiate(textCanvas, worldPos, Quaternion.identity, this.transform);
                var text = canvas.GetComponent<UnityEngine.UI.Text>();
                var currentNodeNumeration = xNumerationOffset + yNumerationOffset;
                text.text = "(" + (x - xMin).ToString() + ", " + (y - yMin).ToString() + /*", " + z.ToString() +*/ ")\n(" + currentNodeNumeration.x + ", " + currentNodeNumeration.y + ", " + currentNodeNumeration.z + ")";
                yNumerationOffset += y % 2 != 0 ? XY_INCREMENT : XZ_INCREMENT;
                coordinateView.Add(canvas);
            }
            xNumerationOffset += YZ_INCREMENT;
        }
    }
}
