using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemV3_2 : MonoBehaviour
{
    private bool firstGeneration = true;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] allTrianglePoints;

    [SerializeField] private GridSettings grid;
    [SerializeField] private PerlinNoiseSettings perlinNoise;
    public HexagonTerrainSettings HexagonTerrain;

    private void Update()
    {
        if (grid.GridXLength <= 0 || grid.GridZLength <= 0 || HexagonTerrain.HexagonTerrainXLength <= 0 || HexagonTerrain.HexagonTerrainZLength <= 0) return;

        if (firstGeneration)
        {
            grid.UpdateValues();
            perlinNoise.UpdateValues();
            HexagonTerrain.UpdateValues();

            GenerateGrid();
            firstGeneration = false;
        }

        if (ValueChanged()) GenerateGrid();
    }

    private bool ValueChanged()
    {
        if (grid.HasValueChanged() || perlinNoise.HasValueChanged() || HexagonTerrain.HasValueChanged()) return true;
        return false;
    }

    private void GenerateGrid()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        HexagonTerrain.CalculateHexTerrainBounds(grid);

        UpdateVertices();
        UpdateTrianglesPoints();
        UpdateMesh();

        GenerateHexagonTerrain();
    }

    private void UpdateVertices()
    {
        vertices = new Vector3[(grid.GridXLength + 1) * (grid.GridZLength + 1)];

        for (int i = 0, z = 0; z <= grid.GridZLength; z++)
        {
            for (int x = 0; x <= grid.GridXLength; x++)
            {
                vertices[i] = new Vector3(x, GetYValue(x, z), z);
                i++;
            }
        }
    }

    private float GetYValue(int indexOfX, int indexOfZ)
    {
        if (HexagonTerrain.HexagonTerrainXStart <= indexOfX && HexagonTerrain.HexagonTerrainXEnd >= indexOfX && HexagonTerrain.HexagonTerrainZStart <= indexOfZ && HexagonTerrain.HexagonTerrainZEnd >= indexOfZ) return 0f;
        return perlinNoise.GetPerlinNoiseValue(indexOfX, indexOfZ);
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
                Vector3 currentVertice = vertices[currentVert];
                if (currentVertice.x >= HexagonTerrain.HexagonTerrainXStart && currentVertice.x <= HexagonTerrain.HexagonTerrainXEnd - 1 &&
                    currentVertice.z >= HexagonTerrain.HexagonTerrainZStart && currentVertice.z <= HexagonTerrain.HexagonTerrainZEnd - 1 &&
                   currentVertice.y == 0f)
                {
                    // skip 'm
                }
                else
                {
                    allTrianglePoints[currentSquare * 6 + 0] = currentVert + 0;
                    allTrianglePoints[currentSquare * 6 + 1] = currentVert + grid.GridXLength + 1;
                    allTrianglePoints[currentSquare * 6 + 2] = currentVert + 1;
                    allTrianglePoints[currentSquare * 6 + 3] = currentVert + 1;
                    allTrianglePoints[currentSquare * 6 + 4] = currentVert + grid.GridXLength + 1;
                    allTrianglePoints[currentSquare * 6 + 5] = currentVert + grid.GridXLength + 2;
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
        ClearHexagonTerrain();
        HexagonTerrain.CreateHexagonTilePool();

        int newHexagonTerrainZLength = HexagonTerrain.HexagonTerrainZEnd + Mathf.CeilToInt(HexagonTerrain.HexagonTerrainZLength * HexagonTerrain.HexagonTileZSpaceCorrection) + 1;

        for (int z = HexagonTerrain.HexagonTerrainZStart; z < newHexagonTerrainZLength; z++)
        {
            for (int x = HexagonTerrain.HexagonTerrainXStart; x < HexagonTerrain.HexagonTerrainXEnd + 1; x++)
            {
                InstantiateHexagon(x, z);
            }
        }
    }

    private void ClearHexagonTerrain()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    private void InstantiateHexagon(int xPos, int zPos)
    {
        if (HexagonTerrain.HexagonTileTypes == null) return;
        GameObject hexPrefab = HexagonTerrain.HexagonTileTypes.Types[0].Prefab;
        GameObject hex = Instantiate(hexPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);

        HexagonTileSettings hexagonTileSetting = hex.GetComponent<HexagonTileSettings>();
        hexagonTileSetting.TileTypes = HexagonTerrain.HexagonTileTypes;
        hexagonTileSetting.SetTileType(HexagonTerrain.GetHexagonTileType());
        hexagonTileSetting.UpdateTileType();

        float x = xPos;
        float z = zPos - ((zPos - HexagonTerrain.HexagonTerrainZStart) * HexagonTerrain.HexagonTileZSpaceCorrection);

        if (zPos % 2 == 1) x += HexagonTerrain.HexagonTileXOddOffset;

        hex.transform.position = new Vector3(x, 1, z);
        hex.name = $"Hex-tile {xPos},{zPos}";
    }
}
