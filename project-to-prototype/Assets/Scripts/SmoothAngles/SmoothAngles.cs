using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothAngles : MonoBehaviour
{
    [SerializeField] private bool drawGizmo = false;
    [Space(10)]
    [SerializeField, Min(0)] private int gridLengthX;
    [SerializeField] private float spaceCorrectionX = 1;
    [SerializeField] private float offsetX = 0.5f;
    [SerializeField, Min(0)] private int gridLengthZ;
    [SerializeField] private float spaceCorrectionZ = 1;
    [Space(5)]
    [SerializeField] private float controlPointWeight;
    [SerializeField] private BezierCurvePoint[] anchorPoints;

    [SerializeField, HideInInspector] private BezierCurve bezierCurve;

    private List<Vector3> totalPoints;

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmo) return;
        drawGizmo = false;

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

        for (int cpIndex = 0; cpIndex < anchorPoints.Length; cpIndex++)
        {
            BezierCurvePoint cp = anchorPoints[cpIndex];

            float actualX = cp.Coord.x * spaceCorrectionX;
            if (cp.Coord.y % 2 == 1) actualX += offsetX * spaceCorrectionX;

            float actualZ = cp.Coord.y * spaceCorrectionZ;

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(new Vector3(actualX, 1, actualZ), .2f);
        }

        totalPoints = new List<Vector3>();

        for (int i = 0; i < anchorPoints.Length; i++)
        {
            if(i == 0)
            {
                totalPoints.Add(anchorPoints[i].Coord);
                totalPoints.Add(new Vector2());
            }
            else if (i == anchorPoints.Length - 1)
            {
                totalPoints.Add(new Vector2());
                totalPoints.Add(anchorPoints[i].Coord);
            }
            else
            {
                totalPoints.Add(new Vector2());
                totalPoints.Add(anchorPoints[i].Coord);
                totalPoints.Add(new Vector2());
            }
        }

        for (int i = 0; i < anchorPoints.Length; i++)
        {
            int apIndex = i * 3;
            Vector3 anchorPos = totalPoints[apIndex];
            Vector3 dir = Vector3.zero;
            float[] neighbourDistances = new float[2];

            if (apIndex - 3 >= 0)
            {
                Vector3 offset = totalPoints[apIndex - 3] - anchorPos;
                dir += offset.normalized;
                neighbourDistances[0] = offset.magnitude;
            }
            if (apIndex + 3 < totalPoints.Count)
            {
                Vector3 offset = totalPoints[apIndex + 3] - anchorPos;
                dir -= offset.normalized;
                neighbourDistances[1] = -offset.magnitude;
            }

            dir.Normalize();

            // Set the control points along the calculated direction, with a distance proportional to the distance to the neighbouring control point
            for (int j = 0; j < 2; j++)
            {
                int controlIndex = apIndex + j * 2 - 1;
                if (controlIndex >= 0 && controlIndex < totalPoints.Count)
                {
                    Vector3 coord = anchorPos + dir * neighbourDistances[j] * controlPointWeight;
                    totalPoints[controlIndex] = new Vector3(Mathf.RoundToInt(coord.x), 0, Mathf.RoundToInt(coord.y));
                }
            }
        }

        for (int i = 0; i < totalPoints.Count; i++)
        {
            Vector3 coord = totalPoints[i];
            float actualX = coord.x * spaceCorrectionX;
            if (coord.z % 2 == 1) actualX += offsetX * spaceCorrectionX;

            float actualZ = coord.z * spaceCorrectionZ;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(actualX, 1, actualZ), .2f);
        }

        //for (int i = 0; i < 50; i++)
        //{
        //    Vector3 curvePoint = bezierCurve.CalculateQuadraticCurvePoint(controlPoints[0].Coord, controlPoints[1].Coord, controlPoints[2].Coord, controlPoints[3].Coord, (float)i / 50, controlPointWeight);

        //    float actualX2 = curvePoint.x * spaceCorrectionX;
        //    if (curvePoint.y % 2 == 1) actualX2 += offsetX * spaceCorrectionX;

        //    float actualZ2 = curvePoint.y * spaceCorrectionZ;

        //    Gizmos.color = Color.red;
        //    Gizmos.DrawSphere(new Vector3(actualX2, 1, actualZ2), .2f);
        //}
    }
}