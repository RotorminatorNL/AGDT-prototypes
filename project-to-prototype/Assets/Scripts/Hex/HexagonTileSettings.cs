using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class HexagonTileSettings : MonoBehaviour
{
    public HexagonTileTypes TileTypes { set; get; }
    private bool hasSpawned = false;

    public int SelectedTileTypeIndex { get { return selectedTileTypeIndex; } set { selectedTileTypeIndex = value; } }
    private int selectedTileTypeIndex;
    private int previousTileTypeIndex;

    private void Awake()
    {
        TileTypes = GetComponentInParent<GridSystemV3_2>().TileTypes;
    }

    private void Start()
    {
        HexagonTileInfo hexagonTileInfo = GetComponentInChildren<HexagonTileInfo>();
        if (hexagonTileInfo == null) return;
        hasSpawned = true;

        int index = TileTypes.Types.FindIndex(type => type.Name == hexagonTileInfo.HexagonTileType.Name);
        previousTileTypeIndex = index;
        selectedTileTypeIndex = index;
    }

    private void Update()
    {
        if (TileTypes == null || !hasSpawned || selectedTileTypeIndex == 0 || previousTileTypeIndex == selectedTileTypeIndex) return;
        UpdateTileType();
        previousTileTypeIndex = selectedTileTypeIndex;
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
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
