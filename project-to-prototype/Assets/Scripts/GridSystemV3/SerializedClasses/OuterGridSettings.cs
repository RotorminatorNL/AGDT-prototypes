using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OuterGridSettings
{
    [Header("Recommended: 6.400 (example: 80 * 80) or less")]
    [Min(1)] public int GridXLength = 80;
    [Min(1)] public int GridZLength = 80;
}
