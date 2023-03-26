using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public TileType TileType { get { return tileType; } }
    [SerializeField] private TileType tileType;
}
