using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSystem))]
public class GridSystemInspectorButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridSystem gridSystem = (GridSystem)target;

        if (gridSystem.HexTileTypes == null)
        {
            gridSystem.Tiles = new List<HexagonTileTypeChance>();
        }
        else
        {
            List<HexagonTileType> newTiles = gridSystem.HexTileTypes.Types;

            if (IsShownTileListtUpToDate(gridSystem.Tiles, newTiles))
            {
                gridSystem.Tiles = new List<HexagonTileTypeChance>();
                foreach (HexagonTileType tile in newTiles)
                {
                    gridSystem.Tiles.Add(new HexagonTileTypeChance(tile.Name));
                }
            }
        }

        if (GUILayout.Button("(Re)Generate grid")) gridSystem.GenerateHexGrid();
        if (GUILayout.Button("Clear grid")) gridSystem.ClearHexGrid();
    }

    private bool IsShownTileListtUpToDate(List<HexagonTileTypeChance> shownTiles, List<HexagonTileType> newTiles)
    {
        if (shownTiles.Count != newTiles.Count)
            return true;

        return false;
    }
}
