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
    public List<TileTypeSettings> TileTypeSettings = new List<TileTypeSettings>();

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

    public void ActivateGenerateGrid()
    {
        if (!AbleToGenerate()) return;

        ClearGrid();

        InnerGridSettings.CalculateBorders(OuterGridSettings);
        transitionSettings.CalculateBorders(InnerGridSettings);

        GenerateTypelessTiles();
        SetTypeOfTiles();

        NavMeshRoad.BuildNavMesh();
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }

    private bool AbleToGenerate()
    {
        bool outerGridSizeWrong = OuterGridSettings.GridXLength <= 0 || OuterGridSettings.GridZLength <= 0;
        bool innerGridSizeWrong = InnerGridSettings.GridXLength <= 0 || InnerGridSettings.GridZLength <= 0;
        if (gridSystemDB == null || tileParentPrefab == null || outerGridSizeWrong || innerGridSizeWrong || transitionSettings.Length <= 0) return false;
        return true;
    }

    public void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);
        gridSystemDB.ClearAllTiles();
        NavMeshRoad.BuildNavMesh();
    }

    private void GenerateTypelessTiles()
    {
        for (int z = 0; z < OuterGridSettings.GridZLength; z++)
        {
            for (int x = 0; x < OuterGridSettings.GridXLength; x++)
            {
                bool innerGrid = InnerGridSettings.IsInside(x, z);
                float heightValue = GetHeightValue(x, z, innerGrid);

                GameObject generatedTile = InstantiateTile(x, z, heightValue);
                gridSystemDB.StoreTile(generatedTile.name, generatedTile.GetComponent<TileSetup>(), heightValue, !innerGrid, innerGrid);

                if (x == 0 && z == 0)
                {
                    lowestHeight = heightValue;
                    highestHeight = heightValue;
                }
                else
                {
                    lowestHeight = heightValue < lowestHeight ? heightValue : lowestHeight;
                    highestHeight = heightValue > highestHeight ? heightValue : highestHeight;
                }
            }
        }
    }

    private float GetHeightValue(int x, int z, bool insideInnerGrid)
    {
        float transitionPercentage = transitionSettings.GetTransitionPercentage(x, z);
        if (!insideInnerGrid && transitionPercentage != 1)
        {
            float outerPerlinNoise = perlinNoiseSettings.GetPerlinNoiseValue(x, z);
            float innerPerlinNoise = perlinNoiseSettings.GetPerlinNoiseValue(x, z, true);
            return innerPerlinNoise + ((outerPerlinNoise - innerPerlinNoise) * transitionPercentage);
        }
        float perlinNoise = perlinNoiseSettings.GetPerlinNoiseValue(x, z, insideInnerGrid);
        return perlinNoise;
    }

    private GameObject InstantiateTile(int xPos, int zPos, float newHeight = 1)
    {
        float x = xPos + ((xPos - InnerGridSettings.GridXStart) * tileXSpaceCorrection);
        float z = zPos + ((zPos - InnerGridSettings.GridZStart) * tileZSpaceCorrection);
        if (zPos % 2 == 1) x += tileXOddOffset;

        GameObject tile = Instantiate(tileParentPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
        tile.transform.localPosition = new Vector3(x, 0, z);
        tile.transform.localScale = new Vector3(tile.transform.localScale.x, newHeight, tile.transform.localScale.z);
        tile.name = $"Hex coord {xPos},{zPos}";
        return tile;
    }

    private void SetTypeOfTiles()
    {
        for (int i = 0; i < gridSystemDB.Tiles.Count; i++)
        {
            string tileTypeName = GetTileTypeName(gridSystemDB.Tiles[i].Height, gridSystemDB.Tiles[i].OuterGrid, gridSystemDB.Tiles[i].InnerGrid);
            gridSystemDB.Tiles[i].TileSettings.SetTileType(tileTypeName);
            gridSystemDB.Tiles[i].TileSettings.UpdateTile();
        }
    }
    
    private string GetTileTypeName(float currentHeight, bool outerGrid, bool innerGrid)
    {
        string tileTypeName = "";
        foreach (TileTypeSettings tile in TileTypeSettings)
        {
            if (tileTypeName == "" && (outerGrid && tile.OuterGrid || innerGrid && tile.InnerGrid))
            {
                tileTypeName = tile.IsHeightBelowMaxHeight(currentHeight, lowestHeight, highestHeight) ? tile.Name : tileTypeName;
            }
        }
        return tileTypeName;
    }
}