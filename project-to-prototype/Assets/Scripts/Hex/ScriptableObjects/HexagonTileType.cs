using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileType", menuName = "Grid system/Tile type")]
public class HexagonTileType : ScriptableObject
{
    public string Name;
    public string PoolKey;
    public GameObject Prefab;
}
