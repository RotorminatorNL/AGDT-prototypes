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

    // I know yikes, but it needs to be like this for now.

    public HexagonTileTypes HexTileTypes { get { return hexTileTypes; } }
    [Header("Temporary")]
    [SerializeField] private HexagonTileTypes hexTileTypes;
    public List<HexagonTileTypeChance> Tiles = new List<HexagonTileTypeChance>();
    private List<int> tilePool;
    [SerializeField] private float hexXOffset = 0.5f;
    [SerializeField] private float hexZOffset = -0.1f;
    [SerializeField] private float hexZCorrectionOffset = -0.135f;
    [SerializeField] private float hexXOddOffset = 0.5f;

    // I am serious I hate this too, so lit it be for now.

    [Space(10)]
    [SerializeField] private GridSettings gridSettings;

    [Space(10)]
    [SerializeField] private PerlinNoiseSettings perlinNoiseSettings;

    [Space(10)]
    [SerializeField] private HexagonTerrainSettings hexTerrainSettings;

    [Space(10)]
    [SerializeField] private TransitionSettings transitionSettings;

    private void Update()
    {
        if (gridSettings.GridXLength <= 0 || gridSettings.GridZLength <= 0 || hexTerrainSettings.HexagonTerrainXLength <= 0 || hexTerrainSettings.HexagonTerrainZLength <= 0 || transitionSettings.TransitionLength <= 0) return;

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

    public void GenerateGrid()
    {
        Debug.Log("sweg");

        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        hexTerrainSettings.CalculateHexTerrainBounds(gridSettings);
        transitionSettings.SetTransitionPercentages(hexTerrainSettings.HexagonTerrainXStart, hexTerrainSettings.HexagonTerrainXEnd, hexTerrainSettings.HexagonTerrainXLength, "X");
        transitionSettings.SetTransitionPercentages(hexTerrainSettings.HexagonTerrainZStart, hexTerrainSettings.HexagonTerrainZEnd, hexTerrainSettings.HexagonTerrainZLength, "Z");

        UpdateVertices();
        UpdateTrianglesPoints();
        UpdateMesh();

        GenerateHexTiles();
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
        if (hexTerrainSettings.HexagonTerrainXStart <= indexOfX && hexTerrainSettings.HexagonTerrainXEnd >= indexOfX && hexTerrainSettings.HexagonTerrainZStart <= indexOfZ && hexTerrainSettings.HexagonTerrainZEnd >= indexOfZ) return 0f;
        return perlinNoiseSettings.GetPerlinNoiseValue(indexOfX, indexOfZ) * GetTransitionPercentage(indexOfX, indexOfZ);
    }

    private float GetTransitionPercentage(int indexOfX, int indexOfZ)
    {
        if (transitionSettings.TransitionXVertices.ContainsKey(indexOfX) && hexTerrainSettings.HexagonTerrainZStart <= indexOfZ && hexTerrainSettings.HexagonTerrainZEnd >= indexOfZ) return transitionSettings.TransitionXVertices[indexOfX];
        else if (transitionSettings.TransitionZVertices.ContainsKey(indexOfZ) && hexTerrainSettings.HexagonTerrainXStart <= indexOfX && hexTerrainSettings.HexagonTerrainXEnd >= indexOfX) return transitionSettings.TransitionZVertices[indexOfZ];
        else if (transitionSettings.TransitionXVertices.ContainsKey(indexOfX) && transitionSettings.TransitionZVertices.ContainsKey(indexOfZ))
        {
            if (indexOfX <= hexTerrainSettings.HexagonTerrainXStart && indexOfZ <= hexTerrainSettings.HexagonTerrainZStart) return (indexOfX - hexTerrainSettings.HexagonTerrainXStart) <= (indexOfZ - hexTerrainSettings.HexagonTerrainZStart) ? transitionSettings.TransitionXVertices[indexOfX] : transitionSettings.TransitionZVertices[indexOfZ];
            else if (indexOfX >= hexTerrainSettings.HexagonTerrainXEnd && indexOfZ <= hexTerrainSettings.HexagonTerrainZStart) return (indexOfX - hexTerrainSettings.HexagonTerrainXEnd) >= (hexTerrainSettings.HexagonTerrainZStart - indexOfZ) ? transitionSettings.TransitionXVertices[indexOfX] : transitionSettings.TransitionZVertices[indexOfZ];
            else if (indexOfX <= hexTerrainSettings.HexagonTerrainXStart && indexOfZ >= hexTerrainSettings.HexagonTerrainZEnd) return (hexTerrainSettings.HexagonTerrainXStart - indexOfX) >= (indexOfZ - hexTerrainSettings.HexagonTerrainZEnd) ? transitionSettings.TransitionXVertices[indexOfX] : transitionSettings.TransitionZVertices[indexOfZ];
            else return (indexOfX - hexTerrainSettings.HexagonTerrainXEnd) >= (indexOfZ - hexTerrainSettings.HexagonTerrainZEnd) ? transitionSettings.TransitionXVertices[indexOfX] : transitionSettings.TransitionZVertices[indexOfZ];
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
                if (currentVertice.x >= hexTerrainSettings.HexagonTerrainXStart && currentVertice.x <= hexTerrainSettings.HexagonTerrainXEnd - 1 &&
                    currentVertice.z >= hexTerrainSettings.HexagonTerrainZStart && currentVertice.z <= hexTerrainSettings.HexagonTerrainZEnd - 1 &&
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

    private void GenerateHexTiles()
    {
        CreateTilePool();
        ClearHexTiles();

        for (int z = hexTerrainSettings.HexagonTerrainZStart; z < hexTerrainSettings.HexagonTerrainZEnd; z++)
        {
            for (int x = hexTerrainSettings.HexagonTerrainXStart; x < hexTerrainSettings.HexagonTerrainXEnd; x++)
            {
                GenerateHexTile(x, z);
            }
        }
    }

    private void CreateTilePool()
    {
        Tiles[0].Chance = 0;
        tilePool = new List<int>();
        for (int i = 0; i < Tiles.Count; i++)
        {
            for (int j = 0; j < (Tiles[i].Chance * 10); j++)
            {
                tilePool.Add(i);
            }
        }
    }

    public void ClearHexTiles()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    private void GenerateHexTile(int xPos, int zPos)
    {
        if (hexTileTypes == null) return;
        GameObject hexPrefab = hexTileTypes.Types[0].Prefab;
        GameObject hex = Instantiate(hexPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);

        hex.GetComponent<HexagonTileSettings>().SetTileType(GetTileType());
        hex.GetComponent<HexagonTileSettings>().UpdateTileType();

        float x = xPos + hexXOffset;
        float z = zPos + hexZOffset + ((zPos - hexTerrainSettings.HexagonTerrainZLength) * hexZCorrectionOffset);

        if (zPos % 2 == 1) x += hexXOddOffset;

        hex.transform.position = new Vector3(x, 1, z);
        hex.name = $"Hex {xPos},{zPos}";
    }

    private int GetTileType()
    {
        return tilePool[Random.Range(0, tilePool.Count)];
    }
}
