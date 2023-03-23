using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

[CustomEditor(typeof(HexagonTileSettings))]
public class HexagonTileSettingsInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HexagonTileSettings hexagonTileSettings = (HexagonTileSettings)target;
        HexagonTileTypes tileTypes = hexagonTileSettings.transform.parent.GetComponent<GridSystemV3_2>().TileTypes;

        if (tileTypes == null) return;
        hexagonTileSettings.TileTypes = tileTypes;
        string[] hexagonTiles = hexagonTileSettings.TileTypes.Types.Select(i => i.Name).ToArray();
        hexagonTileSettings.SelectedTileTypeIndex = EditorGUILayout.Popup("Hexagon tiles", hexagonTileSettings.SelectedTileTypeIndex, hexagonTiles);
    }
}
