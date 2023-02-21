using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class GridSystemV3 : MonoBehaviour
{
    private Mesh mesh;

    private Vector3[] vertices;
    private int[] allTrianglePoints;

    [Header("Grid size")]
    [SerializeField] private int gridXAxisLength = 30;
    [SerializeField] private int gridZAxisLength = 20;

    [Header("Perlin Noise X coord settings")]
    [SerializeField] private float perlinNoiseXCoordOffset = 0f;
    [SerializeField, Range(0.01f, 1f)] private float perlinNoiseXScale = 0.1f;

    [Header("Perlin Noise Z coord settings")]
    [SerializeField] private float perlinNoiseZCoordOffset = 0f;
    [SerializeField, Range(0.01f, 1f)] private float perlinNoiseZScale = 0.1f;

    [Header("Perin Noise Y scale")]
    [SerializeField, Range(0, 30)] private float perlinNoiseYScale = 10f;

    private void Update()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    private void CreateShape()
    {
        vertices = new Vector3[(gridXAxisLength + 1) * (gridZAxisLength + 1)];

        for (int i = 0, z = 0; z <= gridZAxisLength; z++)
        {
            for (int x = 0; x <= gridXAxisLength; x++)
            {
                float perlinNoiseXCoord = x * perlinNoiseXScale + perlinNoiseXCoordOffset;
                float perlinNoiseYCoord = z * perlinNoiseZScale + perlinNoiseZCoordOffset;
                float y = Mathf.PerlinNoise(perlinNoiseXCoord, perlinNoiseYCoord) * perlinNoiseYScale;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        allTrianglePoints = new int[gridXAxisLength * gridZAxisLength * 6];

        int currentVert = 0;
        int currentSqaure = 0;

        for (int z = 0; z < gridZAxisLength; z++)
        {
            for (int x = 0; x < gridXAxisLength; x++)
            {
                allTrianglePoints[currentSqaure * 6 + 0] = currentVert + 0;
                allTrianglePoints[currentSqaure * 6 + 1] = currentVert + gridXAxisLength + 1;
                allTrianglePoints[currentSqaure * 6 + 2] = currentVert + 1;
                allTrianglePoints[currentSqaure * 6 + 3] = currentVert + 1;
                allTrianglePoints[currentSqaure * 6 + 4] = currentVert + gridXAxisLength + 1;
                allTrianglePoints[currentSqaure * 6 + 5] = currentVert + gridXAxisLength + 2;

                currentVert++;
                currentSqaure++;
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
