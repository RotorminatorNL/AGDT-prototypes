using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridSettings
{
    [Min(1)] public int GridXLength = 100;
    [Min(1)] public int GridZLength = 80;

    private int previousGridXLength;
    private int previousGridZLength;

    public void UpdateValues()
    {
        previousGridXLength = GridXLength;
        previousGridZLength = GridZLength;
    }

    public bool HasValueChanged()
    {
        if (previousGridXLength == GridXLength && previousGridZLength == GridZLength) return false;

        previousGridXLength = GridXLength;
        previousGridZLength = GridZLength;
        return true;
    }
}
