using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

[ExecuteInEditMode]
public class HexTileSettings : MonoBehaviour
{
    public HexTileTypes HexTileTypes { get { return hexTileTypes; } }
    [SerializeField] private HexTileTypes hexTileTypes;
    private bool hasSpawned = true;

    public int SelectedTileTypeIndex { get { return selectedTileTypeIndex; } set { selectedTileTypeIndex = value; } }
    private int selectedTileTypeIndex;
    private int previousTileTypeIndex;

    private void Update()
    {
        if (previousTileTypeIndex == selectedTileTypeIndex || hasSpawned) return;
        UpdateTileType();
        previousTileTypeIndex = selectedTileTypeIndex;
    }

    public void SetTileType(int genSelectedTileTypeIndex) 
    {
        selectedTileTypeIndex = genSelectedTileTypeIndex;
        previousTileTypeIndex = selectedTileTypeIndex;
        hasSpawned = false;
    }

    public void UpdateTileType()
    {
        if (transform.childCount != 0) DestroyImmediate(transform.GetChild(0).gameObject);
        Vector3 worldPosition = gameObject.transform.TransformPoint(Vector3.zero);
        Tile selectedTile = hexTileTypes.TileTypes[selectedTileTypeIndex];
        GameObject hex = Instantiate(selectedTile.TilePrefab, worldPosition, Quaternion.identity, transform);
        hex.name = selectedTile.TileName;
    }
}
