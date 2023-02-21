using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

[ExecuteInEditMode]
public class GridGen_v2 : MonoBehaviour
{
    [Header("Chunk prerequisites")]
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Transform gridParent;

    [Header("Chunk size (width * height * depth)")]
    [SerializeField] private float chunkXLength = 10;
    [SerializeField] private float chunkYLength = 10;
    [SerializeField] private float chunkZLength = 10;

    [Header("Grid size (width * height * depth) generated with chunks")]
    [SerializeField] private float gridXLength = 1;
    [SerializeField] private float gridYLength = 1;
    [SerializeField] private float gridZLength = 1;

    public void GenerateGrid()
    {
        for (float x = 0; x < gridXLength; x++)
        {
            for (float z = 0; z < gridZLength; z++)
            {
                Debug.Log(Mathf.PerlinNoise(x / .3f, z / .3f) * 2f);
            }
        }
    }

    public void DeleteGrid()
    {
        while (gridParent.childCount > 0)
        {
            DestroyImmediate(gridParent.GetChild(0).gameObject);
        }
    }
}
