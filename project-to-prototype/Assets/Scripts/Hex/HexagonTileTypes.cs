using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileTypes", menuName = "Tile/Tile types")]
public class HexagonTileTypes : ScriptableObject
{
    public List<HexagonTileType> Types = new List<HexagonTileType>();
}
