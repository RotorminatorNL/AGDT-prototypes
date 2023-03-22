using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransitionSettings
{
    public int Length = 10;
    [Range(0f, 10f)] public float Curve = 1f;
    [HideInInspector] public Dictionary<int, float> XVertices;
    [HideInInspector] public Dictionary<int, float> ZVertices;

    public void SetTransitionPercentages(int innerGridStart, int innerGridEnd, int innerGridLength, string axis = "X")
    {
        Dictionary<int, float> transitionVertices = new Dictionary<int, float>();
        bool otherSide = false;
        for (int x = Length, i = innerGridStart - Length; i <= innerGridEnd + Length; i++)
        {
            if (i == innerGridStart)
            {
                i += innerGridLength + 1;
                x = 1;
                otherSide = true;
            }

            float percentage = CalculatePercentage(otherSide == false ? x-- : x++);
            transitionVertices.Add(i, percentage);
        }

        if (axis == "X") XVertices = transitionVertices;
        else if (axis == "Z") ZVertices = transitionVertices;
    }

    private float CalculatePercentage(float currentStep)
    {
        if (currentStep == Length) return 1f;
        else if (currentStep == 0f) return 0f;
        else
        {
            float percentage = currentStep / Length;
            percentage = Mathf.Pow(percentage, Curve);
            percentage = Mathf.Lerp(0f, 100f, percentage);
            return percentage / 100f;
        }
    }
}
