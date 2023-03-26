using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class TileSetup : MonoBehaviour
{
    public TileTypeCollection TileTypes;
    private bool hasSpawned = false;

    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private PlaceableTile placeableTile;

    private GridSystemV3_2 gridSystem;

    public int SelectedTileTypeIndex { get { return selectedTileTypeIndex; } set { selectedTileTypeIndex = value; } }
    private int selectedTileTypeIndex;
    private int previousTileTypeIndex;

    private void Start()
    {
        gridSystem = GetComponentInParent<GridSystemV3_2>();
        hasSpawned = true;

        TileInfo tileInfo = GetComponentInChildren<TileInfo>();
        if (tileInfo == null) return;
        selectedTileTypeIndex = TileTypes.Types.FindIndex(type => type.Name == tileInfo.TileType.Name);
        previousTileTypeIndex = selectedTileTypeIndex;
    }

    private void Update()
    {
        if (TileTypes == null || !hasSpawned || previousTileTypeIndex == selectedTileTypeIndex) return;
        UpdateTile();
        previousTileTypeIndex = selectedTileTypeIndex;
        gridSystem.NavMeshRoad.BuildNavMesh();
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }

    public void SetTileType(string genSelectedTileType)
    {
        selectedTileTypeIndex = TileTypes.Types.FindIndex(type => type.Name == genSelectedTileType);
        previousTileTypeIndex = selectedTileTypeIndex;
        hasSpawned = true;
    }

    public void UpdateTile()
    {
        TileType selectedTile = TileTypes.Types[selectedTileTypeIndex];
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
