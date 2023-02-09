using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileTypes", menuName = "Tile/Tile types")]
public class HexTileTypes : ScriptableObject
{
    public enum TileType
    {
        Standard,
        Grass,
        Water,
        Road
    }

    [SerializeField] private GameObject standard;
    [SerializeField] private GameObject grass;
    [SerializeField] private GameObject water;
    [SerializeField] private GameObject road;

    public GameObject GetTileType(TileType tileType)
    {
        switch (tileType)
        {
            case TileType.Standard:
                return standard;
            case TileType.Grass:
                return grass;
            case TileType.Water:
                return water;
            case TileType.Road:
                return road;
        }
        return null;
    }
}
