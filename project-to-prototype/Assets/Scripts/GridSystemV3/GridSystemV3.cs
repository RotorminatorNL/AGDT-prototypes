using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class GridSystemV3 : MonoBehaviour
{
    private Mesh mesh;

    private Vector3[] vertices;
    private int[] allTrianglePoints;

    [Header("Grid settings")]
    [SerializeField] private int outerGridXLength = 120;
    [SerializeField] private int outerGridZLength = 80;

    [Space(15)]
    [SerializeField] private int innerGridXLength = 80;
    private int innerGridXStart;
    private int innerGridXEnd;
    [SerializeField] private int innerGridZLength = 50;
    private int innerGridZStart;
    private int innerGridZEnd;

    [Space(15)]
    [SerializeField] private int outerToInnerTransition = 2;
    [SerializeField, Range(0f, 1f)] private float transitionStrengh = 0.8f;
    private Dictionary<int, float> transitionXVertices = new Dictionary<int, float>();
    private Dictionary<int, float> transitionZVertices = new Dictionary<int, float>();

    [Space(20)]
    [Header("Perlin Noise settings")]
    [SerializeField] private float perlinNoiseXCoordOffset = 0f;
    [SerializeField, Range(0.01f, 1f)] private float perlinNoiseXScale = 0.1f;

    [Space(15)]
    [SerializeField] private float perlinNoiseZCoordOffset = 0f;
    [SerializeField, Range(0.01f, 1f)] private float perlinNoiseZScale = 0.1f;

    [Space(15)]
    [SerializeField, Range(0, 30)] private float perlinNoiseYScale = 10f;

    private void Update()
    {
        if (outerGridXLength <= 0 || outerGridZLength <= 0 || innerGridXLength <= 0 || innerGridZLength <= 0) return;
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        CalculateInnerGridBounds();
        CalculateTransitionBounds();

        UpdateVertices();
        UpdateTrianglesPoints();

        UpdateMesh();
    }

    private void CalculateInnerGridBounds()
    {
        innerGridXStart = (outerGridXLength - innerGridXLength) / 2;
        innerGridXEnd = innerGridXStart + innerGridXLength;

        innerGridZStart = (outerGridZLength - innerGridZLength) / 2;
        innerGridZEnd = innerGridZStart + innerGridZLength;
    }

    private void CalculateTransitionBounds()
    {
        transitionXVertices = new Dictionary<int, float>();
        bool otherSideOfX = false;
        for (int x = outerToInnerTransition, i = innerGridXStart - outerToInnerTransition; i <= innerGridXEnd + outerToInnerTransition; i++)
        {
            if (i == innerGridXStart)
            {
                i += innerGridXLength + 1;
                x = 1;
                otherSideOfX = true;
            }

            transitionXVertices.Add(i, (otherSideOfX == false ? (float)x-- : (float)x++) / (float)outerToInnerTransition * transitionStrengh);
        }

        transitionZVertices = new Dictionary<int, float>();
        bool otherSideOfZ = false;
        for (int x = outerToInnerTransition, i = innerGridZStart - outerToInnerTransition; i <= innerGridZEnd + outerToInnerTransition; i++)
        {
            if (i == innerGridZStart)
            {
                i += innerGridZLength + 1;
                x = 1;
                otherSideOfZ = true;
            }

            transitionZVertices.Add(i, (otherSideOfZ == false ? (float)x-- : (float)x++) / (float)outerToInnerTransition * transitionStrengh);
        }
    }

    private void UpdateVertices()
    {
        vertices = new Vector3[(outerGridXLength + 1) * (outerGridZLength + 1)];

        for (int i = 0, z = 0; z <= outerGridZLength; z++)
        {
            for (int x = 0; x <= outerGridXLength; x++)
            {
                vertices[i] = new Vector3(x, SetYValue(x, z), z);
                i++;
            }
        }
    }

    private float SetYValue(int indexOfX, int indexOfZ)
    {
        if (innerGridXStart <= indexOfX && innerGridXEnd >= indexOfX && innerGridZStart <= indexOfZ && innerGridZEnd >= indexOfZ)
        {
            return 0f;
        }

        float perlinNoiseXCoord = indexOfX * perlinNoiseXScale + perlinNoiseXCoordOffset;
        float perlinNoiseZCoord = indexOfZ * perlinNoiseZScale + perlinNoiseZCoordOffset;

        float perlinNoise = Mathf.PerlinNoise(perlinNoiseXCoord, perlinNoiseZCoord) * perlinNoiseYScale;

        if (transitionXVertices.ContainsKey(indexOfX) && innerGridZStart - outerToInnerTransition < indexOfZ && innerGridZEnd + outerToInnerTransition > indexOfZ) perlinNoise *= transitionXVertices[indexOfX];
        else if (transitionZVertices.ContainsKey(indexOfZ) && innerGridXStart - outerToInnerTransition < indexOfX && innerGridXEnd + outerToInnerTransition > indexOfX) perlinNoise *= transitionZVertices[indexOfZ];

        return perlinNoise;
    }

    private void UpdateTrianglesPoints()
    {
        allTrianglePoints = new int[outerGridXLength * outerGridZLength * 6];

        int currentVert = 0;
        int currentSqaure = 0;

        for (int z = 0; z < outerGridZLength; z++)
        {
            for (int x = 0; x < outerGridXLength; x++)
            {
                allTrianglePoints[currentSqaure * 6 + 0] = currentVert + 0;
                allTrianglePoints[currentSqaure * 6 + 1] = currentVert + outerGridXLength + 1;
                allTrianglePoints[currentSqaure * 6 + 2] = currentVert + 1;
                allTrianglePoints[currentSqaure * 6 + 3] = currentVert + 1;
                allTrianglePoints[currentSqaure * 6 + 4] = currentVert + outerGridXLength + 1;
                allTrianglePoints[currentSqaure * 6 + 5] = currentVert + outerGridXLength + 2;

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
