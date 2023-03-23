using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemV3_2 : MonoBehaviour
{
    public HexagonTileTypes TileTypes { get { return tileTypes; } }
    [SerializeField] private HexagonTileTypes tileTypes;
    public List<HexagonTileTypeChance> HexagonTiles = new List<HexagonTileTypeChance>();
    private List<int> hexagonTilePool;

    [Space(10)]

    [SerializeField] private OuterGridSettings outerGrid;
    [SerializeField] private PerlinNoiseSettings perlinNoiseSettings;
    [SerializeField] private TransitionSettings transitionSettings;
    public InnerGridSettings innerGrid;

    public void GenerateGrid()
    {
        if (!AbleToGenerate()) return;

        ClearGrid();

        innerGrid.CalculateBounds(outerGrid);
        transitionSettings.SetTransitionPercentages(innerGrid.GridXStart, innerGrid.GridXEnd, innerGrid.GridXLength, "X");
        transitionSettings.SetTransitionPercentages(innerGrid.GridZStart, innerGrid.GridZEnd, innerGrid.GridZLength, "Z");

        CreateHexagonTilePool();
        GenerateOuterGrid();
        GenerateInnerGrid();
    }

    private bool AbleToGenerate()
    {
        bool outerGridSizeWrong = outerGrid.GridXLength <= 0 || outerGrid.GridZLength <= 0;
        bool innerGridSizeWrong = innerGrid.GridXLength <= 0 || innerGrid.GridZLength <= 0;
        if (outerGridSizeWrong || innerGridSizeWrong || transitionSettings.Length <= 0) return false;
        return true;
    }

    public void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);
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
                if (x >= innerGrid.GridXStart && x <= innerGrid.GridXEnd && z >= innerGrid.GridZStart && z <= innerGrid.GridZEnd) { /* skip 'm */ }
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
        if (transitionSettings.XHexagons.ContainsKey(x) && innerGrid.GridZStart <= z && innerGrid.GridZEnd >= z) return transitionSettings.XHexagons[x];
        else if (transitionSettings.ZHexagons.ContainsKey(z) && innerGrid.GridXStart <= x && innerGrid.GridXEnd >= x) return transitionSettings.ZHexagons[z];
        else if (transitionSettings.XHexagons.ContainsKey(x) && transitionSettings.ZHexagons.ContainsKey(z))
        {
            if (x <= innerGrid.GridXStart && z <= innerGrid.GridZStart) return (x - innerGrid.GridXStart) <= (z - innerGrid.GridZStart) ? transitionSettings.XHexagons[x] : transitionSettings.ZHexagons[z];
            else if (x >= innerGrid.GridXEnd && z <= innerGrid.GridZStart) return (x - innerGrid.GridXEnd) >= (innerGrid.GridZStart - z) ? transitionSettings.XHexagons[x] : transitionSettings.ZHexagons[z];
            else if (x <= innerGrid.GridXStart && z >= innerGrid.GridZEnd) return (innerGrid.GridXStart - x) >= (z - innerGrid.GridZEnd) ? transitionSettings.XHexagons[x] : transitionSettings.ZHexagons[z];
            else return (x - innerGrid.GridXEnd) >= (z - innerGrid.GridZEnd) ? transitionSettings.XHexagons[x] : transitionSettings.ZHexagons[z];
        }

        return 1;
    }

    private void GenerateInnerGrid()
    {
        int newHexagonTerrainZLength = innerGrid.GridZEnd + Mathf.CeilToInt(innerGrid.GridZLength * innerGrid.HexagonTileZSpaceCorrection) + 1;

        for (int z = innerGrid.GridZStart; z < newHexagonTerrainZLength; z++)
        {
            for (int x = innerGrid.GridXStart; x < innerGrid.GridXEnd + 1; x++)
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
        float z = zPos - ((zPos - innerGrid.GridZStart) * innerGrid.HexagonTileZSpaceCorrection);

        if (zPos % 2 == 1) x += innerGrid.HexagonTileXOddOffset;

        hex.transform.position = new Vector3(x, 0.8f, z);
        hex.transform.localScale = new Vector3(1, newHeight, 1);
        hex.name = $"Hex-tile {xPos},{zPos}";
    }

    public int GetHexagonTileType()
    {
        return hexagonTilePool[Random.Range(0, hexagonTilePool.Count)];
    }
}
