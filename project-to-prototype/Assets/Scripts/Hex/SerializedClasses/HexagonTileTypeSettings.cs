using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class HexagonTileTypeSettings
{
    public string Name;
    public bool NoOverrideNextGen = false;
    [Space(5)]
    public bool OuterGrid;
    public bool InnerGrid;
    [Range(0f, 1f)] public float MaxHeight = 1f;

    public HexagonTileTypeSettings(string name)
    {
        Name = name;
    }
}