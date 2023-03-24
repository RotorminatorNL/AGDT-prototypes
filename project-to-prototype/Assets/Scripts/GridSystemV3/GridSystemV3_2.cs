using System.Collections.Generic;
using Unity.AI.Navigation;
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
    [SerializeField] private PerlinNoiseSettings perlinNoiseSettings;
    [SerializeField] private TransitionSettings transitionSettings;
    public InnerGridSettings InnerGrid;

    [Space(10)]

    public NavMeshSurface RoadMesh;

    private void Awake()
    {
        InnerGrid.CalculateBounds(OuterGrid);
    }

    public void GenerateGrid()
    {
        if (!AbleToGenerate()) return;

        ClearGrid();

        InnerGrid.CalculateBounds(OuterGrid);
        transitionSettings.SetTransitionPercentages(InnerGrid.GridXStart, InnerGrid.GridXEnd, InnerGrid.GridXLength, "X");
        transitionSettings.SetTransitionPercentages(InnerGrid.GridZStart, InnerGrid.GridZEnd, InnerGrid.GridZLength, "Z");

        OuterGrid.CreateHexagonTilePool();
        InnerGrid.CreateHexagonTilePool();
        GenerateOuterGrid();
        GenerateInnerGrid();

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

    private void GenerateOuterGrid()
    {
        for (int z = 0; z < OuterGrid.GridZLength; z++)
        {
            for (int x = 0; x < OuterGrid.GridXLength; x++)
            {
                if (x >= InnerGrid.GridXStart && x <= InnerGrid.GridXEnd && z >= InnerGrid.GridZStart && z <= InnerGrid.GridZEnd) { /* skip 'm */ }
                else
                {
                    InstantiateHexagon(OuterGrid.GetHexagonTileType(), x, z, GetYValue(x, z));
                }
            }
        }
    }

    private float GetYValue(int x, int z)
    {
        float newYValue = perlinNoiseSettings.GetPerlinNoiseValue(x, z) * GetTransitionPercentage(x, z);
        return newYValue < 1 ? 1 : newYValue;
    }

    private float GetTransitionPercentage(int x, int z)
    {
        if (transitionSettings.XHexagons.ContainsKey(x) && InnerGrid.GridZStart <= z && InnerGrid.GridZEnd >= z) return transitionSettings.XHexagons[x];
        else if (transitionSettings.ZHexagons.ContainsKey(z) && InnerGrid.GridXStart <= x && InnerGrid.GridXEnd >= x) return transitionSettings.ZHexagons[z];
        else if (transitionSettings.XHexagons.ContainsKey(x) && transitionSettings.ZHexagons.ContainsKey(z))
        {
            if (x <= InnerGrid.GridXStart && z <= InnerGrid.GridZStart) return (x - InnerGrid.GridXStart) <= (z - InnerGrid.GridZStart) ? transitionSettings.XHexagons[x] : transitionSettings.ZHexagons[z];
            else if (x >= InnerGrid.GridXEnd && z <= InnerGrid.GridZStart) return (x - InnerGrid.GridXEnd) >= (InnerGrid.GridZStart - z) ? transitionSettings.XHexagons[x] : transitionSettings.ZHexagons[z];
            else if (x <= InnerGrid.GridXStart && z >= InnerGrid.GridZEnd) return (InnerGrid.GridXStart - x) >= (z - InnerGrid.GridZEnd) ? transitionSettings.XHexagons[x] : transitionSettings.ZHexagons[z];
            else return (x - InnerGrid.GridXEnd) >= (z - InnerGrid.GridZEnd) ? transitionSettings.XHexagons[x] : transitionSettings.ZHexagons[z];
        }

        return 1;
    }

    private void GenerateInnerGrid()
    {
        int newHexagonTerrainZLength = InnerGrid.GridZEnd + Mathf.CeilToInt(InnerGrid.GridZLength * hexagonTileZSpaceCorrection) + 1;

        for (int z = InnerGrid.GridZStart; z < newHexagonTerrainZLength; z++)
        {
            for (int x = InnerGrid.GridXStart; x < InnerGrid.GridXEnd + 1; x++)
            {
                InstantiateHexagon(InnerGrid.GetHexagonTileType(), x, z); ;
            }
        }
    }

    private void InstantiateHexagon(string tileType, int xPos, int zPos, float newHeight = 1)
    {
        if (hexagonParentPrefab == null) return;
        GameObject hexPrefab = hexagonParentPrefab;
        GameObject hex = Instantiate(hexPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);

        HexagonTileSettings hexagonTileSetting = hex.GetComponent<HexagonTileSettings>();
        hexagonTileSetting.SetTileType(tileType);
        hexagonTileSetting.UpdateTileType();

        float x = xPos;
        float z = zPos - ((zPos - InnerGrid.GridZStart) * hexagonTileZSpaceCorrection);

        if (zPos % 2 == 1) x += hexagonTileXOddOffset;

        hex.transform.localPosition = new Vector3(x, 0, z);
        hex.transform.localScale = new Vector3(1, newHeight, 1);
        hex.name = $"Hex coord {xPos},{zPos}";
    }

}
