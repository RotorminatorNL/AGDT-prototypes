using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridSystemDB", menuName = "Grid system/Database")]
public class GridSystemDB : ScriptableObject
{
    private int currentXLength;
    private int currentZLength;
    public IReadOnlyList<GridTileInfo> Tiles { get { return tiles; } }
    private List<GridTileInfo> tiles;

    public void StoreGridLengths(int xLength, int zLength)
    {
        currentXLength = xLength;
        currentZLength = zLength;
    }

    public bool CompareGridLengths(int xLength, int zLength)
    {
        if (currentXLength == xLength && currentZLength == zLength) return true;
        return false;
    }

    public void StoreTile(int xPos, int zPos, float height, bool outerGrid, bool innerGrid)
    {
        tiles ??= new List<GridTileInfo>();
        tiles.Add(new GridTileInfo(xPos, zPos, height, outerGrid, innerGrid));
    }

    public void ClearInfo()
    {
        currentXLength = 0;
        currentZLength = 0;
        tiles = new List<GridTileInfo>();
    }
}

[Serializable]
public class GridTileInfo
{
    public int XPos { get; private set; }
    public int ZPos { get; private set; }
    public float Height { get; private set; }
    public bool OuterGrid { get; private set; }
    public bool InnerGrid { get; private set; }

    public GridTileInfo(int xPos, int zPos, float height, bool outerGrid, bool innerGrid)
    {
        XPos = xPos;
        ZPos = zPos;
        Height = height;
        OuterGrid = outerGrid;
        InnerGrid = innerGrid;
    }

    public void UpdateTileInfo(float height, bool outerGrid, bool innerGrid)
    {
        Height = height;
        OuterGrid = outerGrid;
        InnerGrid = innerGrid;
    }
}
