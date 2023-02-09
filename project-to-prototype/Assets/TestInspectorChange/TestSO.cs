using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tests/Test")]
public class TestSO : ScriptableObject
{
    public enum Tests
    {
        Test1,
        Test2
    }

    public Tests test;

    public float range;
    public float fireRate;

    public int towerCost;
    public int sellValue;
}
