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

    public void CalculateBounds(InnerGridSettings innerGrid)
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
        float percentage = 1f;

        if ((gridXLeftStart > x || gridXRightEnd < x) && (gridZBottomEnd > z || gridZTopEnd < z)) return 1;

        if (gridXLeftStart < x && gridXLeftEnd >= x) 
        {
            float xPercentage = (float)(gridXLeftEnd - x) / Length;
            float zBottomPercentage = (float)(gridZBottomEnd - z) / Length;
            float zTopPercentage = (float)(z - gridZTopStart) / Length;

            if (gridZBottomEnd < z && gridZTopStart > z) return xPercentage;
            if (gridZBottomStart < z && gridZBottomEnd >= z) return xPercentage >= zBottomPercentage ? xPercentage : zBottomPercentage;
            if (gridZTopStart <= z && gridZTopEnd > z) return xPercentage >= zTopPercentage ? xPercentage : zTopPercentage;
        }
        else if (gridXRightStart <= x && gridXRightEnd > x)
        {
            float xPercentage = (float)(x - gridXRightStart) / Length;
            float zBottomPercentage = (float)(gridZBottomEnd - z) / Length;
            float zTopPercentage = (float)(z - gridZTopStart) / Length;

            if (gridZBottomEnd < z && gridZTopStart > z) return xPercentage;
            if (gridZBottomStart < z && gridZBottomEnd >= z) return xPercentage >= zBottomPercentage ? xPercentage : zBottomPercentage;
            if (gridZTopStart <= z && gridZTopEnd > z) return xPercentage >= zTopPercentage ? xPercentage : zTopPercentage;
        }
        else if (gridZBottomStart < z && gridZBottomEnd > z)
        {
            if (gridXLeftEnd < x && gridXRightStart > x) return (float)(gridZBottomEnd - z) / Length;
        }
        else if (gridZTopStart < z && gridZTopEnd > z)
        {
            if (gridXLeftEnd < x && gridXRightStart > x) return (float)(z - gridZTopStart) / Length;
        }
        return percentage;
    }
}
