using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

[ExecuteInEditMode]
public class HexagonTileSettings : MonoBehaviour
{
    public HexagonTileTypes TileTypes { set; get; }
    private bool hasSpawned = false;

    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private PlaceableTile placeableTile;

    private GridSystemV3_2 gridSystem;

    public int SelectedTileTypeIndex { get { return selectedTileTypeIndex; } set { selectedTileTypeIndex = value; } }
    private int selectedTileTypeIndex;
    private int previousTileTypeIndex;

    private void Awake()
    {
        gridSystem = GetComponentInParent<GridSystemV3_2>();
        TileTypes = gridSystem?.TileTypes;
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
        gridSystem.RoadMesh.BuildNavMesh();
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
        HexagonTileType selectedTile = TileTypes.Types[selectedTileTypeIndex];
        int layer = (int)Mathf.Log(selectedTile.LayerMask.value, 2);
        gameObject.layer = layer;
        meshFilter.mesh = selectedTile.Mesh;
        meshRenderer.material = selectedTile.Material;
        if (!selectedTile.MeshColliderConvex) meshCollider.enabled = false;
        else
        {
            meshCollider.enabled = true;
            meshCollider.convex = selectedTile.MeshColliderConvex;
            meshCollider.isTrigger = selectedTile.MeshColliderIsTrigger;
            meshCollider.sharedMesh = selectedTile.Mesh;
        }
        placeableTile.enabled = selectedTile.Placeable;

        // add/replace child
        if (transform.childCount != 0) DestroyImmediate(transform.GetChild(0).gameObject);
        Vector3 worldPosition = gameObject.transform.TransformPoint(Vector3.zero);
        GameObject hexInfo = Instantiate(selectedTile.Prefab, worldPosition, Quaternion.identity, transform);
        hexInfo.name = selectedTile.Name;
    }
}
