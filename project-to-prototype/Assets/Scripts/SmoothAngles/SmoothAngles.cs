using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothAngles : MonoBehaviour
{
    [SerializeField, Min(0)] private int gridLengthX;
    [SerializeField] private float spaceCorrectionX = 1;
    [SerializeField] private float offsetX = 0.5f;
    [SerializeField, Min(0)] private int gridLengthZ;
    [SerializeField] private float spaceCorrectionZ = 1;
    [Space(5)]
    [SerializeField] private bool showCurve = true;
    [SerializeField] private bool showControlPoints = true;
    [SerializeField] private float extraCurvesOffset = 0.4f;
    [SerializeField] private float controlPointWeight = 0.3f;
    [SerializeField] private BezierCurvePoint[] anchorPoints;

    [SerializeField, HideInInspector] private BezierCurve bezierCurve;

    private List<Vector3> actualAnchorPoints;

    private void OnDrawGizmosSelected()
    {
        for (int z = 0; z < gridLengthZ; z++)
        {
            for (int x = 0; x < gridLengthX; x++)
            {
                float actualX = x * spaceCorrectionX;
                if (z % 2 == 1) actualX += offsetX * spaceCorrectionX;

                float actualZ = z * spaceCorrectionZ;

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(new Vector3(actualX, 1, actualZ), .2f);
            }
        }

        actualAnchorPoints = GetActualAnchorPoints();

        foreach (Vector3 point in actualAnchorPoints)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(new Vector3(point.x, 1, point.z), .5f);
        }

        if (anchorPoints.Length == 1) return;

        List<Vector3> totalPoints = CreateTotalPoints();
        totalPoints = SetTotalPoints(totalPoints);

        if (showControlPoints)
        {
            for (int i = 0; i < totalPoints.Count; i++)
            {
                Vector3 coord = totalPoints[i];

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(new Vector3(coord.x, 1, coord.z), .3f);
            }
        }

        int curvePosLength = gridLengthX + gridLengthZ;
        for (int loop = 0, ap = 0; ap < anchorPoints.Length - 1; ap++)
        {
            for (int curvePos = 0; curvePos < curvePosLength; curvePos++)
            {
                Vector3 curvePoint = bezierCurve.CalculateQuadraticCurvePoint(totalPoints[loop + 0], totalPoints[loop + 1], totalPoints[loop + 2], totalPoints[loop + 3], (float)curvePos / curvePosLength);

                Vector3 curvePoint1 = new Vector3(curvePoint.x - extraCurvesOffset, 0, curvePoint.z);
                Vector3 curvePoint2 = new Vector3(curvePoint.x, 0, curvePoint.z - extraCurvesOffset);
                Vector3 curvePoint3 = new Vector3(curvePoint.x + extraCurvesOffset, 0, curvePoint.z);
                Vector3 curvePoint4 = new Vector3(curvePoint.x, 0, curvePoint.z + extraCurvesOffset);

                if (showCurve)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawSphere(new Vector3(curvePoint.x, 1, curvePoint.z), .2f);
                    Gizmos.DrawSphere(new Vector3(curvePoint1.x, 1, curvePoint1.z), .1f);
                    Gizmos.DrawSphere(new Vector3(curvePoint2.x, 1, curvePoint2.z), .1f);
                    Gizmos.DrawSphere(new Vector3(curvePoint3.x, 1, curvePoint3.z), .1f);
                    Gizmos.DrawSphere(new Vector3(curvePoint4.x, 1, curvePoint4.z), .1f);
                }

                Vector3 actualCurvePoint = GetActualCurvePoint(curvePoint);
                Vector3 actualCurvePoint1 = GetActualCurvePoint(curvePoint1);
                Vector3 actualCurvePoint2 = GetActualCurvePoint(curvePoint2);
                Vector3 actualCurvePoint3 = GetActualCurvePoint(curvePoint3);
                Vector3 actualCurvePoint4 = GetActualCurvePoint(curvePoint4);

                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(new Vector3(actualCurvePoint.x, 1, actualCurvePoint.z), .2f);
                Gizmos.DrawSphere(new Vector3(actualCurvePoint1.x, 1, actualCurvePoint1.z), .2f);
                Gizmos.DrawSphere(new Vector3(actualCurvePoint2.x, 1, actualCurvePoint2.z), .2f);
                Gizmos.DrawSphere(new Vector3(actualCurvePoint3.x, 1, actualCurvePoint3.z), .2f);
                Gizmos.DrawSphere(new Vector3(actualCurvePoint4.x, 1, actualCurvePoint4.z), .2f);
            }
            loop += 3;
        }
    }

    private List<Vector3> GetActualAnchorPoints()
    {
        List<Vector3> points = new List<Vector3>();
        foreach (BezierCurvePoint point in anchorPoints)
        {
            float actualX = point.Coord.x * spaceCorrectionX;
            if (point.Coord.z % 2 == 1) actualX += offsetX * spaceCorrectionX;

            float actualZ = point.Coord.z * spaceCorrectionZ;

            points.Add(new Vector3(actualX, 0, actualZ));
        }
        return points;
    }

    private List<Vector3> CreateTotalPoints()
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < anchorPoints.Length; i++)
        {
            if (i == 0)
            {
                points.Add(actualAnchorPoints[i]);
                points.Add(new Vector3());
            }
            else if (i == anchorPoints.Length - 1)
            {
                points.Add(new Vector3());
                points.Add(actualAnchorPoints[i]);
            }
            else
            {
                points.Add(new Vector3());
                points.Add(actualAnchorPoints[i]);
                points.Add(new Vector3());
            }
        }
        return points;
    }

    private List<Vector3> SetTotalPoints(List<Vector3> totalPoints)
    {
        List<Vector3> points = totalPoints;
        for (int ap = 0; ap < anchorPoints.Length; ap++)
        {
            int apIndex = ap * 3;
            Vector3 apPos = points[apIndex];
            Vector3 dir = Vector3.zero;
            float[] distances = new float[2];

            if (apIndex - 3 >= 0)
            {
                Vector3 offset = points[apIndex - 3] - apPos;
                dir += offset.normalized;
                distances[0] = offset.magnitude;
            }
            if (apIndex + 3 < points.Count)
            {
                Vector3 offset = points[apIndex + 3] - apPos;
                dir -= offset.normalized;
                distances[1] = -offset.magnitude;
            }

            dir.Normalize();

            for (int cp = 0; cp < 2; cp++)
            {
                int cpIndex = apIndex + cp * 2 - 1;
                if (cpIndex >= 0 && cpIndex < points.Count)
                {
                    Vector3 coord = apPos + dir * distances[cp] * controlPointWeight;
                    points[cpIndex] = new Vector3(coord.x, 0, coord.z);
                }
            }
        }
        return points;
    }

    private Vector3 GetActualCurvePoint(Vector3 curvePoint)
    {
        float previousDistance = float.MaxValue;
        Vector3 point = Vector3.zero;
        for (int z = 0; z < gridLengthZ; z++)
        {
            for (int x = 0; x < gridLengthX; x++)
            {
                float xPos = x * spaceCorrectionX;
                if (z % 2 == 1) xPos += offsetX * spaceCorrectionX;

                float zPos = z * spaceCorrectionZ;

                Vector3 gridCoord = new Vector3(xPos, 0, zPos);
                float distance = Vector3.Distance(curvePoint, gridCoord);
                if (previousDistance > distance)
                {
                    previousDistance = distance;
                    point = gridCoord;
                }
            }
        }

        return point;
    }
}