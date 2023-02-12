using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class TileGenerationChance
{
    public string TileName;
    [Range(0f, 1f)] public float TileChance = 1f;


    public TileGenerationChance(string tileName)
    {
        TileName = tileName;
    }
}
