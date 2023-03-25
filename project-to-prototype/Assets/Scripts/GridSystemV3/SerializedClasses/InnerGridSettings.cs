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

    public void CalculateBounds(OuterGridSettings grid)
    {
        GridXStart = (grid.GridXLength - GridXLength) / 2;
        GridXEnd = GridXStart + GridXLength;

        GridZStart = (grid.GridZLength - GridZLength) / 2;
        GridZEnd = GridZStart + GridZLength;
    }

    public bool IsInside(int x, int z)
    {
        return x >= GridXStart && x <= GridXEnd && z >= GridZStart && z <= GridZEnd;
    }
}
