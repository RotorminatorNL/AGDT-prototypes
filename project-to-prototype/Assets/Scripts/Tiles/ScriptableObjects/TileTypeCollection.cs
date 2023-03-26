using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileTypeCollection", menuName = "Grid system/Tile type collection")]
public class TileTypeCollection : ScriptableObject
{
    public List<TileType> Types = new List<TileType>();
}
