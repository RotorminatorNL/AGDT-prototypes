using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridSystemV3_2 : MonoBehaviour
{
    [Header("Storage of current generation (each generator its own)")]
    [SerializeField] private GridSystemDB gridSystemDB;

    [Header("Nav mesh to connect the roads")]
    public NavMeshSurface NavMeshRoad;

    [Header("Grid tile settings")]
    [SerializeField] private GameObject tileParentPrefab;
    [SerializeField] private float tileXSpaceCorrection = 0.5f;
    [SerializeField] private float tileZSpaceCorrection = 0.3f;
    [SerializeField] private float tileXOddOffset = 0.75f;
    public TileTypeCollection TileTypes;
    public List<TileTypeSettings> TileTypeSettings;

    [Space(5)] public OuterGridSettings OuterGridSettings;
    [Space(5)] [SerializeField] private TransitionSettings transitionSettings;
    [Space(5)] public InnerGridSettings InnerGridSettings;
    [Space(5)] [SerializeField] private PerlinNoiseSettings perlinNoiseSettings;

    private float lowestHeight = 0;
    private float highestHeight = 0;

    private void Awake()
    {
        InnerGridSettings.CalculateBorders(OuterGridSettings);
    }

    public void SaveScene()
    {
        NavMeshRoad.BuildNavMesh();
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        Debug.Log("Saved");
    }

    public void ActivateGenerateGrid()
    {
        if (!AbleToGenerate()) return;

        InnerGridSettings.CalculateBorders(OuterGridSettings);
        transitionSettings.CalculateBorders(InnerGridSettings);

        bool gridLengthsSame = gridSystemDB.CompareGridLengths(OuterGridSettings.GridXLength, OuterGridSettings.GridZLength);
        if (!gridLengthsSame)
        {
            ClearGrid();
            gridSystemDB.StoreGridLengths(OuterGridSettings.GridXLength, OuterGridSettings.GridZLength);
            GenerateTypelessTiles();
        }
        else UpdateGeneratedTiles();

        SetTypeOfTiles();
        SaveScene();
    }

    private bool AbleToGenerate()
    {
        bool outerGridSizeWrong = OuterGridSettings.GridXLength <= 1 || OuterGridSettings.GridZLength <= 1;
        bool innerGridSizeWrong = InnerGridSettings.GridXLength <= 0 || InnerGridSettings.GridZLength <= 0;
        if (gridSystemDB == null || tileParentPrefab == null || outerGridSizeWrong || innerGridSizeWrong || transitionSettings.Length <= 0) return false;
        return true;
    }

    public void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);
        gridSystemDB.ClearInfo();
        NavMeshRoad.BuildNavMesh();
    }

    private void GenerateTypelessTiles()
    {
        for (int zPos = 0; zPos < OuterGridSettings.GridZLength; zPos++)
        {
            for (int xPos = 0; xPos < OuterGridSettings.GridXLength; xPos++)
            {
                bool innerGrid = InnerGridSettings.IsInside(xPos, zPos);
                float heightValue = GetHeightValue(xPos, zPos, innerGrid);

                InstantiateTile(xPos, zPos, heightValue);
                gridSystemDB.StoreTile(xPos, zPos, heightValue, !innerGrid, innerGrid);
            }
        }
    }

    private void UpdateGeneratedTiles()
    {
        for (int i = 0; i < gridSystemDB.Tiles.Count; i++)
        {
            GridTileInfo tileInfo = gridSystemDB.Tiles[i];
            bool innerGrid = InnerGridSettings.IsInside(tileInfo.XPos, tileInfo.ZPos);
            float heightValue = GetHeightValue(tileInfo.XPos, tileInfo.ZPos, innerGrid);

            Transform tile = transform.GetChild(i);
            tile.localScale = new Vector3(tile.localScale.x, heightValue, tile.localScale.z);
            tileInfo.UpdateTileInfo(heightValue, !innerGrid, innerGrid);
        }
    }

    private float GetHeightValue(int xPos, int zPos, bool insideInnerGrid)
    {
        float transitionPercentage = transitionSettings.GetTransitionPercentage(xPos, zPos);
        float perlinNoiseHeight;
        if (!insideInnerGrid && transitionPercentage != 1)
        {
            float outerPerlinNoise = perlinNoiseSettings.GetPerlinNoiseValue(xPos, zPos);
            float innerPerlinNoise = perlinNoiseSettings.GetPerlinNoiseValue(xPos, zPos, true);
            perlinNoiseHeight = innerPerlinNoise + ((outerPerlinNoise - innerPerlinNoise) * transitionPercentage);
        }
        else
        {
            perlinNoiseHeight = perlinNoiseSettings.GetPerlinNoiseValue(xPos, zPos, insideInnerGrid);
        }

        if (xPos == 0 && zPos == 0)
        {
            lowestHeight = perlinNoiseHeight;
            highestHeight = perlinNoiseHeight;
        }
        else
        {
            lowestHeight = perlinNoiseHeight < lowestHeight ? perlinNoiseHeight : lowestHeight;
            highestHeight = perlinNoiseHeight > highestHeight ? perlinNoiseHeight : highestHeight;
        }

        return perlinNoiseHeight;
    }

    private void InstantiateTile(int xPos, int zPos, float newHeight = 1)
    {
        Debug.Log($"{xPos} | {xPos - InnerGridSettings.GridXStart}");
        float xActualPos = xPos + ((xPos - InnerGridSettings.GridXStart) * tileXSpaceCorrection);
        float zActualPos = zPos + ((zPos - InnerGridSettings.GridZStart) * tileZSpaceCorrection);
        if (zPos % 2 == 1) xActualPos += tileXOddOffset;

        GameObject tile = Instantiate(tileParentPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
        tile.transform.localPosition = new Vector3(xActualPos, 0, zActualPos);
        tile.transform.localScale = new Vector3(tile.transform.localScale.x, newHeight, tile.transform.localScale.z);
        tile.name = $"Hex coord {xPos},{zPos}";
    }

    private void SetTypeOfTiles()
    {
        for (int i = 0; i < gridSystemDB.Tiles.Count; i++)
        {
            GridTileInfo tileInfo = gridSystemDB.Tiles[i];
            Transform tile = transform.GetChild(i);
            TileSetup tileSetup = tile.GetComponent<TileSetup>();

            TileTypeSettings settings = null;
            bool skipTile = false;
            for (int j = 0; j < TileTypeSettings.Count; j++)
            {
                if (tileSetup.SelectedTileTypeIndex == j && TileTypeSettings[j].SkipNextGen)
                {
                    settings = TileTypeSettings[j];
                    skipTile = true;
                }
            }

            if (!skipTile)
            {
                settings = GetTileTypeSettings(tileInfo.Height, tileInfo.OuterGrid, tileInfo.InnerGrid);
                tileSetup.SetTileType(settings.Name);
                tileSetup.UpdateTile();
            }

            tile.localScale = new Vector3(tile.localScale.x, tile.localScale.y + settings.HeightOffset, tile.localScale.z);
        }
    }
    
    private TileTypeSettings GetTileTypeSettings(float currentHeight, bool outerGrid, bool innerGrid)
    {
        TileTypeSettings returnSettings = null;
        foreach (TileTypeSettings settings in TileTypeSettings)
        {
            if (returnSettings == null && !settings.SkipNextGen && (outerGrid && settings.OuterGrid || innerGrid && settings.InnerGrid))
            {
                returnSettings = settings.IsHeightBelowMaxHeight(currentHeight, lowestHeight, highestHeight) ? settings : returnSettings;
            }
        }
        return returnSettings;
    }
}