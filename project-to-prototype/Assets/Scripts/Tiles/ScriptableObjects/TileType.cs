using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileType", menuName = "Grid system/Tile type")]
public class TileType : ScriptableObject
{
    public string Name;
    public string PoolKey;
    public GameObject Prefab;
    public LayerMask LayerMask;
    public Material Material;
    public Mesh Mesh;
    public bool MeshColliderConvex;
    public bool MeshColliderIsTrigger;
    public bool Placeable;
}
