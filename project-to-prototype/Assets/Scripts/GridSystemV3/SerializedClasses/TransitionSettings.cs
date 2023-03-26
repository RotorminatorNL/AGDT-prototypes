using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransitionSettings
{
    public int Length = 10;
    [Range(0f, 10f)] public float Curve = 1f; // currently out of use
    [HideInInspector] public Dictionary<int, float> XHexagons;
    [HideInInspector] public Dictionary<int, float> ZHexagons;

    private int gridXLeftStart;
    private int gridXLeftEnd;
    private int gridXRightStart;
    private int gridXRightEnd;
    private int gridZBottomStart;
    private int gridZBottomEnd;
    private int gridZTopStart;
    private int gridZTopEnd;

    public void CalculateBorders(InnerGridSettings innerGrid)
    {
        gridXLeftStart = innerGrid.GridXStart - Length;
        gridXLeftEnd = innerGrid.GridXStart;
        gridXRightStart = innerGrid.GridXEnd;
        gridXRightEnd = innerGrid.GridXEnd + Length;

        gridZBottomStart = innerGrid.GridZStart - Length;
        gridZBottomEnd = innerGrid.GridZStart;
        gridZTopStart = innerGrid.GridZEnd;
        gridZTopEnd = innerGrid.GridZEnd + Length;
    }

    public float GetTransitionPercentage(int x, int z)
    {
        if ((gridXLeftStart > x || gridXRightEnd < x) && (gridZBottomEnd > z || gridZTopEnd < z)) return 1;

        float xPercentage = 1f;
        float zBottomPercentage = (float)(gridZBottomEnd - z) / Length;
        float zTopPercentage = (float)(z - gridZTopStart) / Length;

        if (gridXLeftStart < x && gridXLeftEnd >= x) xPercentage = (float)(gridXLeftEnd - x) / Length;
        else if (gridXRightStart <= x && gridXRightEnd > x) xPercentage = (float)(x - gridXRightStart) / Length;
        else if (gridZBottomStart < z && gridZBottomEnd > z && gridXLeftEnd < x && gridXRightStart > x) return zBottomPercentage;
        else if (gridZTopStart < z && gridZTopEnd > z && gridXLeftEnd < x && gridXRightStart > x) return zTopPercentage;

        if (gridZBottomEnd < z && gridZTopStart > z) return xPercentage;
        if (gridZBottomStart < z && gridZBottomEnd >= z) return xPercentage >= zBottomPercentage ? xPercentage : zBottomPercentage;
        if (gridZTopStart <= z && gridZTopEnd > z) return xPercentage >= zTopPercentage ? xPercentage : zTopPercentage;

        return 1;
    }
}
