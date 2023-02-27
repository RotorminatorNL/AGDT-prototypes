using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class PerlinNoiseV2
{
    public Vector2 Offset;
    public int Seed;
    [Min(0.0001f)] public float NoiseScale = 0.0001f;
    [Min(1)] public int Octaves = 1;
    [Range(0, 1)] public float Persistence = 0;
    [Min(1)] public float Lacunarity = 1;

    public float[,] GenerateNoiseMap(GridSettings grid)
    {
        float[,] noiseMap = new float[grid.GridXLength + 1, grid.GridZLength + 1];

        System.Random rnd = new System.Random(Seed);
        Vector2[] octaveOffsets = new Vector2[Octaves];
        for (int i = 0; i < Octaves; i++)
        {
            float offsetX = rnd.Next(-100000, 100000) + Offset.x;
            float offsetZ = rnd.Next(-100000, 100000) + Offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetZ);
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfXlength = grid.GridXLength / 2f;
        float halfZlength = grid.GridZLength / 2f;

        for (int z = 0; z < grid.GridZLength; z++)
        {
            for (int x = 0; x < grid.GridXLength; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < Octaves; i++)
                {
                    float sampleX = (x - halfXlength) / NoiseScale * frequency + octaveOffsets[i].x;
                    float sampleZ = (z - halfZlength) / NoiseScale * frequency + octaveOffsets[i].x;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ);
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= Persistence;
                    frequency *= Lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

                noiseMap[x, z] = noiseHeight;
            }
        }

        for (int z = 0; z < grid.GridZLength + 1; z++)
        {
            for (int x = 0; x < grid.GridXLength + 1; x++)
            {
                noiseMap[x, z] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, z]);
            }
        }

        return noiseMap;
    }
}
