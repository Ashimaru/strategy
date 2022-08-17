using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileProperties : ScriptableObject
{
    public TileBase[] tiles;

    public bool isWalkable = true;
    public float walkingCost = 1.0f;
}
