using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

[CustomEditor(typeof(TileSetup))]
public class TileSetupInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileSetup tileSetup = (TileSetup)target;
        TileTypeCollection tileTypes = tileSetup.TileTypes;

        if (tileTypes == null) return;
        tileSetup.TileTypes = tileTypes;
        string[] tileTypeNames = tileSetup.TileTypes.Types.Select(i => i.Name).ToArray();
        tileSetup.SelectedTileTypeIndex = EditorGUILayout.Popup("Hexagon tiles", tileSetup.SelectedTileTypeIndex, tileTypeNames);
    }
}