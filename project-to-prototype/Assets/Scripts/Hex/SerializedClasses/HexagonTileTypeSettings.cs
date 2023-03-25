using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class HexagonTileTypeSettings
{
    public string Name;
    [Range(0f, 1f)] public float Height = 1f;

    public HexagonTileTypeSettings(string name)
    {
        Name = name;
    }
}
