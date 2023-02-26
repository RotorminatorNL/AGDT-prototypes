using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransitionSettings
{
    public int TransitionLength = 10;
    [Range(0f, 10f)] public float TransitionCurve = 1f;
    [HideInInspector] public Dictionary<int, float> TransitionXVertices;
    [HideInInspector] public Dictionary<int, float> TransitionZVertices;

    private int previousTransitionLength;
    private float previousTransitionCurve;

    public void UpdateValues()
    {
        previousTransitionLength = TransitionLength;
        previousTransitionCurve = TransitionCurve;
    }

    public bool HasValueChanged()
    {
        if (previousTransitionLength == TransitionLength && previousTransitionCurve == TransitionCurve) return false;

        previousTransitionLength = TransitionLength;
        previousTransitionCurve = TransitionCurve;
        return true;
    }

    public void SetTransitionPercentages(int innerGridStart, int innerGridEnd, int innerGridLength, string axis = "X")
    {
        Dictionary<int, float> transitionVertices = new Dictionary<int, float>();
        bool otherSide = false;
        for (int x = TransitionLength, i = innerGridStart - TransitionLength; i <= innerGridEnd + TransitionLength; i++)
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

        if (axis == "X") TransitionXVertices = transitionVertices;
        else if (axis == "Z") TransitionZVertices = transitionVertices;
    }

    private float CalculatePercentage(float currentStep)
    {
        if (currentStep == TransitionLength) return 1f;
        else if (currentStep == 0f) return 0f;
        else
        {
            float percentage = currentStep / TransitionLength;
            percentage = Mathf.Pow(percentage, TransitionCurve);
            percentage = Mathf.Lerp(0f, 100f, percentage);
            return percentage / 100f;
        }
    }
}
