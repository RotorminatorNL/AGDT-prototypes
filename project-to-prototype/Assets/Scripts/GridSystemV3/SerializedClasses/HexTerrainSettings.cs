using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexTerrainSettings
{
    public int HexTerrainXLength = 150;
    [HideInInspector] public int HexTerrainXStart { get; private set; }
    [HideInInspector] public int HexTerrainXEnd { get; private set; }

    public int HexTerrainZLength = 100;
    [HideInInspector] public int HexTerrainZStart { get; private set; }
    [HideInInspector] public int HexTerrainZEnd { get; private set; }

    private int previousHexTerrainXLength;
    private int previousHexTerrainZLength; 
    
    public void UpdateValues()
    {
        previousHexTerrainXLength = HexTerrainXLength;
        previousHexTerrainZLength = HexTerrainZLength;
    }

    public bool HasValueChanged()
    {
        if (previousHexTerrainXLength == HexTerrainXLength && previousHexTerrainZLength == HexTerrainZLength) return false;

        previousHexTerrainXLength = HexTerrainXLength;
        previousHexTerrainZLength = HexTerrainZLength;
        return true;
    }

    public void CalculateHexTerrainBounds(GridSettings gridSettings)
    {
        HexTerrainXStart = (gridSettings.GridXLength - HexTerrainXLength) / 2;
        HexTerrainXEnd = HexTerrainXStart + HexTerrainXLength;

        HexTerrainZStart = (gridSettings.GridZLength - HexTerrainZLength) / 2;
        HexTerrainZEnd = HexTerrainZStart + HexTerrainZLength;
    }
}
