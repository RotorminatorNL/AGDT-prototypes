using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BezierCurve
{
    public Vector3 CalculateQuadraticCurvePoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = (uu * p0) + (2f * u * t * p1) + (tt * p2);
        return new Vector3(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));
    }
}
