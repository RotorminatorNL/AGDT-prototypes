using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

[CustomEditor(typeof(TileSetup))]
[CanEditMultipleObjects]
public class TileSetupInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (targets.Length == 1) DrawDefaultInspector();

        TileSetup tileSetupTarget = (TileSetup)target;
        TileTypeCollection tileTypes = tileSetupTarget.TileTypes;

        if (tileTypes == null) return;
        tileSetupTarget.TileTypes = tileTypes;
        string[] tileTypeNames = tileSetupTarget.TileTypes.Types.Select(i => i.Name).ToArray();
        tileSetupTarget.SelectedTileTypeIndex = EditorGUILayout.Popup("Hexagon tiles", tileSetupTarget.SelectedTileTypeIndex, tileTypeNames);

        if (targets.Length > 1 && tileSetupTarget.TileTypeIndexChanged())
        {
            foreach (Object tileSetup in targets) tileSetup.GetComponent<TileSetup>().SelectedTileTypeIndex = tileSetupTarget.SelectedTileTypeIndex;
        }
    }
}