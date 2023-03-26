using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

[System.Serializable]
public class OuterGridSettings
{
    [Header("Recommended: 6.400 (example: 80 * 80) or less")]
    [Min(1)] public int GridXLength = 50;
    [Min(1)] public int GridZLength = 50;
}
