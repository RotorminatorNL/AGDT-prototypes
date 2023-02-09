using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSystem))]
public class GridSystemInspectorButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridSystem gridSystem = (GridSystem)target;
        if (GUILayout.Button("(Re)Generate grid")) gridSystem.GenerateHexGrid();
        if (GUILayout.Button("Clear grid")) gridSystem.ClearHexGrid();
    }
}
