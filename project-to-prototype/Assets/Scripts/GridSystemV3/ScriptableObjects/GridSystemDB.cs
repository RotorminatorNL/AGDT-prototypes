using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridSystemDB", menuName = "Grid system/Database")]
public class GridSystemDB : ScriptableObject
{
    public IReadOnlyList<GridTileInfo> Tiles { get { return tiles; } }
    private List<GridTileInfo> tiles;
    private int currentXLength;
    private int currentZLength;

    public void StoreTile(string name, TileSetup tileSettings, float height, bool outerGrid, bool innerGrid)
    {
        tiles ??= new List<GridTileInfo>();
        tiles.Add(new GridTileInfo(name, tileSettings, height, outerGrid, innerGrid));
    }

    public void ClearAllTiles()
    {
        tiles = new List<GridTileInfo>();
    }
}

public class GridTileInfo
{
    public string Name;
    public TileSetup TileSettings;
    public float Height;
    public bool OuterGrid;
    public bool InnerGrid;

    public GridTileInfo(string name, TileSetup tileSettings, float height, bool outerGrid, bool innerGrid)
    {
        Name = name;
        TileSettings = tileSettings;
        Height = height;
        OuterGrid = outerGrid;
        InnerGrid = innerGrid;
    }
}
