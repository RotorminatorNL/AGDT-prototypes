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

    [Header("Hexagon tile offset")]
    public float HexagonTileXSpaceCorrection = 0f;
    public float HexagonTileZSpaceCorrection = 0.134f;
    public float HexagonTileXOddOffset = 0.5f;

    public void CalculateBounds(OuterGridSettings grid)
    {
        GridXStart = (grid.GridXLength - GridXLength) / 2;
        GridXEnd = GridXStart + GridXLength;

        GridZStart = (grid.GridZLength - GridZLength) / 2;
        GridZEnd = GridZStart + GridZLength;
    }
}
