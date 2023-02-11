using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileChance : MonoBehaviour
{
    public string TileName;
    [Range(0, 10)] public int Chance;

    TileChance(string tileName, int tileChance)
    {
        this.TileName = tileName;
        this.Chance = tileChance;
    }
}
