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

    [SerializeField] private bool generatorActive = true;

    [Space(10)]
    [SerializeField] private GridSettings gridSettings;

    [Space(10)]
    [SerializeField] private PerlinNoiseSettings perlinNoiseSettings;

    [Space(10)]
    [SerializeField] private HexTerrainSettings hexTerrainSettings;

    [Space(10)]
    [SerializeField] private TransitionSettings transitionSettings;

    // Variable to check for change
    private bool previousGeneratorActive;
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
        if (gridSettings.GridXLength <= 0 || gridSettings.GridZLength <= 0 || hexTerrainSettings.HexTerrainXLength <= 0 || hexTerrainSettings.HexTerrainZLength <= 0 || transitionSettings.TransitionLength <= 0) return;

        // No change, no generation
        if (IsValueChanged() && generatorActive) GenerateGrid();
    }

    private bool IsValueChanged()
    {
        // Check for changes
        if (previousGeneratorActive == generatorActive &&
            previousGridXLength == gridSettings.GridXLength &&
            previousGridZLength == gridSettings.GridZLength &&
            previousPerlinNoiseXCoordOffset == perlinNoiseSettings.PerlinNoiseXCoordOffset &&
            perviousPerlinNoiseXScale == perlinNoiseSettings.PerlinNoiseXScale &&
            previousPerlinNoiseZCoordOffset == perlinNoiseSettings.PerlinNoiseZCoordOffset &&
            previousPerlinNoiseZScale == perlinNoiseSettings.PerlinNoiseZScale &&
            previousPerlinNoiseYScale == perlinNoiseSettings.PerlinNoiseYScale &&
            previousHexTerrainXLength == hexTerrainSettings.HexTerrainXLength &&
            previousHexTerrainZLength == hexTerrainSettings.HexTerrainZLength &&
            previousTransitionLength == transitionSettings.TransitionLength &&
            previousTransitionCurve == transitionSettings.TransitionCurve) return false;

        // Change detected -> store changes
        previousGeneratorActive = generatorActive;
        previousGridXLength = gridSettings.GridXLength;
        previousGridZLength = gridSettings.GridZLength;
        previousPerlinNoiseXCoordOffset = perlinNoiseSettings.PerlinNoiseXCoordOffset;
        perviousPerlinNoiseXScale = perlinNoiseSettings.PerlinNoiseXScale;
        previousPerlinNoiseZCoordOffset = perlinNoiseSettings.PerlinNoiseZCoordOffset;
        previousPerlinNoiseZScale = perlinNoiseSettings.PerlinNoiseZScale;
        previousPerlinNoiseYScale = perlinNoiseSettings.PerlinNoiseYScale;
        previousHexTerrainXLength = hexTerrainSettings.HexTerrainXLength;
        previousHexTerrainZLength = hexTerrainSettings.HexTerrainZLength;
        previousTransitionLength = transitionSettings.TransitionLength;
        previousTransitionCurve = transitionSettings.TransitionCurve;

        return true;
    }

    private void GenerateGrid()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        CalculateHexTerrainBounds();
        transitionSettings.TransitionXVertices = CalculateTransitionBounds(hexTerrainSettings.HexTerrainXStart, hexTerrainSettings.HexTerrainXEnd, hexTerrainSettings.HexTerrainXLength);
        transitionSettings.TransitionZVertices = CalculateTransitionBounds(hexTerrainSettings.HexTerrainZStart, hexTerrainSettings.HexTerrainZEnd, hexTerrainSettings.HexTerrainZLength);

        UpdateVertices();
        UpdateTrianglesPoints();
        UpdateMesh();
    }

    private void CalculateHexTerrainBounds()
    {
        hexTerrainSettings.HexTerrainXStart = (gridSettings.GridXLength - hexTerrainSettings.HexTerrainXLength) / 2;
        hexTerrainSettings.HexTerrainXEnd = hexTerrainSettings.HexTerrainXStart + hexTerrainSettings.HexTerrainXLength;

        hexTerrainSettings.HexTerrainZStart = (gridSettings.GridZLength - hexTerrainSettings.HexTerrainZLength) / 2;
        hexTerrainSettings.HexTerrainZEnd = hexTerrainSettings.HexTerrainZStart + hexTerrainSettings.HexTerrainZLength;
    }

    private Dictionary<int, float> CalculateTransitionBounds(int innerGridStart, int innerGridEnd, int innerGridLength)
    {
        Dictionary<int, float> transitionVertices = new Dictionary<int, float>();
        bool otherSide = false;
        for (int x = transitionSettings.TransitionLength, i = innerGridStart - transitionSettings.TransitionLength; i <= innerGridEnd + transitionSettings.TransitionLength; i++)
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
        if (currentStep == transitionSettings.TransitionLength) return 1f;
        else if (currentStep == 0f) return 0f;
        else
        {
            float percentage = currentStep / transitionSettings.TransitionLength;
            percentage = Mathf.Pow(percentage, transitionSettings.TransitionCurve);
            percentage = Mathf.Lerp(0f, 100f, percentage);
            return percentage / 100f;
        }
    }

    private void UpdateVertices()
    {
        vertices = new Vector3[(gridSettings.GridXLength + 1) * (gridSettings.GridZLength + 1)];

        for (int i = 0, z = 0; z <= gridSettings.GridZLength; z++)
        {
            for (int x = 0; x <= gridSettings.GridXLength; x++)
            {
                vertices[i] = new Vector3(x, SetYValue(x, z), z);
                i++;
            }
        }
    }

    private float SetYValue(int indexOfX, int indexOfZ)
    {
        if (hexTerrainSettings.HexTerrainXStart <= indexOfX && hexTerrainSettings.HexTerrainXEnd >= indexOfX && hexTerrainSettings.HexTerrainZStart <= indexOfZ && hexTerrainSettings.HexTerrainZEnd >= indexOfZ) return 0f;

        float perlinNoiseXCoord = indexOfX * perlinNoiseSettings.PerlinNoiseXScale + perlinNoiseSettings.PerlinNoiseXCoordOffset;
        float perlinNoiseZCoord = indexOfZ * perlinNoiseSettings.PerlinNoiseZScale + perlinNoiseSettings.PerlinNoiseZCoordOffset;

        float perlinNoise = Mathf.PerlinNoise(perlinNoiseXCoord, perlinNoiseZCoord) * perlinNoiseSettings.PerlinNoiseYScale;

        perlinNoise *= GetTransitionPercentage(indexOfX, indexOfZ);

        return perlinNoise;
    }

    private float GetTransitionPercentage(int indexOfX, int indexOfZ)
    {
        if (transitionSettings.TransitionXVertices.ContainsKey(indexOfX) && hexTerrainSettings.HexTerrainZStart <= indexOfZ && hexTerrainSettings.HexTerrainZEnd >= indexOfZ) return transitionSettings.TransitionXVertices[indexOfX];
        else if (transitionSettings.TransitionZVertices.ContainsKey(indexOfZ) && hexTerrainSettings.HexTerrainXStart <= indexOfX && hexTerrainSettings.HexTerrainXEnd >= indexOfX) return transitionSettings.TransitionZVertices[indexOfZ];
        else if (transitionSettings.TransitionXVertices.ContainsKey(indexOfX) && transitionSettings.TransitionZVertices.ContainsKey(indexOfZ))
        {
            if (indexOfX <= hexTerrainSettings.HexTerrainXStart && indexOfZ <= hexTerrainSettings.HexTerrainZStart) return (indexOfX - hexTerrainSettings.HexTerrainXStart) <= (indexOfZ - hexTerrainSettings.HexTerrainZStart) ? transitionSettings.TransitionXVertices[indexOfX] : transitionSettings.TransitionZVertices[indexOfZ];
            else if (indexOfX >= hexTerrainSettings.HexTerrainXEnd && indexOfZ <= hexTerrainSettings.HexTerrainZStart) return (indexOfX - hexTerrainSettings.HexTerrainXEnd) >= (hexTerrainSettings.HexTerrainZStart - indexOfZ) ? transitionSettings.TransitionXVertices[indexOfX] : transitionSettings.TransitionZVertices[indexOfZ];
            else if (indexOfX <= hexTerrainSettings.HexTerrainXStart && indexOfZ >= hexTerrainSettings.HexTerrainZEnd) return (hexTerrainSettings.HexTerrainXStart - indexOfX) >= (indexOfZ - hexTerrainSettings.HexTerrainZEnd) ? transitionSettings.TransitionXVertices[indexOfX] : transitionSettings.TransitionZVertices[indexOfZ];
            else return (indexOfX - hexTerrainSettings.HexTerrainXEnd) >= (indexOfZ - hexTerrainSettings.HexTerrainZEnd) ? transitionSettings.TransitionXVertices[indexOfX] : transitionSettings.TransitionZVertices[indexOfZ];
        }

        return 1;
    }

    private void UpdateTrianglesPoints()
    {
        allTrianglePoints = new int[gridSettings.GridXLength * gridSettings.GridZLength * 6];

        int currentVert = 0;
        int currentSqaure = 0;

        for (int z = 0; z < gridSettings.GridZLength; z++)
        {
            for (int x = 0; x < gridSettings.GridXLength; x++)
            {
                Vector3 currentVertice = vertices[currentVert];
                if (currentVertice.x >= hexTerrainSettings.HexTerrainXStart && currentVertice.x <= hexTerrainSettings.HexTerrainXEnd - 1 &&
                    currentVertice.z >= hexTerrainSettings.HexTerrainZStart && currentVertice.z <= hexTerrainSettings.HexTerrainZEnd - 1 &&
                   currentVertice.y == 0f)
                {
                    // skip 'm
                }
                else
                {
                    allTrianglePoints[currentSqaure * 6 + 0] = currentVert + 0;
                    allTrianglePoints[currentSqaure * 6 + 1] = currentVert + gridSettings.GridXLength + 1;
                    allTrianglePoints[currentSqaure * 6 + 2] = currentVert + 1;
                    allTrianglePoints[currentSqaure * 6 + 3] = currentVert + 1;
                    allTrianglePoints[currentSqaure * 6 + 4] = currentVert + gridSettings.GridXLength + 1;
                    allTrianglePoints[currentSqaure * 6 + 5] = currentVert + gridSettings.GridXLength + 2;
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
