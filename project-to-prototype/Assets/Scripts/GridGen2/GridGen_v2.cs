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
    [SerializeField] private int chunkXLength = 10;
    [SerializeField] private int chunkYLength = 10;
    [SerializeField] private int chunkZLength = 10;

    [Header("Grid size (width * height * depth) generated with chunks")]
    [SerializeField] private int gridXLength = 1;
    [SerializeField] private int gridYLength = 1;
    [SerializeField] private int gridZLength = 1;

    public void GenerateGrid()
    {
        DeleteGrid();
        for (int x = 0; x < gridXLength; x++)
        {
            for (int y = 0; y < gridYLength; y++)
            {
                for (int z = 0; z < gridZLength; z++)
                {
                    GeneratingChunk(x * chunkXLength, y * chunkYLength, z * chunkZLength);
                }
            }
        }
    }

    private void GeneratingChunk(int xStartPos, int yStartPos, int zStartPos)
    {
        for (int x = 0; x < chunkXLength; x++)
        {
            for (int y = 0; y < chunkYLength; y++)
            {
                for (int z = 0; z < chunkZLength; z++)
                {
                    Instantiate(cubePrefab, new Vector3(x + xStartPos, y + yStartPos, z + zStartPos), Quaternion.identity, gridParent);
                }
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
