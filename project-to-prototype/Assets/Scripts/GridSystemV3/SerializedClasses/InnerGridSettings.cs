using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InnerGridSettings
{
    [Header("Grid size")]
    public int GridXLength = 50;
    [HideInInspector] public int GridXStart { get; private set; }
    [HideInInspector] public int GridXEnd { get; private set; }

    public int GridZLength = 30;
    [HideInInspector] public int GridZStart { get; private set; }
    [HideInInspector] public int GridZEnd { get; private set; }

    [Space(10)]
    public HexagonTileTypes HexagonTileTypes;
    public List<HexagonTileTypeSettings> HexagonTiles = new List<HexagonTileTypeSettings>();
    private List<int> hexagonTilePool;

    public void CalculateBounds(OuterGridSettings grid)
    {
        GridXStart = (grid.GridXLength - GridXLength) / 2;
        GridXEnd = GridXStart + GridXLength;

        GridZStart = (grid.GridZLength - GridZLength) / 2;
        GridZEnd = GridZStart + GridZLength;
    }

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
