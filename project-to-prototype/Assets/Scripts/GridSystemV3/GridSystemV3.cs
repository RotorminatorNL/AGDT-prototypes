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
    private bool firstGeneration = true;

    [Space(10)]
    [SerializeField] private GridSettings gridSettings;

    [Space(10)]
    [SerializeField] private PerlinNoiseSettings perlinNoiseSettings;

    [Space(10)]
    [SerializeField] private HexTerrainSettings hexTerrainSettings;

    [Space(10)]
    [SerializeField] private TransitionSettings transitionSettings;

    private void Update()
    {
        if (gridSettings.GridXLength <= 0 || gridSettings.GridZLength <= 0 || hexTerrainSettings.HexTerrainXLength <= 0 || hexTerrainSettings.HexTerrainZLength <= 0 || transitionSettings.TransitionLength <= 0) return;

        if (generatorActive)
        {
            if (firstGeneration)
            {
                gridSettings.UpdateValues();
                perlinNoiseSettings.UpdateValues();
                hexTerrainSettings.UpdateValues();
                transitionSettings.UpdateValues();

                GenerateGrid();
                firstGeneration = false;
            }

            if (HasValueChanged()) GenerateGrid();
        }
    }

    private bool HasValueChanged()
    {
        if (gridSettings.HasValueChanged() || perlinNoiseSettings.HasValueChanged() || hexTerrainSettings.HasValueChanged() || transitionSettings.HasValueChanged()) return true;
        return false;
    }

    private void GenerateGrid()
    {
        Debug.Log("sweg");

        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        hexTerrainSettings.CalculateHexTerrainBounds(gridSettings);
        transitionSettings.SetTransitionPercentages(hexTerrainSettings.HexTerrainXStart, hexTerrainSettings.HexTerrainXEnd, hexTerrainSettings.HexTerrainXLength, "X");
        transitionSettings.SetTransitionPercentages(hexTerrainSettings.HexTerrainZStart, hexTerrainSettings.HexTerrainZEnd, hexTerrainSettings.HexTerrainZLength, "Z");

        UpdateVertices();
        UpdateTrianglesPoints();
        UpdateMesh();
    }

    private void UpdateVertices()
    {
        vertices = new Vector3[(gridSettings.GridXLength + 1) * (gridSettings.GridZLength + 1)];

        for (int i = 0, z = 0; z <= gridSettings.GridZLength; z++)
        {
            for (int x = 0; x <= gridSettings.GridXLength; x++)
            {
                vertices[i] = new Vector3(x, GetYValue(x, z), z);
                i++;
            }
        }
    }

    private float GetYValue(int indexOfX, int indexOfZ)
    {
        if (hexTerrainSettings.HexTerrainXStart <= indexOfX && hexTerrainSettings.HexTerrainXEnd >= indexOfX && hexTerrainSettings.HexTerrainZStart <= indexOfZ && hexTerrainSettings.HexTerrainZEnd >= indexOfZ) return 0f;
        return perlinNoiseSettings.GetPerlinNoiseValue(indexOfX, indexOfZ) * GetTransitionPercentage(indexOfX, indexOfZ);
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
