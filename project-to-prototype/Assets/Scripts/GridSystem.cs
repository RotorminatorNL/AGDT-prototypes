using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridSystem : MonoBehaviour
{
    [SerializeField] private HexTileTypes hexTileTypes;
    [SerializeField] private float hexHeightOffset;
    [SerializeField] private int gridHeight;
    [SerializeField] private int gridWidth;
    [SerializeField] private float gridWidthOffset;

    private void Start()
    {
        GenerateHexGrid();
    }

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
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                GenerateHexTile(i, j);
            }
        }
    }

    private void GenerateHexTile(int xPos, int zPos)
    {
        GameObject hexPrefab = hexTileTypes.GetTileType(0);
        GameObject hex = Instantiate(hexPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
        hex.GetComponent<HexTileSettings>().SetTileType((HexTileTypes.TileType)Random.Range(1, 3));
        hex.GetComponent<HexTileSettings>().UpdateTileType();

        float hexWidth = hex.transform.GetChild(0).GetComponent<Renderer>().bounds.size.x;
        float hexHeight = hex.transform.GetChild(0).GetComponent<Renderer>().bounds.size.z;

        float x = xPos * hexWidth;
        float z = zPos * hexHeight * hexHeightOffset;

        if (zPos % 2 == 1) x += hexWidth * gridWidthOffset;

        hex.transform.position = new Vector3(x, 0, z);
        hex.name = $"Hex {xPos},{zPos}";
    }
}
