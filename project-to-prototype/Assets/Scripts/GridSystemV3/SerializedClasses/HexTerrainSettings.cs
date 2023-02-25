using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexTerrainSettings
{
    public int HexTerrainXLength = 150;
    [HideInInspector] public int HexTerrainXStart;
    [HideInInspector] public int HexTerrainXEnd;
    public int HexTerrainZLength = 100;
    [HideInInspector] public int HexTerrainZStart;
    [HideInInspector] public int HexTerrainZEnd;
}
