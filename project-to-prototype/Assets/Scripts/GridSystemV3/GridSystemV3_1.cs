using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class GridSystemV3_1 : MonoBehaviour
{
    [SerializeField] private GridSettings grid;
    [SerializeField] private PerlinNoiseV2 perlinNoise;
    [SerializeField] private float heightScale = 1;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] allTrianglePoints;
    private float[,] noiseMap;

    private void Update()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        noiseMap = perlinNoise.GenerateNoiseMap(grid);
        UpdateVertices();
        UpdateTrianglesPoints();
        UpdateMesh();
    }

    private void UpdateVertices()
    {
        vertices = new Vector3[(grid.GridXLength + 1) * (grid.GridZLength + 1)];

        for (int i = 0, z = 0; z <= grid.GridZLength; z++)
        {
            for (int x = 0; x <= grid.GridXLength; x++)
            {
                vertices[i] = new Vector3(x, noiseMap[x,z] * heightScale, z);
                i++;
            }
        }
    }

    private void UpdateTrianglesPoints()
    {
        allTrianglePoints = new int[grid.GridXLength * grid.GridZLength * 6];

        int currentVert = 0;
        int currentSquare = 0;

        for (int z = 0; z < grid.GridZLength; z++)
        {
            for (int x = 0; x < grid.GridXLength; x++)
            {
                allTrianglePoints[currentSquare * 6 + 0] = currentVert + 0;
                allTrianglePoints[currentSquare * 6 + 1] = currentVert + grid.GridXLength + 1;
                allTrianglePoints[currentSquare * 6 + 2] = currentVert + 1;
                allTrianglePoints[currentSquare * 6 + 3] = currentVert + 1;
                allTrianglePoints[currentSquare * 6 + 4] = currentVert + grid.GridXLength + 1;
                allTrianglePoints[currentSquare * 6 + 5] = currentVert + grid.GridXLength + 2;

                currentVert++;
                currentSquare++;
            }
            currentVert++;
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = allTrianglePoints;

        mesh.RecalculateNormals();
    }
}
