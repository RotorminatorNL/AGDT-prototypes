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

        gridSystem.OuterGrid.HexagonTiles = GetTileTypeChanceList(gridSystem.OuterGrid.HexagonTiles, gridSystem.OuterGrid.HexagonTileTypes);
        gridSystem.InnerGrid.HexagonTiles = GetTileTypeChanceList(gridSystem.InnerGrid.HexagonTiles, gridSystem.InnerGrid.HexagonTileTypes);

        if (GUILayout.Button("(Re)Generate grid")) gridSystem.GenerateGrid();
        if (GUILayout.Button("Clear grid")) gridSystem.ClearGrid();

    }

    private List<HexagonTileTypeChance> GetTileTypeChanceList(List<HexagonTileTypeChance> oldTileTypes, HexagonTileTypes newTileTypes)
    {
        List<HexagonTileTypeChance> temp = new List<HexagonTileTypeChance>();

        if (newTileTypes != null && oldTileTypes.Count != newTileTypes.Types.Count) foreach (HexagonTileType tile in newTileTypes.Types) temp.Add(new HexagonTileTypeChance(tile.Name));
        else temp = oldTileTypes;

        return temp;
    }
}
