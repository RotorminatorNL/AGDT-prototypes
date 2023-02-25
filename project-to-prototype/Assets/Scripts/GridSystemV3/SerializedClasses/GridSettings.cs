using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridSettings
{
    public int GridXLength = 300;
    public int GridZLength = 250;

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
