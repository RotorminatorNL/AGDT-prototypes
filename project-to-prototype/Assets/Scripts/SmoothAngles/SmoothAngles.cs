using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothAngles : MonoBehaviour
{
    [SerializeField, Min(0)] private int gridLengthX;
    [SerializeField] private float spaceCorrectionX = 1;
    [SerializeField, Min(0)] private int gridLengthZ;
    [SerializeField] private float spaceCorrectionZ = 1;

    [SerializeField] private ControlPoint[] controlPoints;

    private BezierCurve bezierCurve;

    private void OnDrawGizmosSelected()
    {
        for (int z = 0; z < gridLengthZ; z++)
        {
            for (int x = 0; x < gridLengthX; x++)
            {
                float actualX = x * spaceCorrectionX;
                if (z % 2 == 1) actualX += .5f * spaceCorrectionX;

                float actualZ = z * spaceCorrectionZ;

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(new Vector3(actualX, 1, actualZ), .2f);
            }
        }

        for (int cpIndex = 0; cpIndex < controlPoints.Length; cpIndex++)
        {
            ControlPoint cp = controlPoints[cpIndex];

            float actualX = cp.Coord.x * spaceCorrectionX;
            if (cp.Coord.y % 2 == 1) actualX += .5f * spaceCorrectionX;

            float actualZ = cp.Coord.y * spaceCorrectionZ;

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(new Vector3(actualX, 1, actualZ), .2f);
        }

        for (int i = 0; i < 20; i++)
        {
            Vector3 curvePoint = bezierCurve.CalculateQuadraticCurvePoint(controlPoints[0].Coord, controlPoints[1].Coord, controlPoints[2].Coord, (float)i / 20);

            float actualX = curvePoint.x * spaceCorrectionX;
            if (curvePoint.y % 2 == 1) actualX += .5f * spaceCorrectionX;

            float actualZ = curvePoint.y * spaceCorrectionZ;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(actualX, 1, actualZ), .2f);
        }
    }
}