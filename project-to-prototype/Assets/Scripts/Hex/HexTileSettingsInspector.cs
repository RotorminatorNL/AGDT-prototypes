using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

[CustomEditor(typeof(HexTileSettings))]
public class HexTileSettingsInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        HexTileSettings hexTileSettings = (HexTileSettings)target;
        string[] tileTypes = hexTileSettings.HexTileTypes.TileTypes.Select(i => i.TileName).ToArray();
        hexTileSettings.SelectedTileTypeIndex = EditorGUILayout.Popup("Tile types", hexTileSettings.SelectedTileTypeIndex, tileTypes);
    }
}
