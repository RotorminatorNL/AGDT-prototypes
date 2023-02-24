using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class GridSystemV3 : MonoBehaviour
{
    private Mesh mesh;

    private Vector3[] vertices;
    private int[] allTrianglePoints;

    [Header("Grid settings")]
    [SerializeField] private int gridXLength = 300;
    [SerializeField] private int gridZLength = 250;

    [Space(10)]
    [Header("Perlin Noise settings")]
    [SerializeField] private float perlinNoiseXCoordOffset = 0f;
    [SerializeField, Range(0.01f, 1f)] private float perlinNoiseXScale = 0.05f;

    [Space(10)]
    [SerializeField] private float perlinNoiseZCoordOffset = 0f;
    [SerializeField, Range(0.01f, 1f)] private float perlinNoiseZScale = 0.05f;

    [Space(10)]
    [SerializeField, Range(0, 30)] private float perlinNoiseYScale = 10f;

    [Space(10)]
    [Header("Flat terrain settings")]
    [SerializeField] private int flatTerrainXLength = 150;
    private int flatTerrainXStart;
    private int flatTerrainXEnd;
    [SerializeField] private int flatTerrainZLength = 100;
    private int flatTerrainZStart;
    private int flatTerrainZEnd;

    [Space(10)]
    [SerializeField] private int transitionLength = 20;
    [SerializeField, Range(0f, 10f)] private float transitionCurve = 1.5f;
    private Dictionary<int, float> transitionXVertices = new Dictionary<int, float>();
    private Dictionary<int, float> transitionZVertices = new Dictionary<int, float>();

    private void Update()
    {
        if (gridXLength <= 0 || gridZLength <= 0 || flatTerrainXLength <= 0 || flatTerrainZLength <= 0 || transitionLength <= 0) return;
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        CalculateFlatTerrainBounds();
        transitionXVertices = CalculateTransitionBounds(flatTerrainXStart, flatTerrainXEnd, flatTerrainXLength);
        transitionZVertices = CalculateTransitionBounds(flatTerrainZStart, flatTerrainZEnd, flatTerrainZLength);

        UpdateVertices();
        UpdateTrianglesPoints();

        UpdateMesh();
    }

    private void CalculateFlatTerrainBounds()
    {
        flatTerrainXStart = (gridXLength - flatTerrainXLength) / 2;
        flatTerrainXEnd = flatTerrainXStart + flatTerrainXLength;

        flatTerrainZStart = (gridZLength - flatTerrainZLength) / 2;
        flatTerrainZEnd = flatTerrainZStart + flatTerrainZLength;
    }

    private Dictionary<int, float> CalculateTransitionBounds(int innerGridStart, int innerGridEnd, int innerGridLength)
    {
        Dictionary<int, float> transitionVertices = new Dictionary<int, float>();
        bool otherSide = false;
        for (int x = transitionLength, i = innerGridStart - transitionLength; i <= innerGridEnd + transitionLength; i++)
        {
            if (i == innerGridStart)
            {
                i += innerGridLength + 1;
                x = 1;
                otherSide = true;
            }

            float percentage = CalculatePercentage(otherSide == false ? x-- : x++);
            transitionVertices.Add(i, percentage);
        }
        return transitionVertices;
    }

    private float CalculatePercentage(float currentStep)
    {
        if (currentStep == transitionLength) return 1f;
        else if (currentStep == 0f) return 0f;
        else
        {
            float percentage = currentStep / transitionLength;
            percentage = Mathf.Pow(percentage, transitionCurve);
            percentage = Mathf.Lerp(0f, 100f, percentage);
            return percentage / 100f;
        }
    }

    private void UpdateVertices()
    {
        vertices = new Vector3[(gridXLength + 1) * (gridZLength + 1)];

        for (int i = 0, z = 0; z <= gridZLength; z++)
        {
            for (int x = 0; x <= gridXLength; x++)
            {
                vertices[i] = new Vector3(x, SetYValue(x, z), z);
                i++;
            }
        }
    }

    private float SetYValue(int indexOfX, int indexOfZ)
    {
        if (flatTerrainXStart <= indexOfX && flatTerrainXEnd >= indexOfX && flatTerrainZStart <= indexOfZ && flatTerrainZEnd >= indexOfZ) return 0f;

        float perlinNoiseXCoord = indexOfX * perlinNoiseXScale + perlinNoiseXCoordOffset;
        float perlinNoiseZCoord = indexOfZ * perlinNoiseZScale + perlinNoiseZCoordOffset;

        float perlinNoise = Mathf.PerlinNoise(perlinNoiseXCoord, perlinNoiseZCoord) * perlinNoiseYScale;

        perlinNoise *= GetTransitionPercentage(indexOfX, indexOfZ);

        return perlinNoise;
    }

    private float GetTransitionPercentage(int indexOfX, int indexOfZ)
    {
        if (transitionXVertices.ContainsKey(indexOfX) && flatTerrainZStart <= indexOfZ && flatTerrainZEnd >= indexOfZ) return transitionXVertices[indexOfX];
        else if (transitionZVertices.ContainsKey(indexOfZ) && flatTerrainXStart <= indexOfX && flatTerrainXEnd >= indexOfX) return transitionZVertices[indexOfZ];
        else if (transitionXVertices.ContainsKey(indexOfX) && transitionZVertices.ContainsKey(indexOfZ))
        {
            if (indexOfX <= flatTerrainXStart && indexOfZ <= flatTerrainZStart) return (indexOfX - flatTerrainXStart) <= (indexOfZ - flatTerrainZStart) ? transitionXVertices[indexOfX] : transitionZVertices[indexOfZ];
            else if (indexOfX >= flatTerrainXEnd && indexOfZ <= flatTerrainZStart) return (indexOfX - flatTerrainXEnd) >= (flatTerrainZStart - indexOfZ) ? transitionXVertices[indexOfX] : transitionZVertices[indexOfZ];
            else if (indexOfX <= flatTerrainXStart && indexOfZ >= flatTerrainZEnd) return (flatTerrainXStart - indexOfX) >= (indexOfZ - flatTerrainZEnd) ? transitionXVertices[indexOfX] : transitionZVertices[indexOfZ];
            else return (indexOfX - flatTerrainXEnd) >= (indexOfZ - flatTerrainZEnd) ? transitionXVertices[indexOfX] : transitionZVertices[indexOfZ];
        }

        return 1;
    }

    private void UpdateTrianglesPoints()
    {
        allTrianglePoints = new int[gridXLength * gridZLength * 6];

        int currentVert = 0;
        int currentSqaure = 0;

        for (int z = 0; z < gridZLength; z++)
        {
            for (int x = 0; x < gridXLength; x++)
            {
                allTrianglePoints[currentSqaure * 6 + 0] = currentVert + 0;
                allTrianglePoints[currentSqaure * 6 + 1] = currentVert + gridXLength + 1;
                allTrianglePoints[currentSqaure * 6 + 2] = currentVert + 1;
                allTrianglePoints[currentSqaure * 6 + 3] = currentVert + 1;
                allTrianglePoints[currentSqaure * 6 + 4] = currentVert + gridXLength + 1;
                allTrianglePoints[currentSqaure * 6 + 5] = currentVert + gridXLength + 2;

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
