using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(TestSO))]
public class MyScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var testOS = (TestSO)target;

        testOS.test = (TestSO.Tests)EditorGUILayout.EnumPopup("Test type", testOS.test);

        testOS.range = EditorGUILayout.FloatField("Range", testOS.range);
        testOS.fireRate = EditorGUILayout.FloatField("Fire rate", testOS.fireRate);

        switch (testOS.test)
        {
            case TestSO.Tests.Test1:
                testOS.towerCost = EditorGUILayout.IntField("Tower cost", testOS.towerCost);
                break;
            case TestSO.Tests.Test2:
                testOS.sellValue = EditorGUILayout.IntField("Sell value", testOS.sellValue);
                break;
        }
    }
}
