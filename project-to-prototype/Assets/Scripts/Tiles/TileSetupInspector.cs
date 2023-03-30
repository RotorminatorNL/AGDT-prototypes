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
    bool tileTypeIndexChanged = false;

    public override void OnInspectorGUI()
    {
        if (targets.Length == 1) DrawDefaultInspector();

        TileSetup tileSetupTarget = (TileSetup)target;
        TileTypeCollection tileTypes = tileSetupTarget.TileTypes;

        if (tileTypes == null) return;
        tileSetupTarget.TileTypes = tileTypes;
        string[] tileTypeNames = tileSetupTarget.TileTypes.Types.Select(i => i.Name).ToArray();
        int selectedTileTypeIndex = EditorGUILayout.Popup("Tile types", tileSetupTarget.SelectedTileTypeIndex, tileTypeNames);

        if (tileSetupTarget.TileTypeIndexChanged(selectedTileTypeIndex)) tileTypeIndexChanged = true;
        if (tileTypeIndexChanged)
        {
            if (targets.Length == 1)
            {
                tileSetupTarget.SetTileType(selectedTileTypeIndex);
                tileSetupTarget.UpdateTile();
            }
            if (targets.Length > 1)
            {
                foreach (Object gameObject in targets)
                {
                    Debug.Log("sweg");
                    TileSetup tileSetup = (TileSetup)gameObject;
                    tileSetup.SetTileType(selectedTileTypeIndex);
                    tileSetup.UpdateTile();
                }
            }
            tileTypeIndexChanged = false;
        }
    }
}