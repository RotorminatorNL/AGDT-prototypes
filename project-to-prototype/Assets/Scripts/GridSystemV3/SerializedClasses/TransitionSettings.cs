using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransitionSettings
{
    public int TransitionLength = 20;
    [Range(0f, 10f)] public float TransitionCurve = 1.5f;
    [HideInInspector] public Dictionary<int, float> TransitionXVertices;
    [HideInInspector] public Dictionary<int, float> TransitionZVertices;
}
