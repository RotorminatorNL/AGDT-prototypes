using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridSettings
{
    [Min(1)] public int GridXLength = 100;
    [Min(1)] public int GridZLength = 80;
}
