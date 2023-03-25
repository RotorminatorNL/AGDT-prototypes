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

    private List<HexagonTileSettings> outerGridTiles;
    private List<float> outerGridTilesHeight;
    private List<HexagonTileSettings> innerGridTiles;
    private List<float> innerGridTilesHeight;
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

        InnerGrid.CalculateBounds(OuterGrid);
        transitionSettings.CalculateBounds(InnerGrid);

        outerGridTiles = new List<HexagonTileSettings>();
        outerGridTilesHeight = new List<float>();
        innerGridTiles = new List<HexagonTileSettings>();
        innerGridTilesHeight = new List<float>();

        GenerateGrid();
        SetHexagonType(outerGridTiles, outerGridTilesHeight);
        SetHexagonType(innerGridTiles, innerGridTilesHeight);

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

    private void GenerateGrid()
    {
        for (int z = 0; z < OuterGrid.GridZLength; z++)
        {
            for (int x = 0; x < OuterGrid.GridXLength; x++)
            {
                float yValue;
                if (x >= InnerGrid.GridXStart && x <= InnerGrid.GridXEnd && z >= InnerGrid.GridZStart && z <= InnerGrid.GridZEnd) 
                {
                    yValue = GetYValue(x, z, true);
                    innerGridTilesHeight.Add(yValue);
                    innerGridTiles.Add(InstantiateHexagon(x, z, yValue).GetComponent<HexagonTileSettings>());
                }
                else
                {
                    yValue = GetYValue(x, z);
                    outerGridTilesHeight.Add(yValue);
                    outerGridTiles.Add(InstantiateHexagon(x, z, yValue).GetComponent<HexagonTileSettings>());
                }

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

    private float GetYValue(int x, int z, bool isInnerGrid = false)
    {
        float transitionPercentage = transitionSettings.GetTransitionPercentage(x, z);
        if (!isInnerGrid && transitionPercentage != 1)
        {
            float outerPerlinNoise = perlinNoiseSettings.GetPerlinNoiseValue(x, z);
            float innerPerlinNoise = perlinNoiseSettings.GetPerlinNoiseValue(x, z, true);

            return innerPerlinNoise + ((outerPerlinNoise - innerPerlinNoise) * transitionPercentage);
        }

        float perlinNoise = perlinNoiseSettings.GetPerlinNoiseValue(x, z, isInnerGrid);
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

    private void SetHexagonType(List<HexagonTileSettings> gridTiles, List<float> gridTilesHeight)
    {
        for (int i = 0; i < gridTiles.Count; i++)
        {
            gridTiles[i].SetTileType(GetHexagonTileType(gridTilesHeight[i]));
            gridTiles[i].UpdateTileType();
        }
    }
    
    private string GetHexagonTileType(float currentHeight, bool isInnerGrid = false)
    {
        string nameOfTileType = "";
        foreach (HexagonTileTypeSettings tile in HexagonTiles)
        {
            if (!isInnerGrid && tile.OuterGrid || isInnerGrid && tile.InnerGrid)
            {
                if (nameOfTileType == "") nameOfTileType = IsBelowHeightLimits(tile.MaxHeight, currentHeight) ? tile.Name : nameOfTileType;
            }
        }
        return nameOfTileType;
    }

    private bool IsBelowHeightLimits(float maxHeightPercentage, float height)
    {
        float maxHeight = lowestHeight + ((highestHeight - lowestHeight) * maxHeightPercentage);
        return height <= maxHeight;
    }
}