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
            gridSystem.Tiles = new List<TileGenerationChance>();
        }
        else
        {
            List<Tile> newTiles = gridSystem.HexTileTypes.TileTypes;

            if (IsShownTileListtUpToDate(gridSystem.Tiles, newTiles))
            {
                gridSystem.Tiles = new List<TileGenerationChance>();
                foreach (Tile tile in newTiles)
                {
                    gridSystem.Tiles.Add(new TileGenerationChance(tile.TileName));
                }
            }
        }

        if (GUILayout.Button("(Re)Generate grid")) gridSystem.GenerateHexGrid();
        if (GUILayout.Button("Clear grid")) gridSystem.ClearHexGrid();
    }

    private bool IsShownTileListtUpToDate(List<TileGenerationChance> shownTiles, List<Tile> newTiles)
    {
        if (shownTiles.Count != newTiles.Count)
            return true;

        return false;
    }
}
