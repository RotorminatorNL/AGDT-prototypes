using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BezierCurvePoint
{
    public Vector3 Coord;
    public BezierCurvePoint(Vector3 coord) { Coord = coord; }
}
