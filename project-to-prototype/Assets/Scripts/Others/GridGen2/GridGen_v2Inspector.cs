using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridGen_v2))]
public class GridGen_v2Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridGen_v2 gridGen_V2 = (GridGen_v2)target;
        GUILayout.Space(15);
        if (GUILayout.Button("Generate grid")) gridGen_V2.GenerateGrid();
        if (GUILayout.Button("Delete grid")) gridGen_V2.DeleteGrid();
    }
}
