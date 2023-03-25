using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

[System.Serializable]
public class OuterGridSettings
{
    [Header("Recommended: 6.400 (example: 80 * 80) or less")]
    [Min(1)] public int GridXLength = 80;
    [Min(1)] public int GridZLength = 80;

    [Space(10)]
    public HexagonTileTypes HexagonTileTypes;
    public List<HexagonTileTypeSettings> HexagonTiles = new List<HexagonTileTypeSettings>();

    public string GetHexagonTileType(float lowestHeight, float highestHeight, float currentHeight)
    {
        string nameOfTileType = "";
        foreach (HexagonTileTypeSettings tile in HexagonTiles)
        {
            if (nameOfTileType == "") nameOfTileType = tile.Name;
            else
            {
                float heightLimit = lowestHeight + ((highestHeight - lowestHeight) * tile.Height);
                nameOfTileType = heightLimit > currentHeight ? tile.Name : nameOfTileType;
            }
        }
        return nameOfTileType;
    }
}
