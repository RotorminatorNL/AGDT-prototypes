using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridSystemV3_2 : MonoBehaviour
{
    public HexagonTileTypes TileTypes { get { return tileTypes; } }
    [SerializeField] private HexagonTileTypes tileTypes;
    public List<HexagonTileTypeChance> HexagonTiles = new List<HexagonTileTypeChance>();
    private List<int> hexagonTilePool;

    [SerializeField] private OuterGridSettings outerGrid;
    [SerializeField] private PerlinNoiseSettings perlinNoiseSettings;
    [SerializeField] private TransitionSettings transitionSettings;
    public InnerGridSettings InnerGrid;

    public NavMeshSurface RoadMesh;

    private void Awake()
    {
        InnerGrid.CalculateBounds(outerGrid);
    }

    public void GenerateGrid()
    {
        if (!AbleToGenerate()) return;

        ClearGrid();

        InnerGrid.CalculateBounds(outerGrid);
        transitionSettings.SetTransitionPercentages(InnerGrid.GridXStart, InnerGrid.GridXEnd, InnerGrid.GridXLength, "X");
        transitionSettings.SetTransitionPercentages(InnerGrid.GridZStart, InnerGrid.GridZEnd, InnerGrid.GridZLength, "Z");

        CreateHexagonTilePool();
        GenerateOuterGrid();
        GenerateInnerGrid();

        RoadMesh.BuildNavMesh();

        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }

    private bool AbleToGenerate()
    {
        bool outerGridSizeWrong = outerGrid.GridXLength <= 0 || outerGrid.GridZLength <= 0;
        bool innerGridSizeWrong = InnerGrid.GridXLength <= 0 || InnerGrid.GridZLength <= 0;
        if (outerGridSizeWrong || innerGridSizeWrong || transitionSettings.Length <= 0) return false;
        return true;
    }

    public void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);
        RoadMesh.BuildNavMesh();
    }

    private void CreateHexagonTilePool()
    {
        if (tileTypes == null) return;
        HexagonTiles[0].Chance = 0;
        hexagonTilePool = new List<int>();
        for (int i = 0; i < HexagonTiles.Count; i++)
        {
            for (int j = 0; j < (HexagonTiles[i].Chance * 10); j++)
            {
                hexagonTilePool.Add(i);
            }
        }
    }

    private void GenerateOuterGrid()
    {
        for (int z = 0; z < outerGrid.GridZLength; z++)
        {
            for (int x = 0; x < outerGrid.GridXLength; x++)
            {
                if (x >= InnerGrid.GridXStart && x <= InnerGrid.GridXEnd && z >= InnerGrid.GridZStart && z <= InnerGrid.GridZEnd) { /* skip 'm */ }
                else
                {
                    InstantiateHexagon(x, z, GetYValue(x, z));
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
        int newHexagonTerrainZLength = InnerGrid.GridZEnd + Mathf.CeilToInt(InnerGrid.GridZLength * InnerGrid.HexagonTileZSpaceCorrection) + 1;

        for (int z = InnerGrid.GridZStart; z < newHexagonTerrainZLength; z++)
        {
            for (int x = InnerGrid.GridXStart; x < InnerGrid.GridXEnd + 1; x++)
            {
                InstantiateHexagon(x, z);
            }
        }
    }

    private void InstantiateHexagon(int xPos, int zPos, float newHeight = 1)
    {
        if (tileTypes == null) return;
        GameObject hexPrefab = tileTypes.Types[0].Prefab;
        GameObject hex = Instantiate(hexPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);

        HexagonTileSettings hexagonTileSetting = hex.GetComponent<HexagonTileSettings>();
        hexagonTileSetting.TileTypes = tileTypes;
        hexagonTileSetting.SetTileType(GetHexagonTileType());
        hexagonTileSetting.UpdateTileType();

        float x = xPos;
        float z = zPos - ((zPos - InnerGrid.GridZStart) * InnerGrid.HexagonTileZSpaceCorrection);

        if (zPos % 2 == 1) x += InnerGrid.HexagonTileXOddOffset;

        hex.transform.localPosition = new Vector3(x, 0, z);
        hex.transform.localScale = new Vector3(1, newHeight, 1);
        hex.name = $"Hex coord {xPos},{zPos}";
    }

    public int GetHexagonTileType()
    {
        return hexagonTilePool[Random.Range(0, hexagonTilePool.Count)];
    }
}
