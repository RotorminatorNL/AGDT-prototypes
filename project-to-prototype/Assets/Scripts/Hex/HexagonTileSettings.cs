using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

[ExecuteInEditMode]
public class HexagonTileSettings : MonoBehaviour
{
    public HexagonTileTypes TileTypes { set; get; }
    private bool hasSpawned = true;

    public int SelectedTileTypeIndex { get { return selectedTileTypeIndex; } set { selectedTileTypeIndex = value; } }
    private int selectedTileTypeIndex;
    private int previousTileTypeIndex;

    private void Update()
    {
        if ((previousTileTypeIndex == selectedTileTypeIndex && selectedTileTypeIndex == 0 || hasSpawned) && TileTypes == null) return;
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
        HexagonTileType selectedTile = TileTypes.Types[selectedTileTypeIndex];
        GameObject hex = Instantiate(selectedTile.Prefab, worldPosition, Quaternion.identity, transform);
        hex.name = selectedTile.Name;
    }
}
