using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexagonTerrainSettings
{
    [Header("Hex terrain size")]
    public int HexagonTerrainXLength = 50;
    [HideInInspector] public int HexagonTerrainXStart { get; private set; }
    [HideInInspector] public int HexagonTerrainXEnd { get; private set; }

    public int HexagonTerrainZLength = 30;
    [HideInInspector] public int HexagonTerrainZStart { get; private set; }
    [HideInInspector] public int HexagonTerrainZEnd { get; private set; }

    public HexagonTileTypes HexagonTileTypes { get { return hexagonTileTypes; } }
    [Header("Hexagon tile types")]
    [SerializeField] private HexagonTileTypes hexagonTileTypes;
    public List<HexagonTileTypeChance> HexagonTiles = new List<HexagonTileTypeChance>();
    private List<int> hexagonTilePool;

    [Header("Hexagon tile offset")]
    public float HexagonTileXOffset = 0.5f;
    public float HexagonTileZOffset = -0.1f;
    public float HexagonTileXSpaceCorrection = 0f;
    public float HexagonTileZSpaceCorrection = 0f;
    public float HexagonTileXOddOffset = 0.5f;

    private int previousHexTerrainXLength;
    private int previousHexTerrainZLength;
    private float previousHexagonTileXOffset;
    private float previousHexagonTileZOffset;
    private float previousHexagonTileXSpaceCorrection;
    private float previousHexagonTileZSpaceCorrection;
    private float previousHexagonTileXOddOffset;


    public void UpdateValues()
    {
        previousHexTerrainXLength = HexagonTerrainXLength;
        previousHexTerrainZLength = HexagonTerrainZLength;
        previousHexagonTileXOffset = HexagonTileXOffset;
        previousHexagonTileZOffset = HexagonTileZOffset;
        previousHexagonTileXSpaceCorrection = HexagonTileXSpaceCorrection;
        previousHexagonTileZSpaceCorrection = HexagonTileZSpaceCorrection;
        previousHexagonTileXOddOffset = HexagonTileXOddOffset;
    }

    public bool HasValueChanged()
    {
        if (previousHexTerrainXLength == HexagonTerrainXLength && previousHexTerrainZLength == HexagonTerrainZLength && 
            previousHexagonTileXOffset == HexagonTileXOffset && previousHexagonTileZOffset == HexagonTileZOffset &&
            previousHexagonTileXSpaceCorrection == HexagonTileXSpaceCorrection && previousHexagonTileZSpaceCorrection == HexagonTileZSpaceCorrection &&
            previousHexagonTileXOddOffset == HexagonTileXOddOffset) return false;

        previousHexTerrainXLength = HexagonTerrainXLength;
        previousHexTerrainZLength = HexagonTerrainZLength;
        previousHexagonTileXOffset = HexagonTileXOffset;
        previousHexagonTileZOffset = HexagonTileZOffset;
        previousHexagonTileXSpaceCorrection = HexagonTileXSpaceCorrection;
        previousHexagonTileZSpaceCorrection = HexagonTileZSpaceCorrection;
        previousHexagonTileXOddOffset = HexagonTileXOddOffset;
        return true;
    }

    public void CalculateHexTerrainBounds(GridSettings grid)
    {
        HexagonTerrainXStart = (grid.GridXLength - HexagonTerrainXLength) / 2;
        HexagonTerrainXEnd = HexagonTerrainXStart + HexagonTerrainXLength;

        HexagonTerrainZStart = (grid.GridZLength - HexagonTerrainZLength) / 2;
        HexagonTerrainZEnd = HexagonTerrainZStart + HexagonTerrainZLength;
    }

    public void CreateHexagonTilePool()
    {
        HexagonTiles[0].Chance = 0;
        hexagonTilePool = new List<int>();
        for (int i = 0; i < HexagonTiles.Count; i++)
        {
            for (int j = 0; j < (HexagonTiles[i].Chance * 10); j++)
            {
                hexagonTilePool.Add(i);
            }
        }
    }

    public int GetHexagonTileType()
    {
        return hexagonTilePool[Random.Range(0, hexagonTilePool.Count)];
    }
}
