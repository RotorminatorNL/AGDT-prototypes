using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class HexagonTileTypeChance
{
    public string Name;
    [Range(0f, 1f)] public float Chance = 1f;

    public HexagonTileTypeChance(string name)
    {
        Name = name;
    }
}
