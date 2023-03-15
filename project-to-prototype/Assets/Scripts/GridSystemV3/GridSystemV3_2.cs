using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemV3_2 : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] allTrianglePoints;

    [SerializeField] private GridSettings gridSettings;
    [SerializeField] private PerlinNoiseSettings perlinNoiseSettings;
    [SerializeField] private TransitionSettings transitionSettings;
    public HexagonTerrainSettings HexagonTerrainSettings;

    public void GenerateGrid()
    {
        if (!AbleToGenerate()) return;

        ClearGrid();

        HexagonTerrainSettings.CalculateHexTerrainBounds(gridSettings);
        transitionSettings.SetTransitionPercentages(HexagonTerrainSettings.HexagonTerrainXStart, HexagonTerrainSettings.HexagonTerrainXEnd, HexagonTerrainSettings.HexagonTerrainXLength, "X");
        transitionSettings.SetTransitionPercentages(HexagonTerrainSettings.HexagonTerrainZStart, HexagonTerrainSettings.HexagonTerrainZEnd, HexagonTerrainSettings.HexagonTerrainZLength, "Z");

        UpdateVertices();
        UpdateTrianglesPoints();
        UpdateMesh();

        GenerateHexagonTerrain();
    }

    private bool AbleToGenerate()
    {
        bool gridSizeWrong = gridSettings.GridXLength <= 0 || gridSettings.GridZLength <= 0;
        bool hexagonTerrainWrong = HexagonTerrainSettings.HexagonTerrainXLength <= 0 || HexagonTerrainSettings.HexagonTerrainZLength <= 0;
        if (gridSizeWrong || hexagonTerrainWrong || transitionSettings.TransitionLength <= 0) return false;
        return true;
    }

    public void ClearGrid()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
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
        if (HexagonTerrainSettings.HexagonTerrainXStart <= indexOfX && HexagonTerrainSettings.HexagonTerrainXEnd >= indexOfX && HexagonTerrainSettings.HexagonTerrainZStart <= indexOfZ && HexagonTerrainSettings.HexagonTerrainZEnd >= indexOfZ) return 0f;
        return perlinNoiseSettings.GetPerlinNoiseValue(indexOfX, indexOfZ) * GetTransitionPercentage(indexOfX, indexOfZ);
    }

    private float GetTransitionPercentage(int indexOfX, int indexOfZ)
    {
        if (transitionSettings.TransitionXVertices.ContainsKey(indexOfX) && HexagonTerrainSettings.HexagonTerrainZStart <= indexOfZ && HexagonTerrainSettings.HexagonTerrainZEnd >= indexOfZ) return transitionSettings.TransitionXVertices[indexOfX];
        else if (transitionSettings.TransitionZVertices.ContainsKey(indexOfZ) && HexagonTerrainSettings.HexagonTerrainXStart <= indexOfX && HexagonTerrainSettings.HexagonTerrainXEnd >= indexOfX) return transitionSettings.TransitionZVertices[indexOfZ];
        else if (transitionSettings.TransitionXVertices.ContainsKey(indexOfX) && transitionSettings.TransitionZVertices.ContainsKey(indexOfZ))
        {
            if (indexOfX <= HexagonTerrainSettings.HexagonTerrainXStart && indexOfZ <= HexagonTerrainSettings.HexagonTerrainZStart) return (indexOfX - HexagonTerrainSettings.HexagonTerrainXStart) <= (indexOfZ - HexagonTerrainSettings.HexagonTerrainZStart) ? transitionSettings.TransitionXVertices[indexOfX] : transitionSettings.TransitionZVertices[indexOfZ];
            else if (indexOfX >= HexagonTerrainSettings.HexagonTerrainXEnd && indexOfZ <= HexagonTerrainSettings.HexagonTerrainZStart) return (indexOfX - HexagonTerrainSettings.HexagonTerrainXEnd) >= (HexagonTerrainSettings.HexagonTerrainZStart - indexOfZ) ? transitionSettings.TransitionXVertices[indexOfX] : transitionSettings.TransitionZVertices[indexOfZ];
            else if (indexOfX <= HexagonTerrainSettings.HexagonTerrainXStart && indexOfZ >= HexagonTerrainSettings.HexagonTerrainZEnd) return (HexagonTerrainSettings.HexagonTerrainXStart - indexOfX) >= (indexOfZ - HexagonTerrainSettings.HexagonTerrainZEnd) ? transitionSettings.TransitionXVertices[indexOfX] : transitionSettings.TransitionZVertices[indexOfZ];
            else return (indexOfX - HexagonTerrainSettings.HexagonTerrainXEnd) >= (indexOfZ - HexagonTerrainSettings.HexagonTerrainZEnd) ? transitionSettings.TransitionXVertices[indexOfX] : transitionSettings.TransitionZVertices[indexOfZ];
        }

        return 1;
    }

    private void UpdateTrianglesPoints()
    {
        allTrianglePoints = new int[gridSettings.GridXLength * gridSettings.GridZLength * 6];

        int currentVert = 0;
        int currentSquare = 0;

        for (int z = 0; z < gridSettings.GridZLength; z++)
        {
            for (int x = 0; x < gridSettings.GridXLength; x++)
            {
                Vector3 currentVertice = vertices[currentVert];
                if (currentVertice.x >= HexagonTerrainSettings.HexagonTerrainXStart && currentVertice.x <= HexagonTerrainSettings.HexagonTerrainXEnd - 1 &&
                    currentVertice.z >= HexagonTerrainSettings.HexagonTerrainZStart && currentVertice.z <= HexagonTerrainSettings.HexagonTerrainZEnd - 1 &&
                   currentVertice.y == 0f)
                {
                    // skip 'm
                }
                else
                {
                    allTrianglePoints[currentSquare * 6 + 0] = currentVert + 0;
                    allTrianglePoints[currentSquare * 6 + 1] = currentVert + gridSettings.GridXLength + 1;
                    allTrianglePoints[currentSquare * 6 + 2] = currentVert + 1;
                    allTrianglePoints[currentSquare * 6 + 3] = currentVert + 1;
                    allTrianglePoints[currentSquare * 6 + 4] = currentVert + gridSettings.GridXLength + 1;
                    allTrianglePoints[currentSquare * 6 + 5] = currentVert + gridSettings.GridXLength + 2;
                }

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

    private void GenerateHexagonTerrain()
    {
        HexagonTerrainSettings.CreateHexagonTilePool();

        int newHexagonTerrainZLength = HexagonTerrainSettings.HexagonTerrainZEnd + Mathf.CeilToInt(HexagonTerrainSettings.HexagonTerrainZLength * HexagonTerrainSettings.HexagonTileZSpaceCorrection) + 1;

        for (int z = HexagonTerrainSettings.HexagonTerrainZStart; z < newHexagonTerrainZLength; z++)
        {
            for (int x = HexagonTerrainSettings.HexagonTerrainXStart; x < HexagonTerrainSettings.HexagonTerrainXEnd + 1; x++)
            {
                InstantiateHexagon(x, z);
            }
        }
    }

    private void InstantiateHexagon(int xPos, int zPos)
    {
        if (HexagonTerrainSettings.HexagonTileTypes == null) return;
        GameObject hexPrefab = HexagonTerrainSettings.HexagonTileTypes.Types[0].Prefab;
        GameObject hex = Instantiate(hexPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);

        HexagonTileSettings hexagonTileSetting = hex.GetComponent<HexagonTileSettings>();
        hexagonTileSetting.TileTypes = HexagonTerrainSettings.HexagonTileTypes;
        hexagonTileSetting.SetTileType(HexagonTerrainSettings.GetHexagonTileType());
        hexagonTileSetting.UpdateTileType();

        float x = xPos;
        float z = zPos - ((zPos - HexagonTerrainSettings.HexagonTerrainZStart) * HexagonTerrainSettings.HexagonTileZSpaceCorrection);

        if (zPos % 2 == 1) x += HexagonTerrainSettings.HexagonTileXOddOffset;

        hex.transform.position = new Vector3(x, 0.8f, z);
        hex.name = $"Hex-tile {xPos},{zPos}";
    }
}
