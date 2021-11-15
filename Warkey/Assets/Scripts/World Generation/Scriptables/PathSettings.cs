using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PathSettings : UpdateableData
{
    public int seed;
    [Range(0, 100000)]
    public int maxIteration;
    [Range(0, 100000)]
    public int maxSubIteration;
    public MinMax distance;
    [Range(0.0f,0.1f)]
    public float maxSteepChange;
    public MinMax height;
    [Range(0, 1)]
    public float negativeStrength;

    [Header("Branch")]
    [Range(0, 1)]
    public float branchChance;
    [Range(0, 100000)]
    public int branchIteration;
    public float branchWidth;
    [Range(0, 1)]
    public float branchNegativeStrength;


    public float startRadius;
    public float pathWidth;
    public float pathHeightOffset;
    public float smoothWeight;
}