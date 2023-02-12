using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

[ExecuteInEditMode]
public class GridSystem : MonoBehaviour
{
    public HexTileTypes HexTileTypes { get { return hexTileTypes; } }

    [Header("Grid tile types")]
    [SerializeField] private HexTileTypes hexTileTypes;
    public List<TileGenerationChance> Tiles = new List<TileGenerationChance>();

    [Header("Grid size")]
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private int gridWidth = 10;

    [Header("Grid gen options")]
    [SerializeField] private float hexHeightOffset = 0.75f;
    [SerializeField] private float gridOddOffset = 0.5f;

    private List<int> tilePool;

    public void ClearHexGrid()
    {
        for (int i = transform.childCount - 1; i >= 0 ; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public void GenerateHexGrid()
    {
        ClearHexGrid();
        CreateTilePool();
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                GenerateHexTile(i, j);
            }
        }
    }

    private void CreateTilePool()
    {
        Tiles[0].TileChance = 0;
        tilePool = new List<int>();
        for (int i = 0; i < Tiles.Count; i++)
        {
            for (int j = 0; j < (Tiles[i].TileChance * 10); j++)
            {
                tilePool.Add(i);
            }
        }
    }

    private void GenerateHexTile(int xPos, int zPos)
    {
        if (hexTileTypes == null) return;
        GameObject hexPrefab = hexTileTypes.TileTypes[0].TilePrefab;
        GameObject hex = Instantiate(hexPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);

        hex.GetComponent<HexTileSettings>().SetTileType(GetTileType(xPos, zPos));
        hex.GetComponent<HexTileSettings>().UpdateTileType();

        float hexWidth = hex.transform.GetChild(0).GetComponent<Renderer>().bounds.size.x;
        float hexHeight = hex.transform.GetChild(0).GetComponent<Renderer>().bounds.size.z;

        float x = xPos * hexWidth;
        float z = zPos * hexHeight * hexHeightOffset;

        if (zPos % 2 == 1) x += hexWidth * gridOddOffset;

        hex.transform.position = new Vector3(x, 0, z);
        hex.name = $"Hex {xPos},{zPos}";
    }

    private int GetTileType(int xPos, int zPos)
    {
        return tilePool[Random.Range(0, tilePool.Count)];
    }
}
