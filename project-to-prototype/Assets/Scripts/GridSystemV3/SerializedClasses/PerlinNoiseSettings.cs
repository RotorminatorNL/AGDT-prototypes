using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PerlinNoiseSettings
{
    public float PerlinNoiseXCoordOffset = 0f;
    [Range(0.01f, 1f)] public float PerlinNoiseXScale = 0.05f;

    [Space(10)]
    public float PerlinNoiseZCoordOffset = 0f;
    [Range(0.01f, 1f)] public float PerlinNoiseZScale = 0.05f;

    [Space(10)]
    [Range(0, 30)] public float PerlinNoiseYScale = 10f;
}
