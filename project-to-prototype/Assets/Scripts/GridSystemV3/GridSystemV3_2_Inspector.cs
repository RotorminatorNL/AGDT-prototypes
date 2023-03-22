using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSystemV3_2))]
public class GridSystemV3_2_Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridSystemV3_2 gridSystem = (GridSystemV3_2)target;

        if (gridSystem.TileTypes == null)
        {
            gridSystem.HexagonTiles = new List<HexagonTileTypeChance>();
        }
        else
        {
            List<HexagonTileType> newTiles = gridSystem.TileTypes.Types;

            if (IsTileListUpdated(gridSystem.HexagonTiles, newTiles))
            {
                gridSystem.HexagonTiles = new List<HexagonTileTypeChance>();
                foreach (HexagonTileType tile in newTiles) gridSystem.HexagonTiles.Add(new HexagonTileTypeChance(tile.Name));
            }
        }

        if (GUILayout.Button("(Re)Generate grid")) gridSystem.GenerateGrid();
        if (GUILayout.Button("Clear grid")) gridSystem.ClearGrid();

    }

    private bool IsTileListUpdated(List<HexagonTileTypeChance> oldTiles, List<HexagonTileType> newTiles)
    {
        if (oldTiles.Count != newTiles.Count)
            return true;

        return false;
    }
}
