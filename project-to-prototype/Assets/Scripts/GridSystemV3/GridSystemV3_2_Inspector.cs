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

        gridSystem.TileTypeSettings = GetTileTypeSettingsList(gridSystem.TileTypeSettings, gridSystem.TileTypes);

        EditorGUILayout.Space(5);
        if (GUILayout.Button("(Re)Generate grid")) gridSystem.ActivateGenerateGrid();
        EditorGUILayout.Space(2);
        if (GUILayout.Button("Clear grid")) gridSystem.ClearGrid();
    }

    private List<TileTypeSettings> GetTileTypeSettingsList(List<TileTypeSettings> oldTileTypes, TileTypeCollection newTileTypes)
    {
        List<TileTypeSettings> temp = new List<TileTypeSettings>();

        if (newTileTypes != null)
        {
            if (oldTileTypes.Count != newTileTypes.Types.Count) foreach (TileType tile in newTileTypes.Types) temp.Add(new TileTypeSettings(tile.Name));
            else temp = oldTileTypes;
        }

        return temp;
    }
}
