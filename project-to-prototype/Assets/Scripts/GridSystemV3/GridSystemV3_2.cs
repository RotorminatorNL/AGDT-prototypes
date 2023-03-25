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
        ColoringGrid();

        RoadMesh.BuildNavMesh();

        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }

    private bool AbleToGenerate()
    {
        bool outerGridSizeWrong = OuterGrid.GridXLength <= 0 || OuterGrid.GridZLength <= 0;
        bool innerGridSizeWrong = InnerGrid.GridXLength <= 0 || InnerGrid.GridZLength <= 0;
        if (outerGridSizeWrong || innerGridSizeWrong || transitionSettings.Length <= 0) return false;
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
                    InstantiateHexagon(true, x, z, yValue);
                }
                else
                {
                    yValue = GetYValue(x, z);
                    InstantiateHexagon(false, x, z, yValue);
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

    private void InstantiateHexagon(bool isInnerGrid, int xPos, int zPos, float newHeight = 1)
    {
        if (hexagonParentPrefab == null) return;
        GameObject hexPrefab = hexagonParentPrefab;
        GameObject hex = Instantiate(hexPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);

        if (isInnerGrid)
        {
            innerGridTiles.Add(hex.GetComponent<HexagonTileSettings>());
            innerGridTilesHeight.Add(newHeight);
        }
        else
        {
            outerGridTiles.Add(hex.GetComponent<HexagonTileSettings>());
            outerGridTilesHeight.Add(newHeight);
        }

        float x = xPos;
        float z = zPos - ((zPos - InnerGrid.GridZStart) * hexagonTileZSpaceCorrection);

        if (zPos % 2 == 1) x += hexagonTileXOddOffset;

        hex.transform.localPosition = new Vector3(x, 0, z);
        hex.transform.localScale = new Vector3(1, newHeight, 1);
        hex.name = $"Hex coord {xPos},{zPos}";
    }

    private void ColoringGrid()
    {
        for (int i = 0; i < outerGridTiles.Count; i++)
        {
            outerGridTiles[i].SetTileType(OuterGrid.GetHexagonTileType(lowestHeight, highestHeight, outerGridTilesHeight[i]));
            outerGridTiles[i].UpdateTileType();
        }

        for (int i = 0; i < innerGridTiles.Count; i++)
        {
            innerGridTiles[i].SetTileType(InnerGrid.GetHexagonTileType(lowestHeight, highestHeight, innerGridTilesHeight[i]));
            innerGridTiles[i].UpdateTileType();
        }
    }
}