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

    [Header("Grid size")]
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
    [Header("Hexagon terrain size")]
    [SerializeField] private int hexTerrainXLength = 150;
    private int hexTerrainXStart;
    private int hexTerrainXEnd;
    [SerializeField] private int hexTerrainZLength = 100;
    private int hexTerrainZStart;
    private int hexTerrainZEnd;

    [Space(10)]
    [Header("Transition settings")]
    [SerializeField] private int transitionLength = 20;
    [SerializeField, Range(0f, 10f)] private float transitionCurve = 1.5f;
    private Dictionary<int, float> transitionXVertices = new Dictionary<int, float>();
    private Dictionary<int, float> transitionZVertices = new Dictionary<int, float>();

    // Variable to check for change
    private int previousGridXLength;
    private int previousGridZLength;
    private float previousPerlinNoiseXCoordOffset;
    private float perviousPerlinNoiseXScale;
    private float previousPerlinNoiseZCoordOffset;
    private float previousPerlinNoiseZScale;
    private float previousPerlinNoiseYScale;
    private int previousHexTerrainXLength;
    private int previousHexTerrainZLength;
    private int previousTransitionLength;
    private float previousTransitionCurve;

    private void Update()
    {
        // Prevent generation if value <= 0
        if (gridXLength <= 0 || gridZLength <= 0 || hexTerrainXLength <= 0 || hexTerrainZLength <= 0 || transitionLength <= 0) return;

        // No change, no generation
        if (IsValueChanged()) GenerateGrid();
    }

    private bool IsValueChanged()
    {
        // Check for changes
        if (previousGridXLength == gridXLength &&
            previousGridZLength == gridZLength &&
            previousPerlinNoiseXCoordOffset == perlinNoiseXCoordOffset &&
            perviousPerlinNoiseXScale == perlinNoiseXScale &&
            previousPerlinNoiseZCoordOffset == perlinNoiseZCoordOffset &&
            previousPerlinNoiseZScale == perlinNoiseZScale &&
            previousPerlinNoiseYScale == perlinNoiseYScale &&
            previousHexTerrainXLength == hexTerrainXLength &&
            previousHexTerrainZLength == hexTerrainZLength &&
            previousTransitionLength == transitionLength &&
            previousTransitionCurve == transitionCurve) return false;

        // Change detected -> store changes
        previousGridXLength = gridXLength;
        previousGridZLength = gridZLength;
        previousPerlinNoiseXCoordOffset = perlinNoiseXCoordOffset;
        perviousPerlinNoiseXScale = perlinNoiseXScale;
        previousPerlinNoiseZCoordOffset = perlinNoiseZCoordOffset;
        previousPerlinNoiseZScale = perlinNoiseZScale;
        previousPerlinNoiseYScale = perlinNoiseYScale;
        previousHexTerrainXLength = hexTerrainXLength;
        previousHexTerrainZLength = hexTerrainZLength;
        previousTransitionLength = transitionLength;
        previousTransitionCurve = transitionCurve;

        return true;
    }

    private void GenerateGrid()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        CalculateHexTerrainBounds();
        transitionXVertices = CalculateTransitionBounds(hexTerrainXStart, hexTerrainXEnd, hexTerrainXLength);
        transitionZVertices = CalculateTransitionBounds(hexTerrainZStart, hexTerrainZEnd, hexTerrainZLength);

        UpdateVertices();
        UpdateTrianglesPoints();

        UpdateMesh();
    }

    private void CalculateHexTerrainBounds()
    {
        hexTerrainXStart = (gridXLength - hexTerrainXLength) / 2;
        hexTerrainXEnd = hexTerrainXStart + hexTerrainXLength;

        hexTerrainZStart = (gridZLength - hexTerrainZLength) / 2;
        hexTerrainZEnd = hexTerrainZStart + hexTerrainZLength;
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
        if (hexTerrainXStart <= indexOfX && hexTerrainXEnd >= indexOfX && hexTerrainZStart <= indexOfZ && hexTerrainZEnd >= indexOfZ) return 0f;

        float perlinNoiseXCoord = indexOfX * perlinNoiseXScale + perlinNoiseXCoordOffset;
        float perlinNoiseZCoord = indexOfZ * perlinNoiseZScale + perlinNoiseZCoordOffset;

        float perlinNoise = Mathf.PerlinNoise(perlinNoiseXCoord, perlinNoiseZCoord) * perlinNoiseYScale;

        perlinNoise *= GetTransitionPercentage(indexOfX, indexOfZ);

        return perlinNoise;
    }

    private float GetTransitionPercentage(int indexOfX, int indexOfZ)
    {
        if (transitionXVertices.ContainsKey(indexOfX) && hexTerrainZStart <= indexOfZ && hexTerrainZEnd >= indexOfZ) return transitionXVertices[indexOfX];
        else if (transitionZVertices.ContainsKey(indexOfZ) && hexTerrainXStart <= indexOfX && hexTerrainXEnd >= indexOfX) return transitionZVertices[indexOfZ];
        else if (transitionXVertices.ContainsKey(indexOfX) && transitionZVertices.ContainsKey(indexOfZ))
        {
            if (indexOfX <= hexTerrainXStart && indexOfZ <= hexTerrainZStart) return (indexOfX - hexTerrainXStart) <= (indexOfZ - hexTerrainZStart) ? transitionXVertices[indexOfX] : transitionZVertices[indexOfZ];
            else if (indexOfX >= hexTerrainXEnd && indexOfZ <= hexTerrainZStart) return (indexOfX - hexTerrainXEnd) >= (hexTerrainZStart - indexOfZ) ? transitionXVertices[indexOfX] : transitionZVertices[indexOfZ];
            else if (indexOfX <= hexTerrainXStart && indexOfZ >= hexTerrainZEnd) return (hexTerrainXStart - indexOfX) >= (indexOfZ - hexTerrainZEnd) ? transitionXVertices[indexOfX] : transitionZVertices[indexOfZ];
            else return (indexOfX - hexTerrainXEnd) >= (indexOfZ - hexTerrainZEnd) ? transitionXVertices[indexOfX] : transitionZVertices[indexOfZ];
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
                Vector3 currentVertice = vertices[currentVert];
                if (currentVertice.x >= hexTerrainXStart && currentVertice.x <= hexTerrainXEnd - 1 &&
                    currentVertice.z >= hexTerrainZStart && currentVertice.z <= hexTerrainZEnd - 1 &&
                   currentVertice.y == 0f)
                {
                    // skip 'm
                }
                else
                {
                    allTrianglePoints[currentSqaure * 6 + 0] = currentVert + 0;
                    allTrianglePoints[currentSqaure * 6 + 1] = currentVert + gridXLength + 1;
                    allTrianglePoints[currentSqaure * 6 + 2] = currentVert + 1;
                    allTrianglePoints[currentSqaure * 6 + 3] = currentVert + 1;
                    allTrianglePoints[currentSqaure * 6 + 4] = currentVert + gridXLength + 1;
                    allTrianglePoints[currentSqaure * 6 + 5] = currentVert + gridXLength + 2;
                }

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
