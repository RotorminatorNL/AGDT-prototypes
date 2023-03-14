using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSystemV3))]
public class GridSystemV3Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridSystemV3 gridSystem = (GridSystemV3)target;

        if (gridSystem.HexTileTypes == null)
        {
            gridSystem.Tiles = new List<HexagonTileTypeChance>();
        }
        else
        {
            List<HexagonTileType> newTiles = gridSystem.HexTileTypes.Types;

            if (IsTileListUpdated(gridSystem.Tiles, newTiles))
            {
                gridSystem.Tiles = new List<HexagonTileTypeChance>();
                foreach (HexagonTileType tile in newTiles)
                {
                    gridSystem.Tiles.Add(new HexagonTileTypeChance(tile.Name));
                }
            }
        }

        if (GUILayout.Button("Forced generation")) gridSystem.GenerateGrid();
    }

    private bool IsTileListUpdated(List<HexagonTileTypeChance> oldTiles, List<HexagonTileType> newTiles)
    {
        if (oldTiles.Count != newTiles.Count)
            return true;

        return false;
    }
}
