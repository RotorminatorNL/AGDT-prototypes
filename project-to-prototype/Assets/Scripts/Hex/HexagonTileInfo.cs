using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonTileInfo : MonoBehaviour
{
    public HexagonTileType HexagonTileType { get { return hexagonTileType; } }
    [SerializeField] private HexagonTileType hexagonTileType;
}
