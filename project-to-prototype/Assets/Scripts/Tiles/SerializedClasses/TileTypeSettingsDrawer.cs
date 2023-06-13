using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TileTypeSettings))]
public class TileTypeSettingsDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.indentLevel = 0;

        string[] properties = new string[] { "SkipNextGen", "OuterGrid", "InnerGrid", "HeightOffset", "MaxHeightPercent" };
        string[] propertyLabels = new string[] { "Skip next gen", "Outer grid", "Inner grid", "Height offset", "Max height percent" };

        float xPos = 70;
        float yPos = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        float width = Screen.width - 95;
        float height = 15 + EditorGUIUtility.standardVerticalSpacing;

        Rect foldPos = new Rect(position.x, position.y - (position.height / 2) + 8, position.width, position.height);

        property.isExpanded = EditorGUI.Foldout(foldPos, property.isExpanded, property.FindPropertyRelative("Name").stringValue);
        if (property.isExpanded)
        {
            Rect skipNextGenPos = new(xPos, position.y + yPos, width, height);
            EditorGUI.PropertyField(skipNextGenPos, property.FindPropertyRelative(properties[0]), new GUIContent(propertyLabels[0]));

            Rect outerGridPos = new(xPos, position.y + (yPos * 2), width, height);
            EditorGUI.PropertyField(outerGridPos, property.FindPropertyRelative(properties[1]), new GUIContent(propertyLabels[1]));

            Rect innerGridPos = new(position.x + (width / 2), position.y + (yPos * 2), width, height);
            EditorGUI.PropertyField(innerGridPos, property.FindPropertyRelative(properties[2]), new GUIContent(propertyLabels[2]));

            Rect heightOffsetPos = new(xPos, position.y + (yPos * 3), width, height);
            EditorGUI.PropertyField(heightOffsetPos, property.FindPropertyRelative(properties[3]), new GUIContent(propertyLabels[3]));

            Rect maxHeightPercentPos = new(xPos, position.y + (yPos * 4), width, height);
            EditorGUI.PropertyField(maxHeightPercentPos, property.FindPropertyRelative(properties[4]), new GUIContent(propertyLabels[4]));
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            int lineCount = 5;
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * lineCount;
        }
        else
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}
