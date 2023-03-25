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

        if (GUILayout.Button("(Re)Generate grid")) gridSystem.ActivateGenerateGrid();
        if (GUILayout.Button("Clear grid")) gridSystem.ClearGrid();

    }

    private List<HexagonTileTypeSettings> GetTileTypeChanceList(List<HexagonTileTypeSettings> oldTileTypes, HexagonTileTypes newTileTypes)
    {
        List<HexagonTileTypeSettings> temp = new List<HexagonTileTypeSettings>();

        if (newTileTypes != null && oldTileTypes.Count != newTileTypes.Types.Count) foreach (HexagonTileType tile in newTileTypes.Types) temp.Add(new HexagonTileTypeSettings(tile.Name));
        else temp = oldTileTypes;

        return temp;
    }
}
