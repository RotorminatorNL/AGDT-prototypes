using UnityEngine;

[ExecuteInEditMode]
public class HexTileSettings : MonoBehaviour
{
    [SerializeField] private HexTileTypes hexTileTypes;
    [SerializeField] private HexTileTypes.TileType tileType;
    private HexTileTypes.TileType previousTileType;
    private bool hasSpawned = true;

    private void Update()
    {
        if (previousTileType == tileType || hasSpawned) return;
        UpdateTileType();
        previousTileType = tileType;
    }

    public void SetTileType(HexTileTypes.TileType newTileType) 
    { 
        tileType = newTileType;
        previousTileType = tileType;
        hasSpawned = false;
    }

    public void UpdateTileType()
    {
        if (transform.childCount != 0) DestroyImmediate(transform.GetChild(0).gameObject);
        Vector3 worldPosition = gameObject.transform.TransformPoint(Vector3.zero);
        GameObject hex = Instantiate(hexTileTypes.GetTileType(tileType), worldPosition, Quaternion.identity, transform);
        hex.name = tileType.ToString();
    }
}
