using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

[System.Serializable]
public class OuterGridSettings
{
    public HexagonTileTypes HexagonTileTypes;
    public List<HexagonTileTypeSettings> HexagonTiles = new List<HexagonTileTypeSettings>();
    private List<int> hexagonTilePool;

    [Header("Recommended: 6.400 (example: 80 * 80) or less")]
    [Min(1)] public int GridXLength = 80;
    [Min(1)] public int GridZLength = 80;

    public void CreateHexagonTilePool()
    {
        if (HexagonTileTypes == null) return;

        hexagonTilePool = new List<int>();
        for (int i = 0; i < HexagonTiles.Count; i++)
        {
            for (int j = 0; j < (HexagonTiles[i].Chance * 10); j++)
            {
                hexagonTilePool.Add(i);
            }
        }
    }

    public string GetHexagonTileType()
    {
        return HexagonTileTypes.Types[hexagonTilePool[Random.Range(0, hexagonTilePool.Count)]].Name;
    }
}
