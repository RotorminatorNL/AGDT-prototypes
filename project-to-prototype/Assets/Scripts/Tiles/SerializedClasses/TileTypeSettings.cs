using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class TileTypeSettings
{
    public string Name;
    public bool SkipNextGen = false;
    [Space(5)]
    public bool OuterGrid = true;
    public bool InnerGrid = true;
    public float HeightOffset;
    [Range(0f, 1f)] public float MaxHeightPercent = 1f;

    public TileTypeSettings(string name)
    {
        Name = name;
    }

    public bool IsHeightBelowMaxHeight(float currentHeight, float minGridHeight, float maxGridHeight)
    {
        return currentHeight <= minGridHeight + ((maxGridHeight - minGridHeight) * MaxHeightPercent);
    }
}