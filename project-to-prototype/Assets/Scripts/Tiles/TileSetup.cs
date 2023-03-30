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

    public int SelectedTileTypeIndex { get { return selectedTileTypeIndex; } set { selectedTileTypeIndex = value; } }
    private int selectedTileTypeIndex;

    private void Start()
    {
        TileInfo tileInfo = GetComponentInChildren<TileInfo>();
        if (tileInfo == null) return;
        selectedTileTypeIndex = TileTypes.Types.FindIndex(type => type.Name == tileInfo.TileType.Name);
    }

    public bool TileTypeIndexChanged(int index)
    {
        return selectedTileTypeIndex != index;
    }

    public void SetTileType(string genSelectedTileType)
    {
        selectedTileTypeIndex = TileTypes.Types.FindIndex(type => type.Name == genSelectedTileType);
    }

    public void SetTileType(int index)
    {
        selectedTileTypeIndex = index;
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
