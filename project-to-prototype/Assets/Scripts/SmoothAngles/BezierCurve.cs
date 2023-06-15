using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BezierCurve
{
    public Vector3 CalculateQuadraticCurvePoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float posOnCurve)
    {
        float cpWeight = 3f;

        float posOnCurveReversed = 1f - posOnCurve;
        float posOnCurveReversedSqrd = posOnCurveReversed * posOnCurveReversed;
        float posOnCurveReversedCubed = posOnCurveReversedSqrd * posOnCurveReversed;
        float posOnCurveSqrd = posOnCurve * posOnCurve;
        float posOnCurveCubed = posOnCurveSqrd * posOnCurve;

        Vector3 ap1 = posOnCurveReversedCubed * p0;
        Vector3 cp1 = cpWeight * posOnCurveReversedSqrd * posOnCurve * p1;
        Vector3 cp2 = cpWeight * posOnCurveReversed * posOnCurveSqrd * p2;
        Vector3 ap2 = posOnCurveCubed * p3;

        Vector3 point = ap1 + cp1 + cp2 + ap2;
        return new Vector3(point.x, 0, point.z);
    }
}
