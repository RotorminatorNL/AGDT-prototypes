using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PerlinNoiseSettings
{
    [SerializeField] private float xCoordOffset = 0f;
    [SerializeField, Range(0.01f, 1f)] private float xScale = 0.05f;

    [Space(10)]
    [SerializeField] private float zCoordOffset = 0f;
    [SerializeField, Range(0.01f, 1f)] private float zScale = 0.05f;

    [Space(10)]
    [SerializeField, Range(0, 30)] private float outerYScale = 10f;
    [SerializeField, Range(0, 30)] private float innerYScale = 10f;

    public float GetPerlinNoiseValue(float x, float z, bool isInnerGrid = false)
    {
        float perlinNoiseXCoord = x * xScale + xCoordOffset;
        float perlinNoiseZCoord = z * zScale + zCoordOffset;

        float scaleToUse = !isInnerGrid ? outerYScale : innerYScale;

        return Mathf.PerlinNoise(perlinNoiseXCoord, perlinNoiseZCoord) * scaleToUse;
    }
}
