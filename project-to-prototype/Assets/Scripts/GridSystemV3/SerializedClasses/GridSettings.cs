using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridSettings
{
    public int GridXLength = 100;
    public int GridZLength = 80;

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
