using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileTypes", menuName = "Tile/Tile types")]
public class HexTileTypes : ScriptableObject
{
    public List<Tile> TileTypes = new List<Tile>();
}
