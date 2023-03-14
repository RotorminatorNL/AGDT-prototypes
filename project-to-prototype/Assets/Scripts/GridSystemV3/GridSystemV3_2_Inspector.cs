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

        if (gridSystem.HexagonTerrain.HexagonTileTypes == null)
        {
            gridSystem.HexagonTerrain.HexagonTiles = new List<HexagonTileTypeChance>();
        }
        else
        {
            List<HexagonTileType> newTiles = gridSystem.HexagonTerrain.HexagonTileTypes.Types;

            if (IsTileListUpdated(gridSystem.HexagonTerrain.HexagonTiles, newTiles))
            {
                gridSystem.HexagonTerrain.HexagonTiles = new List<HexagonTileTypeChance>();
                foreach (HexagonTileType tile in newTiles) gridSystem.HexagonTerrain.HexagonTiles.Add(new HexagonTileTypeChance(tile.Name));
            }
        }
    }

    private bool IsTileListUpdated(List<HexagonTileTypeChance> oldTiles, List<HexagonTileType> newTiles)
    {
        if (oldTiles.Count != newTiles.Count)
            return true;

        return false;
    }
}
