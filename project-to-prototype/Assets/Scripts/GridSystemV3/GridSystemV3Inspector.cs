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
            gridSystem.Tiles = new List<TileGenerationChance>();
        }
        else
        {
            List<Tile> newTiles = gridSystem.HexTileTypes.TileTypes;

            if (IsTileListUpdated(gridSystem.Tiles, newTiles))
            {
                gridSystem.Tiles = new List<TileGenerationChance>();
                foreach (Tile tile in newTiles)
                {
                    gridSystem.Tiles.Add(new TileGenerationChance(tile.TileName));
                }
            }
        }

        if (GUILayout.Button("Forced generation")) gridSystem.GenerateGrid();
    }

    private bool IsTileListUpdated(List<TileGenerationChance> oldTiles, List<Tile> newTiles)
    {
        if (oldTiles.Count != newTiles.Count)
            return true;

        return false;
    }
}
