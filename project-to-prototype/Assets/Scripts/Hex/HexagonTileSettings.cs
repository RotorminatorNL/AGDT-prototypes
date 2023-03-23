using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

[ExecuteInEditMode]
public class HexagonTileSettings : MonoBehaviour
{
    public HexagonTileTypes TileTypes { set; get; }
    private bool hasSpawned = false;

    public int SelectedTileTypeIndex { get { return selectedTileTypeIndex; } set { selectedTileTypeIndex = value; } }
    private int selectedTileTypeIndex;
    private int previousTileTypeIndex;

    private void Update()
    {
        if (TileTypes == null || !hasSpawned || selectedTileTypeIndex == 0 || previousTileTypeIndex == selectedTileTypeIndex) return;
        UpdateTileType();
        previousTileTypeIndex = selectedTileTypeIndex;
    }

    public void SetTileType(int genSelectedTileTypeIndex)
    {
        selectedTileTypeIndex = genSelectedTileTypeIndex;
        previousTileTypeIndex = selectedTileTypeIndex;
        hasSpawned = true;
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
