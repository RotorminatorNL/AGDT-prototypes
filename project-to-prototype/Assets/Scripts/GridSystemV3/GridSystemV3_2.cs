using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridSystemV3_2 : MonoBehaviour
{
    [Header("Hexagon settings")]
    [SerializeField] private GameObject hexagonParentPrefab;
    [SerializeField] private float hexagonTileXSpaceCorrection = 0f;
    [SerializeField] private float hexagonTileZSpaceCorrection = 0.134f;
    [SerializeField] private float hexagonTileXOddOffset = 0.5f;
    public HexagonTileTypes HexagonTileTypes;
    public List<HexagonTileTypeSettings> HexagonTiles = new List<HexagonTileTypeSettings>();

    [Space(10)]
    public OuterGridSettings OuterGrid;
    [Space(5)]
    [SerializeField] private TransitionSettings transitionSettings;
    [Space(5)]
    public InnerGridSettings InnerGrid;
    [Space(5)]
    [SerializeField] private PerlinNoiseSettings perlinNoiseSettings;
    [Space(10)]
    public NavMeshSurface RoadMesh;

    private List<GridTiles> gridTiles;
    private float lowestHeight = 0;
    private float highestHeight = 0;

    private void Awake()
    {
        InnerGrid.CalculateBounds(OuterGrid);
    }

    public void ActivateGenerateGrid()
    {
        if (!AbleToGenerate()) return;

        ClearGrid();

        gridTiles = new List<GridTiles>();
        InnerGrid.CalculateBounds(OuterGrid);
        transitionSettings.CalculateBounds(InnerGrid);

        GenerateTypelessHexagonGrid();
        SetTypeOfHexagonsInGrid();

        RoadMesh.BuildNavMesh();
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }

    private bool AbleToGenerate()
    {
        bool outerGridSizeWrong = OuterGrid.GridXLength <= 0 || OuterGrid.GridZLength <= 0;
        bool innerGridSizeWrong = InnerGrid.GridXLength <= 0 || InnerGrid.GridZLength <= 0;
        if (outerGridSizeWrong || innerGridSizeWrong || transitionSettings.Length <= 0 || hexagonParentPrefab == null) return false;
        return true;
    }

    public void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);
        RoadMesh.BuildNavMesh();
    }

    private void GenerateTypelessHexagonGrid()
    {
        for (int z = 0; z < OuterGrid.GridZLength; z++)
        {
            for (int x = 0; x < OuterGrid.GridXLength; x++)
            {
                float yValue = GetYValue(x, z);
                gridTiles.Add(new GridTiles(InstantiateHexagon(x, z, yValue).GetComponent<HexagonTileSettings>(), yValue, InnerGrid.IsInside(x, z)));

                if (x == 0 && z == 0)
                {
                    lowestHeight = yValue;
                    highestHeight = yValue;
                }
                else
                {
                    lowestHeight = yValue < lowestHeight ? yValue : lowestHeight;
                    highestHeight = yValue > highestHeight ? yValue : highestHeight;
                }
            }
        }
    }

    private float GetYValue(int x, int z)
    {
        float transitionPercentage = transitionSettings.GetTransitionPercentage(x, z);
        if (!InnerGrid.IsInside(x, z) && transitionPercentage != 1)
        {
            float outerPerlinNoise = perlinNoiseSettings.GetPerlinNoiseValue(x, z);
            float innerPerlinNoise = perlinNoiseSettings.GetPerlinNoiseValue(x, z, true);

            return innerPerlinNoise + ((outerPerlinNoise - innerPerlinNoise) * transitionPercentage);
        }

        float perlinNoise = perlinNoiseSettings.GetPerlinNoiseValue(x, z, InnerGrid.IsInside(x, z));
        return perlinNoise;
    }

    private GameObject InstantiateHexagon(int xPos, int zPos, float newHeight = 1)
    {
        float x = xPos + ((xPos - InnerGrid.GridXStart) * hexagonTileXSpaceCorrection);
        float z = zPos + ((zPos - InnerGrid.GridZStart) * hexagonTileZSpaceCorrection);
        if (zPos % 2 == 1) x += hexagonTileXOddOffset;

        GameObject hex = Instantiate(hexagonParentPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
        hex.transform.localPosition = new Vector3(x, 0, z);
        hex.transform.localScale = new Vector3(hex.transform.localScale.x, newHeight, hex.transform.localScale.z);
        hex.name = $"Hex coord {xPos},{zPos}";
        return hex;
    }

    private void SetTypeOfHexagonsInGrid()
    {
        for (int i = 0; i < gridTiles.Count; i++)
        {
            gridTiles[i].TileSettings.SetTileType(GetHexagonTileType(gridTiles[i].Height, gridTiles[i].IsInnerGrid));
            gridTiles[i].TileSettings.UpdateTileType();
        }
    }
    
    private string GetHexagonTileType(float currentHeight, bool isInnerGrid = false)
    {
        string nameOfTileType = "";
        foreach (HexagonTileTypeSettings tile in HexagonTiles)
        {
            if (!isInnerGrid && tile.OuterGrid || isInnerGrid && tile.InnerGrid)
            {
                if (nameOfTileType == "") nameOfTileType = tile.IsHeightBelowMaxHeight(currentHeight, lowestHeight, highestHeight) ? tile.Name : nameOfTileType;
            }
        }
        return nameOfTileType;
    }
}

public class GridTiles
{
    public HexagonTileSettings TileSettings;
    public float Height;
    public bool IsInnerGrid;

    public GridTiles(HexagonTileSettings tileSettings, float height, bool isInnerGrid)
    {
        TileSettings = tileSettings;
        Height = height;
        IsInnerGrid = isInnerGrid;
    }
}